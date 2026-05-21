using System.Threading.Tasks;
using form_API.Services;
using form_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace form_API.Controllers
{
    /// <summary>
    /// Operacoes para consulta e manutencao de professores.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProfessorController : ControllerBase
    {
        private readonly ILogger<ProfessorController> _logger;
        private readonly IProfessorService _professorService;

        public ProfessorController(IProfessorService professorService, ILogger<ProfessorController> logger)
        {
            _professorService = professorService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os professores cadastrados.
        /// </summary>
        /// <returns>Lista de professores com seus alunos.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProfessorViewModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _professorService.GetAllAsync(true);
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter professores");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Busca um professor pelo identificador.
        /// </summary>
        /// <param name="ProfessorId">Identificador do professor.</param>
        /// <returns>Professor encontrado.</returns>
        [HttpGet("{ProfessorId}")]
        [ProducesResponseType(typeof(ProfessorViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByProfessorId(int ProfessorId)
        {
            try
            {
                var result = await _professorService.GetByIdAsync(ProfessorId, true);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter professor por id");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Cria um novo professor.
        /// </summary>
        /// <param name="model">Dados do professor.</param>
        /// <returns>Professor criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador,Contribuinte")]
        [ProducesResponseType(typeof(ProfessorViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(ProfessorCreateEditViewModel model)
        {
            try
            {
                var created = await _professorService.AddAsync(model);
                return CreatedAtAction(nameof(GetByProfessorId), new { ProfessorId = created.Id }, created);
            }
            catch
            {
                _logger.LogError("Erro ao criar professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Atualiza um professor existente.
        /// </summary>
        /// <param name="ProfessorId">Identificador do professor.</param>
        /// <param name="model">Dados atualizados do professor.</param>
        /// <returns>Professor atualizado.</returns>
        [HttpPut("{ProfessorId}")]
        [Authorize(Roles = "Administrador,Contribuinte")]
        [ProducesResponseType(typeof(ProfessorViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int ProfessorId, ProfessorCreateEditViewModel model)
        {
            try
            {
                var updated = await _professorService.UpdateAsync(ProfessorId, model);
                if (updated == null) return NotFound();
                return CreatedAtAction(nameof(GetByProfessorId), new { ProfessorId = updated.Id }, updated);
            }
            catch
            {
                _logger.LogError("Erro ao atualizar professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Exclui um professor.
        /// </summary>
        /// <param name="ProfessorId">Identificador do professor.</param>
        [HttpDelete("{ProfessorId}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int ProfessorId)
        {
            try
            {
                var deleted = await _professorService.DeleteAsync(ProfessorId);
                if (!deleted) return NotFound();
                return Ok();
            }
            catch
            {
                _logger.LogError("Erro ao excluir professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}
