using System.Security.Claims;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ESCOLA_API.Services
{
    public class UsuarioArquivoService : IUsuarioArquivoService
    {
        private const long FotoMaxBytes = 2 * 1024 * 1024;
        private const long PdfMaxBytes = 10 * 1024 * 1024;
        private readonly DataContext _context;
        private readonly IUsuarioArquivoStorage _storage;

        public UsuarioArquivoService(DataContext context, IUsuarioArquivoStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<UsuarioSummaryViewModel?> UploadFotoAsync(int usuarioId, IFormFile arquivo, ClaimsPrincipal principal)
        {
            var usuario = await _context.Usuarios
                .Include(item => item.Perfil)
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null)
            {
                return null;
            }

            ValidarPermissaoArquivo(principal, usuario, permiteProfessorEditarAluno: false);
            ValidarArquivo(arquivo, FotoMaxBytes, new[] { ".jpg", ".jpeg", ".png", ".webp" }, new[] { "image/jpeg", "image/png", "image/webp" });

            var fotoAnteriorUrl = usuario.FotoPerfilUrl;
            await _storage.RemoverAsync(null, usuario.FotoPerfilUrl);
            var arquivoSalvo = await _storage.SalvarAsync(usuarioId, "foto", arquivo);
            usuario.FotoPerfilUrl = arquivoSalvo.Url;

            if (DeveNotificarAlteracaoPropria(principal, usuario))
            {
                await CriarNotificacaoPerfilAtualizadoAsync(
                    usuario,
                    "Foto de perfil atualizada",
                    $"Dados anteriores: Foto de perfil: {FormatarValor(fotoAnteriorUrl)}. Dados atuais: Foto de perfil: {FormatarValor(usuario.FotoPerfilUrl)}.");
            }

            await _context.SaveChangesAsync();

            return usuario.ToSummary();
        }

        public async Task<ArquivoDownload?> DownloadFotoAsync(int usuarioId, ClaimsPrincipal principal)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl))
            {
                return null;
            }

            ValidarPermissaoConsultaFoto(principal, usuario);

            var extensao = ObterExtensaoArquivo(usuario.FotoPerfilUrl);
            var nomeArquivo = string.IsNullOrWhiteSpace(extensao)
                ? $"foto-{usuarioId}"
                : $"foto-{usuarioId}{extensao}";

            return await _storage.AbrirAsync(null, usuario.FotoPerfilUrl, nomeArquivo, null);
        }

        public async Task<UsuarioArquivoViewModel[]> GetArquivosAsync(int usuarioId, ClaimsPrincipal principal)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null)
            {
                return Array.Empty<UsuarioArquivoViewModel>();
            }

            ValidarPermissaoConsultaArquivos(principal, usuario);

            return await _context.UsuarioArquivos
                .AsNoTracking()
                .Where(arquivo => arquivo.IdUsuario == usuarioId)
                .OrderByDescending(arquivo => arquivo.CriadoEmUtc)
                .Select(arquivo => new UsuarioArquivoViewModel
                {
                    IdArquivo = arquivo.IdArquivo,
                    IdUsuario = arquivo.IdUsuario,
                    NomeBlob = arquivo.NomeBlob,
                    TipoArquivo = arquivo.TipoArquivo,
                    NomeOriginal = arquivo.NomeOriginal,
                    Url = arquivo.Url,
                    ContentType = arquivo.ContentType,
                    TamanhoBytes = arquivo.TamanhoBytes,
                    CriadoEmUtc = arquivo.CriadoEmUtc
                })
                .ToArrayAsync();
        }

        public async Task<ArquivoDownload?> DownloadArquivoAsync(int usuarioId, int arquivoId, ClaimsPrincipal principal)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null)
            {
                return null;
            }

            ValidarPermissaoConsultaArquivos(principal, usuario);

            var arquivo = await _context.UsuarioArquivos
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdArquivo == arquivoId && item.IdUsuario == usuarioId);

            if (arquivo == null)
            {
                return null;
            }

            return await _storage.AbrirAsync(arquivo.NomeBlob, arquivo.Url, arquivo.NomeOriginal, arquivo.ContentType);
        }

        public async Task<UsuarioArquivoViewModel> UploadCertificadoAsync(int usuarioId, IFormFile arquivo, ClaimsPrincipal principal)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException("Usuario nao encontrado.");
            }

            if (usuario.IdPerfil != PerfilSistema.ProfessorId)
            {
                throw new InvalidOperationException("Certificados podem ser enviados apenas para professores.");
            }

            ValidarPermissaoArquivo(principal, usuario, permiteProfessorEditarAluno: false);
            ValidarArquivo(arquivo, PdfMaxBytes, new[] { ".pdf" }, new[] { "application/pdf" });

            var arquivoSalvo = await _storage.SalvarAsync(usuarioId, "certificados", arquivo);
            var entity = new UsuarioArquivo
            {
                IdUsuario = usuarioId,
                TipoArquivo = "Certificado",
                NomeOriginal = Path.GetFileName(arquivo.FileName),
                NomeBlob = arquivoSalvo.NomeBlob,
                Url = arquivoSalvo.Url,
                ContentType = arquivo.ContentType,
                TamanhoBytes = arquivo.Length,
                CriadoEmUtc = DateTime.UtcNow
            };

            _context.UsuarioArquivos.Add(entity);

            if (DeveNotificarAlteracaoPropria(principal, usuario))
            {
                await CriarNotificacaoPerfilAtualizadoAsync(
                    usuario,
                    "Certificado PDF adicionado",
                    $"Certificado adicionado: {entity.NomeOriginal}; Tamanho: {entity.TamanhoBytes} bytes; Link: {entity.Url}.");
            }

            await _context.SaveChangesAsync();

            return entity.ToViewModel();
        }

        public async Task<bool> DeleteArquivoAsync(int usuarioId, int arquivoId, ClaimsPrincipal principal)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null)
            {
                return false;
            }

            ValidarPermissaoArquivo(principal, usuario, permiteProfessorEditarAluno: false);

            var arquivo = await _context.UsuarioArquivos
                .FirstOrDefaultAsync(item => item.IdArquivo == arquivoId && item.IdUsuario == usuarioId);

            if (arquivo == null)
            {
                return false;
            }

            await _storage.RemoverAsync(arquivo.NomeBlob, arquivo.Url);
            _context.UsuarioArquivos.Remove(arquivo);
            await _context.SaveChangesAsync();
            return true;
        }

        private static void ValidarArquivo(IFormFile arquivo, long maxBytes, string[] extensoes, string[] contentTypes)
        {
            if (arquivo.Length == 0)
            {
                throw new InvalidOperationException("Arquivo vazio.");
            }

            if (arquivo.Length > maxBytes)
            {
                throw new InvalidOperationException("Arquivo excede o tamanho maximo permitido.");
            }

            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!extensoes.Contains(extensao))
            {
                throw new InvalidOperationException("Tipo de arquivo nao permitido.");
            }

            if (!contentTypes.Contains(arquivo.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Tipo de arquivo nao permitido.");
            }
        }

        private static void ValidarPermissaoArquivo(ClaimsPrincipal principal, Usuario usuario, bool permiteProfessorEditarAluno)
        {
            if (principal.IsInRole(PerfilSistema.Administrador))
            {
                return;
            }

            var usuarioAtualId = GetUsuarioAtualId(principal);
            if (usuario.IdUsuario == usuarioAtualId)
            {
                return;
            }

            if (permiteProfessorEditarAluno && principal.IsInRole(PerfilSistema.Professor) && usuario.IdPerfil == PerfilSistema.AlunoId)
            {
                return;
            }

            throw new UnauthorizedAccessException("Usuario nao autorizado a gerenciar arquivos deste cadastro.");
        }

        private static void ValidarPermissaoConsultaFoto(ClaimsPrincipal principal, Usuario usuario)
        {
            if (principal.IsInRole(PerfilSistema.Administrador))
            {
                return;
            }

            var usuarioAtualId = GetUsuarioAtualId(principal);
            if (usuario.IdUsuario == usuarioAtualId)
            {
                return;
            }

            if (principal.IsInRole(PerfilSistema.Professor)
                && (usuario.IdPerfil == PerfilSistema.AlunoId || usuario.IdPerfil == PerfilSistema.ProfessorId))
            {
                return;
            }

            throw new UnauthorizedAccessException("Usuario nao autorizado a consultar a foto deste cadastro.");
        }

        private static string ObterExtensaoArquivo(string caminhoOuUrl)
        {
            if (Uri.TryCreate(caminhoOuUrl, UriKind.Absolute, out var uri))
            {
                return Path.GetExtension(uri.AbsolutePath);
            }

            return Path.GetExtension(caminhoOuUrl.Split('?', 2)[0]);
        }

        private async Task CriarNotificacaoPerfilAtualizadoAsync(Usuario usuario, string alteracao, string detalhes)
        {
            var administradoresIds = await _context.Usuarios
                .AsNoTracking()
                .Where(item => item.IdPerfil == PerfilSistema.AdministradorId)
                .Select(item => item.IdUsuario)
                .ToArrayAsync();

            if (administradoresIds.Length == 0)
            {
                return;
            }

            var mensagem = $"O usuario {usuario.Nome} ({PerfilSistema.ObterDescricaoPorId(usuario.IdPerfil)}) alterou seu perfil. "
                + $"Alteracao realizada: {alteracao}. {detalhes}";

            foreach (var administradorId in administradoresIds)
            {
                _context.Notificacoes.Add(new Notificacao
                {
                    IdUsuario = administradorId,
                    Tipo = "DadosUsuarioAtualizados",
                    Titulo = "Dados do usuario atualizados",
                    Mensagem = mensagem,
                    Link = $"/usuarios/{usuario.IdUsuario}",
                    CriadaEmUtc = DateTime.UtcNow
                });
            }
        }

        private static bool DeveNotificarAlteracaoPropria(ClaimsPrincipal principal, Usuario usuario)
        {
            return !principal.IsInRole(PerfilSistema.Administrador)
                && usuario.IdUsuario == GetUsuarioAtualId(principal);
        }

        private static string FormatarValor(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "nao cadastrada" : valor;
        }

        private static void ValidarPermissaoConsultaArquivos(ClaimsPrincipal principal, Usuario usuario)
        {
            if (principal.IsInRole(PerfilSistema.Administrador))
            {
                return;
            }

            var usuarioAtualId = GetUsuarioAtualId(principal);
            if (usuario.IdUsuario == usuarioAtualId)
            {
                return;
            }

            if (principal.IsInRole(PerfilSistema.Professor) && usuario.IdPerfil == PerfilSistema.ProfessorId)
            {
                return;
            }

            throw new UnauthorizedAccessException("Usuario nao autorizado a consultar arquivos deste cadastro.");
        }

        private static int GetUsuarioAtualId(ClaimsPrincipal principal)
        {
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var idUsuario) ? idUsuario : 0;
        }
    }
}
