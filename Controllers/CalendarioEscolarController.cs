using ESCOLA_API.Services;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESCOLA_API.Controllers
{
    /// <summary>
    /// Painel de calendario escolar anual com feriados nacionais brasileiros.
    /// </summary>
    [Authorize]
    [Route("api/calendario-escolar")]
    [ApiController]
    [Produces("application/json")]
    public class CalendarioEscolarController : ControllerBase
    {
        private readonly ICalendarioEscolarService _service;
        private readonly ILogger<CalendarioEscolarController> _logger;

        public CalendarioEscolarController(
            ICalendarioEscolarService service,
            ILogger<CalendarioEscolarController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Retorna o calendario escolar anual com todos os meses, destacando feriados nacionais brasileiros e o mes selecionado.
        /// </summary>
        /// <param name="ano">Ano desejado. Quando omitido, usa o ano atual.</param>
        /// <param name="mesSelecionado">Mes que o front deve destacar inicialmente. Quando omitido, usa o mes atual no ano vigente.</param>
        [HttpGet]
        [ProducesResponseType(typeof(CalendarioEscolarAnoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] int? ano, [FromQuery] int? mesSelecionado)
        {
            try
            {
                return Ok(await _service.GetCalendarioAnualAsync(ano, mesSelecionado));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar calendario escolar");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Cadastra um evento geral no calendario escolar e notifica os perfis destinatarios.
        /// </summary>
        /// <param name="model">Dados do evento geral do calendario escolar.</param>
        [Authorize(Roles = "Administrador")]
        [HttpPost("eventos")]
        [ProducesResponseType(typeof(CalendarioEscolarEventoViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEvento(CalendarioEscolarEventoCreateViewModel model)
        {
            try
            {
                var created = await _service.AddEventoAsync(model, User);
                return CreatedAtAction(nameof(Get), new { ano = created.Data.Year, mesSelecionado = created.Data.Month }, created);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar evento no calendario escolar");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}
