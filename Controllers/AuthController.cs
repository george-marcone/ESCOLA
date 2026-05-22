using ESCOLA_API.Services;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESCOLA_API.Controllers
{
    /// <summary>
    /// Operacoes de autenticacao e autorizacao.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Autentica um usuario e retorna um token JWT.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginRequestViewModel model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
            {
                _logger.LogWarning("Tentativa de login recusada para {Email}", model.Email);
                return Unauthorized("Email ou senha invalidos.");
            }

            _logger.LogInformation(
                "Login realizado para usuario {UsuarioId} com perfil {Perfil}",
                result.Usuario.IdUsuario,
                result.Usuario.DescricaoPerfil);
            return Ok(result);
        }

        /// <summary>
        /// Retorna os dados do usuario autenticado.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UsuarioSummaryViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Me()
        {
            var usuario = await _authService.GetUsuarioAtualAsync(User);
            if (usuario == null)
            {
                _logger.LogWarning("Requisicao /me sem usuario autenticado valido.");
                return Unauthorized();
            }

            return Ok(usuario);
        }

        /// <summary>
        /// Verifica se o token JWT e valido.
        /// </summary>
        [Authorize]
        [HttpGet("autorizar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Autorizar()
        {
            return Ok(new { autorizado = true });
        }

        /// <summary>
        /// Verifica se o usuario autenticado possui perfil de administrador.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        [HttpGet("autorizar/admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult AutorizarAdministrador()
        {
            return Ok(new { autorizado = true, perfil = "Administrador" });
        }

        /// <summary>
        /// Altera a senha do usuario autenticado.
        /// </summary>
        [Authorize]
        [HttpPost("alterar-senha")]
        [ProducesResponseType(typeof(UsuarioSummaryViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model)
        {
            try
            {
                var usuario = await _authService.AlterarSenhaAsync(User, model);
                if (usuario == null)
                {
                    _logger.LogWarning("Tentativa de alteracao de senha sem usuario autenticado valido.");
                    return Unauthorized();
                }

                _logger.LogInformation("Senha alterada para usuario {UsuarioId}", usuario.IdUsuario);
                return Ok(usuario);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Alteracao de senha recusada por regra de negocio.");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Redefine a senha do usuario para a senha padrao do sistema.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("esqueci-senha")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EsqueciSenha(EsqueciSenhaViewModel model)
        {
            var senhaRedefinida = await _authService.ResetarSenhaPadraoAsync(model);

            if (senhaRedefinida)
            {
                _logger.LogInformation("Senha redefinida para a senha padrao no fluxo de esqueci senha.");
            }
            else
            {
                _logger.LogWarning("Solicitacao de esqueci senha recebida para email nao cadastrado.");
            }

            return Ok(new
            {
                mensagem = "Se o email informado estiver cadastrado, a senha foi redefinida para a senha padrao."
            });
        }
    }
}
