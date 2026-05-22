using System.Threading.Tasks;
using form_API.Services;
using form_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace form_API.Controllers
{
    /// <summary>
    /// Operacoes para consulta e manutencao de alunos.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AlunoController : ControllerBase
    {
        private readonly ILogger<AlunoController> _logger;
        private readonly IAlunoService _alunoService;

        public AlunoController(IAlunoService alunoService, ILogger<AlunoController> logger)
        {
            _alunoService = alunoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os alunos cadastrados.
        /// </summary>
        /// <returns>Lista de alunos com professor vinculado.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(AlunoViewModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _alunoService.GetAllAsync(true);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter alunos");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Busca um aluno pelo identificador.
        /// </summary>
        /// <param name="AlunoId">Identificador do aluno.</param>
        /// <returns>Aluno encontrado.</returns>
        [HttpGet("{AlunoId}")]
        [ProducesResponseType(typeof(AlunoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAlunoId(int AlunoId)
        {
            try
            {
                var result = await _alunoService.GetByIdAsync(AlunoId, true);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter aluno por id {AlunoId}", AlunoId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Lista alunos vinculados a um professor.
        /// </summary>
        /// <param name="ProfessorId">Identificador do professor.</param>
        /// <returns>Lista de alunos do professor informado.</returns>
        [HttpGet("ByProfessor/{ProfessorId}")]
        [ProducesResponseType(typeof(AlunoViewModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByProfessorId(int ProfessorId)
        {
            try
            {
                var result = await _alunoService.GetByProfessorIdAsync(ProfessorId, true);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter alunos por professor {ProfessorId}", ProfessorId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Cria um novo aluno.
        /// </summary>
        /// <param name="model">Dados do aluno.</param>
        /// <returns>Aluno criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador,Contribuinte")]
        [ProducesResponseType(typeof(AlunoViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(AlunoCreateEditViewModel model)
        {
            try
            {
                var created = await _alunoService.AddAsync(model);
                return CreatedAtAction(nameof(GetByAlunoId), new { AlunoId = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aluno");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Atualiza um aluno existente.
        /// </summary>
        /// <param name="AlunoId">Identificador do aluno.</param>
        /// <param name="model">Dados atualizados do aluno.</param>
        /// <returns>Aluno atualizado.</returns>
        [HttpPut("{AlunoId}")]
        [Authorize(Roles = "Administrador,Contribuinte")]
        [ProducesResponseType(typeof(AlunoViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int AlunoId, AlunoCreateEditViewModel model)
        {
            try
            {
                var updated = await _alunoService.UpdateAsync(AlunoId, model);
                if (updated == null) return NotFound();
                return CreatedAtAction(nameof(GetByAlunoId), new { AlunoId = updated.Id }, updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluno {AlunoId}", AlunoId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        /// <summary>
        /// Exclui um aluno.
        /// </summary>
        /// <param name="AlunoId">Identificador do aluno.</param>
        [HttpDelete("{AlunoId}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int AlunoId)
        {
            try
            {
                var deleted = await _alunoService.DeleteAsync(AlunoId);
                if (!deleted) return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir aluno {AlunoId}", AlunoId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}
