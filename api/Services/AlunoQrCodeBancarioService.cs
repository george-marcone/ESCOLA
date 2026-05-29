using System.Security.Claims;
using System.Text;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.ViewModels;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace ESCOLA_API.Services
{
    public class AlunoQrCodeBancarioService : IAlunoQrCodeBancarioService
    {
        private const string BancoFicticio = "Banco Escola Ficticio S.A.";
        private const string AvisoDadosFicticios = "Dados ficticios para uso escolar. Nao realizar pagamentos reais.";
        private readonly DataContext _context;

        public AlunoQrCodeBancarioService(DataContext context)
        {
            _context = context;
        }

        public async Task<AlunoQrCodeBancarioViewModel?> GerarParaAlunoLogadoAsync(ClaimsPrincipal principal)
        {
            if (!principal.IsInRole(PerfilSistema.Aluno))
            {
                throw new UnauthorizedAccessException("Usuario nao autorizado a gerar QR Code bancario de aluno.");
            }

            var usuarioId = GetUsuarioAtualId(principal);
            if (usuarioId <= 0)
            {
                return null;
            }

            var aluno = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(usuario =>
                    usuario.IdUsuario == usuarioId
                    && usuario.IdPerfil == PerfilSistema.AlunoId);

            if (aluno == null)
            {
                return null;
            }

            var geradoEmUtc = DateTime.UtcNow;
            var dadosBancarios = CriarDadosBancariosFicticios(aluno);
            var conteudoQrCode = CriarConteudoQrCode(aluno, dadosBancarios, geradoEmUtc);
            var qrCodeBytes = GerarQrCodePng(conteudoQrCode);
            var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
            var textoCompartilhamento = CriarTextoCompartilhamento(aluno, conteudoQrCode);

            return new AlunoQrCodeBancarioViewModel
            {
                IdUsuario = aluno.IdUsuario,
                NomeAluno = aluno.Nome,
                EmailAluno = aluno.Email,
                DadosBancarios = dadosBancarios,
                ConteudoQrCode = conteudoQrCode,
                QrCodeBase64 = qrCodeBase64,
                QrCodeDataUrl = $"data:image/png;base64,{qrCodeBase64}",
                TextoCompartilhamento = textoCompartilhamento,
                EmailCompartilhamentoUrl = CriarEmailUrl(textoCompartilhamento),
                WhatsAppCompartilhamentoUrl = CriarWhatsAppUrl(textoCompartilhamento),
                GeradoEmUtc = geradoEmUtc
            };
        }

        private static DadosBancariosFicticiosViewModel CriarDadosBancariosFicticios(Usuario aluno)
        {
            var contaNumero = 100000 + (aluno.IdUsuario * 37 % 900000);
            var agenciaNumero = 1000 + (aluno.IdUsuario * 11 % 9000);
            var documentoParte1 = aluno.IdUsuario % 1000;
            var documentoParte2 = aluno.IdUsuario * 17 % 1000;

            return new DadosBancariosFicticiosViewModel
            {
                Banco = BancoFicticio,
                Agencia = $"{agenciaNumero:0000}-{CalcularDigito(agenciaNumero.ToString())}",
                Conta = $"{contaNumero:000000}-{CalcularDigito(contaNumero.ToString())}",
                TipoConta = "Conta corrente ficticia",
                Favorecido = aluno.Nome,
                DocumentoFicticio = $"000.{documentoParte1:000}.{documentoParte2:000}-00",
                ChavePixFicticia = $"aluno-{aluno.IdUsuario}@pix.ficticio.escola.local",
                Aviso = AvisoDadosFicticios
            };
        }

        private static string CriarConteudoQrCode(Usuario aluno, DadosBancariosFicticiosViewModel dados, DateTime geradoEmUtc)
        {
            var builder = new StringBuilder();
            builder.AppendLine("DADOS BANCARIOS FICTICIOS");
            builder.AppendLine(AvisoDadosFicticios);
            builder.AppendLine($"Aluno: {aluno.Nome}");
            builder.AppendLine($"E-mail: {aluno.Email}");
            builder.AppendLine($"Banco: {dados.Banco}");
            builder.AppendLine($"Agencia: {dados.Agencia}");
            builder.AppendLine($"Conta: {dados.Conta}");
            builder.AppendLine($"Tipo de conta: {dados.TipoConta}");
            builder.AppendLine($"Documento ficticio: {dados.DocumentoFicticio}");
            builder.AppendLine($"Chave Pix ficticia: {dados.ChavePixFicticia}");
            builder.AppendLine($"Gerado em UTC: {geradoEmUtc:O}");

            return builder.ToString().TrimEnd();
        }

        private static byte[] GerarQrCodePng(string conteudo)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);

            return qrCode.GetGraphic(8);
        }

        private static string CriarTextoCompartilhamento(Usuario aluno, string conteudoQrCode)
        {
            return $"{aluno.Nome} compartilhou dados bancarios ficticios para uso escolar.\n\n{conteudoQrCode}";
        }

        private static string CriarEmailUrl(string textoCompartilhamento)
        {
            var subject = Uri.EscapeDataString("Dados bancarios ficticios do aluno");
            var body = Uri.EscapeDataString(textoCompartilhamento);

            return $"mailto:?subject={subject}&body={body}";
        }

        private static string CriarWhatsAppUrl(string textoCompartilhamento)
        {
            return $"https://wa.me/?text={Uri.EscapeDataString(textoCompartilhamento)}";
        }

        private static int CalcularDigito(string valor)
        {
            var soma = 0;
            for (var i = 0; i < valor.Length; i++)
            {
                soma += (valor[i] - '0') * (i + 2);
            }

            return soma % 10;
        }

        private static int GetUsuarioAtualId(ClaimsPrincipal principal)
        {
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var idUsuario) ? idUsuario : 0;
        }
    }
}
