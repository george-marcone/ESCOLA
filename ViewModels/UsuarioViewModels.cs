namespace ESCOLA_API.ViewModels
{
    /// <summary>
    /// Dados para criacao de usuario.
    /// </summary>
    public class UsuarioCreateViewModel
    {
        /// <summary>
        /// Nome do usuario.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email usado para login.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefone de contato.
        /// </summary>
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do usuario: Aluno, Professor ou Administrador.
        /// </summary>
        public string TipoUsuario { get; set; } = string.Empty;
    }

    /// <summary>
    /// Dados para atualizacao de usuario.
    /// </summary>
    public class UsuarioUpdateViewModel
    {
        /// <summary>
        /// Nome do usuario.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email usado para login.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefone de contato.
        /// </summary>
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// URL publica da foto de perfil do usuario.
        /// </summary>
        public string? FotoPerfilUrl { get; set; }

        /// <summary>
        /// Tipo do usuario: Aluno, Professor ou Administrador. Apenas administradores podem alterar.
        /// </summary>
        public string? TipoUsuario { get; set; }
    }

    /// <summary>
    /// Dados publicos de um usuario, sem expor a senha.
    /// </summary>
    public class UsuarioSummaryViewModel
    {
        /// <summary>
        /// Identificador do usuario.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Nome do usuario.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email usado para login.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefone de contato.
        /// </summary>
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// URL publica da foto de perfil do usuario.
        /// </summary>
        public string? FotoPerfilUrl { get; set; }

        /// <summary>
        /// Identificador do perfil de autorizacao.
        /// </summary>
        public int IdPerfil { get; set; }

        /// <summary>
        /// Descricao do perfil de autorizacao.
        /// </summary>
        public string DescricaoPerfil { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do usuario.
        /// </summary>
        public string TipoUsuario { get; set; } = string.Empty;
    }

    public class UsuarioArquivoViewModel
    {
        public int IdUsuarioArquivo { get; set; }
        public int IdUsuario { get; set; }
        public string TipoArquivo { get; set; } = string.Empty;
        public string NomeOriginal { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime CriadoEmUtc { get; set; }
    }

    /// <summary>
    /// Perfil de autorizacao disponivel para usuarios.
    /// </summary>
    public class PerfilViewModel
    {
        /// <summary>
        /// Identificador do perfil.
        /// </summary>
        public int IdPerfil { get; set; }

        /// <summary>
        /// Descricao do perfil.
        /// </summary>
        public string DescricaoPerfil { get; set; } = string.Empty;
    }
}
