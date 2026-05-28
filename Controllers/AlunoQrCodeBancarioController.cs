using ESCOLA_API.Services;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESCOLA_API.Controllers
{
    /// <summary>
    /// Operacoes para gerar QR Code bancario ficticio do aluno logado.
    /// </summary>
    [Authorize(Roles = "Aluno")]
    [Route("api/alunos/me/qr-code-bancario")]
    [ApiController]
    [Produces("application/json")]
    public class AlunoQrCodeBancarioController : ControllerBase
    {
        private readonly IAlunoQrCodeBancarioService _qrCodeService;
        private readonly ILogger<AlunoQrCodeBancarioController> _logger;

        public AlunoQrCodeBancarioController(
            IAlunoQrCodeBancarioService qrCodeService,
            ILogger<AlunoQrCodeBancarioController> logger)
        {
            _qrCodeService = qrCodeService;
            _logger = logger;
        }

        /// <summary>
        /// Gera dados bancarios ficticios e um QR Code PNG em base64 para o aluno logado.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(AlunoQrCodeBancarioViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var qrCode = await _qrCodeService.GerarParaAlunoLogadoAsync(User);
                if (qrCode == null) return NotFound();

                return Ok(qrCode);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar QR Code bancario ficticio para aluno logado");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}
