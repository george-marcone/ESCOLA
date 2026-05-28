using System.Security.Claims;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ESCOLA_API.Services
{
    public class HoleriteService : IHoleriteService
    {
        private const long PdfMaxBytes = 10 * 1024 * 1024;
        private readonly DataContext _context;
        private readonly IUsuarioArquivoStorage _storage;

        public HoleriteService(DataContext context, IUsuarioArquivoStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<HoleriteViewModel[]> GetMeusHoleritesAsync(ClaimsPrincipal principal)
        {
            var usuarioId = ValidarFuncionario(principal);
            var holerites = await GetHoleritesQuery()
                .Where(holerite => holerite.IdUsuario == usuarioId)
                .OrderByDescending(holerite => holerite.CompetenciaAno)
                .ThenByDescending(holerite => holerite.CompetenciaMes)
                .ToArrayAsync();

            return holerites.Select(ToViewModel).ToArray();
        }

        public async Task<ArquivoDownload?> DownloadMeuHoleriteAsync(int holeriteId, ClaimsPrincipal principal)
        {
            var usuarioId = ValidarFuncionario(principal);
            return await DownloadHoleriteAsync(usuarioId, holeriteId);
        }

        public async Task<HoleriteViewModel[]> GetHoleritesUsuarioAsync(int usuarioId, ClaimsPrincipal principal)
        {
            ValidarAdministrador(principal);
            await ValidarUsuarioFuncionarioAsync(usuarioId);

            var holerites = await GetHoleritesQuery()
                .Where(holerite => holerite.IdUsuario == usuarioId)
                .OrderByDescending(holerite => holerite.CompetenciaAno)
                .ThenByDescending(holerite => holerite.CompetenciaMes)
                .ToArrayAsync();

            return holerites.Select(ToViewModel).ToArray();
        }

        public async Task<ArquivoDownload?> DownloadHoleriteUsuarioAsync(int usuarioId, int holeriteId, ClaimsPrincipal principal)
        {
            ValidarAdministrador(principal);
            await ValidarUsuarioFuncionarioAsync(usuarioId);
            return await DownloadHoleriteAsync(usuarioId, holeriteId);
        }

        public async Task<HoleriteViewModel> UploadHoleriteAsync(
            int usuarioId,
            int competenciaMes,
            int competenciaAno,
            IFormFile arquivo,
            ClaimsPrincipal principal)
        {
            ValidarAdministrador(principal);
            ValidarCompetencia(competenciaMes, competenciaAno);
            ValidarArquivoPdf(arquivo);

            var usuario = await ValidarUsuarioFuncionarioAsync(usuarioId);
            var arquivoSalvo = await _storage.SalvarAsync(usuario.IdUsuario, $"holerites/{competenciaAno}-{competenciaMes:00}", arquivo);
            var holerite = new Holerite
            {
                IdUsuario = usuario.IdUsuario,
                CompetenciaMes = competenciaMes,
                CompetenciaAno = competenciaAno,
                NomeOriginal = Path.GetFileName(arquivo.FileName),
                NomeBlob = arquivoSalvo.NomeBlob,
                Url = arquivoSalvo.Url,
                ContentType = arquivo.ContentType,
                TamanhoBytes = arquivo.Length,
                CriadoEmUtc = DateTime.UtcNow
            };

            _context.Holerites.Add(holerite);
            await _context.SaveChangesAsync();

            var created = await GetHoleritesQuery()
                .FirstAsync(item => item.IdHolerite == holerite.IdHolerite);

            return ToViewModel(created);
        }

        public async Task<bool> DeleteHoleriteAsync(int usuarioId, int holeriteId, ClaimsPrincipal principal)
        {
            ValidarAdministrador(principal);
            await ValidarUsuarioFuncionarioAsync(usuarioId);

            var holerite = await _context.Holerites
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId && item.IdHolerite == holeriteId);

            if (holerite == null)
            {
                return false;
            }

            await _storage.RemoverAsync(holerite.NomeBlob, holerite.Url);
            _context.Holerites.Remove(holerite);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<ArquivoDownload?> DownloadHoleriteAsync(int usuarioId, int holeriteId)
        {
            var holerite = await _context.Holerites
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId && item.IdHolerite == holeriteId);

            if (holerite == null)
            {
                return null;
            }

            return await _storage.AbrirAsync(holerite.NomeBlob, holerite.Url, holerite.NomeOriginal, holerite.ContentType);
        }

        private IQueryable<Holerite> GetHoleritesQuery()
        {
            return _context.Holerites
                .Include(holerite => holerite.Usuario)
                    .ThenInclude(usuario => usuario!.Perfil)
                .AsNoTracking();
        }

        private async Task<Usuario> ValidarUsuarioFuncionarioAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(item => item.Perfil)
                .FirstOrDefaultAsync(item => item.IdUsuario == usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException("Usuario nao encontrado.");
            }

            if (usuario.IdPerfil == PerfilSistema.AlunoId)
            {
                throw new InvalidOperationException("Holerites podem ser vinculados apenas a professores e administradores.");
            }

            return usuario;
        }

        private static void ValidarArquivoPdf(IFormFile arquivo)
        {
            if (arquivo.Length == 0)
            {
                throw new InvalidOperationException("Arquivo vazio.");
            }

            if (arquivo.Length > PdfMaxBytes)
            {
                throw new InvalidOperationException("Arquivo excede o tamanho maximo permitido.");
            }

            if (!Path.GetExtension(arquivo.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                || !arquivo.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Apenas arquivos PDF podem ser enviados como holerite.");
            }
        }

        private static void ValidarCompetencia(int mes, int ano)
        {
            if (mes is < 1 or > 12)
            {
                throw new InvalidOperationException("Informe uma competencia de mes entre 1 e 12.");
            }

            if (ano is < 2000 or > 2100)
            {
                throw new InvalidOperationException("Informe uma competencia de ano entre 2000 e 2100.");
            }
        }

        private static HoleriteViewModel ToViewModel(Holerite holerite)
        {
            return new HoleriteViewModel
            {
                IdHolerite = holerite.IdHolerite,
                IdUsuario = holerite.IdUsuario,
                NomeUsuario = holerite.Usuario?.Nome ?? string.Empty,
                PerfilUsuario = holerite.Usuario?.Perfil?.DescricaoPerfil ?? PerfilSistema.ObterDescricaoPorId(holerite.Usuario?.IdPerfil ?? 0),
                CompetenciaMes = holerite.CompetenciaMes,
                CompetenciaAno = holerite.CompetenciaAno,
                Competencia = $"{holerite.CompetenciaMes:00}/{holerite.CompetenciaAno}",
                NomeOriginal = holerite.NomeOriginal,
                Url = holerite.Url,
                ContentType = holerite.ContentType,
                TamanhoBytes = holerite.TamanhoBytes,
                CriadoEmUtc = holerite.CriadoEmUtc
            };
        }

        private static void ValidarAdministrador(ClaimsPrincipal principal)
        {
            if (!principal.IsInRole(PerfilSistema.Administrador))
            {
                throw new UnauthorizedAccessException("Apenas administradores podem gerenciar holerites.");
            }
        }

        private static int ValidarFuncionario(ClaimsPrincipal principal)
        {
            if (principal.IsInRole(PerfilSistema.Aluno))
            {
                throw new UnauthorizedAccessException("Alunos nao possuem holerites.");
            }

            if (!principal.IsInRole(PerfilSistema.Professor) && !principal.IsInRole(PerfilSistema.Administrador))
            {
                throw new UnauthorizedAccessException("Usuario nao autorizado a consultar holerites.");
            }

            var usuarioId = GetUsuarioAtualId(principal);
            if (usuarioId <= 0)
            {
                throw new UnauthorizedAccessException("Sessao invalida para consultar holerites.");
            }

            return usuarioId;
        }

        private static int GetUsuarioAtualId(ClaimsPrincipal principal)
        {
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var idUsuario) ? idUsuario : 0;
        }
    }
}
