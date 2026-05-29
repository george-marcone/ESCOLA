using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Services
{
    public interface ICadernetaDigitalEventPublisher
    {
        Task PublishNotasPublicadasAsync(
            CadernetaDigitalViewModel caderneta,
            string operacao,
            CancellationToken cancellationToken = default);
    }

    public sealed class NullCadernetaDigitalEventPublisher : ICadernetaDigitalEventPublisher
    {
        public static readonly NullCadernetaDigitalEventPublisher Instance = new();

        private NullCadernetaDigitalEventPublisher()
        {
        }

        public Task PublishNotasPublicadasAsync(
            CadernetaDigitalViewModel caderneta,
            string operacao,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
