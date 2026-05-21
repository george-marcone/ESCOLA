using form_API.Services;
using form_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace form_API.Controllers
{
    /// <summary>
    /// Operacoes para consulta e manutencao da diretoria.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DiretoriaController : ControllerBase
    {
        private readonly ILogger<DiretoriaController> _logger;
        private readonly IDiretoriaService _diretoriaService;

        public DiretoriaController(IDiretoriaService diretoriaService, ILogger<DiretoriaController> logger)
        {
            _diretoriaService = diretoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os integrantes da diretoria.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(DiretoriaViewModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _diretoriaService.GetAllAsync(true);
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter diretoria");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Busca um integrante da diretoria pelo identificador.
        /// </summary>
        [HttpGet("{DiretoriaId}")]
        [ProducesResponseType(typeof(DiretoriaViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDiretoriaId(int DiretoriaId)
        {
            try
            {
                var result = await _diretoriaService.GetByIdAsync(DiretoriaId, true);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter diretoria por id");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Cria um novo integrante da diretoria.
        /// </summary>
        [Authorize(Roles = "Administrador,Contribuinte")]
        [HttpPost]
        [ProducesResponseType(typeof(DiretoriaViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(DiretoriaCreateEditViewModel model)
        {
            try
            {
                var created = await _diretoriaService.AddAsync(model);
                return CreatedAtAction(nameof(GetByDiretoriaId), new { DiretoriaId = created.Id }, created);
            }
            catch
            {
                _logger.LogError("Erro ao criar diretoria");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Atualiza um integrante da diretoria.
        /// </summary>
        [Authorize(Roles = "Administrador,Contribuinte")]
        [HttpPut("{DiretoriaId}")]
        [ProducesResponseType(typeof(DiretoriaViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int DiretoriaId, DiretoriaCreateEditViewModel model)
        {
            try
            {
                var updated = await _diretoriaService.UpdateAsync(DiretoriaId, model);
                if (updated == null) return NotFound();
                return CreatedAtAction(nameof(GetByDiretoriaId), new { DiretoriaId = updated.Id }, updated);
            }
            catch
            {
                _logger.LogError("Erro ao atualizar diretoria");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Exclui um integrante da diretoria.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{DiretoriaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int DiretoriaId)
        {
            try
            {
                var deleted = await _diretoriaService.DeleteAsync(DiretoriaId);
                if (!deleted) return NotFound();
                return Ok();
            }
            catch
            {
                _logger.LogError("Erro ao excluir diretoria");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}
