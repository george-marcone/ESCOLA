using FluentValidation;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Validators
{
    public class LoginRequestViewModelValidator : AbstractValidator<LoginRequestViewModel>
    {
        public LoginRequestViewModelValidator()
        {
            RuleFor(login => login.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O email e obrigatorio.")
                .EmailAddress().WithMessage("Informe um email valido.");

            RuleFor(login => login.Senha)
                .NotEmpty().WithMessage("A senha e obrigatoria.");
        }
    }
}
