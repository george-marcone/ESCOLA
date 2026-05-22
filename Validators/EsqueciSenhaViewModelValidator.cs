using FluentValidation;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Validators
{
    public class EsqueciSenhaViewModelValidator : AbstractValidator<EsqueciSenhaViewModel>
    {
        public EsqueciSenhaViewModelValidator()
        {
            RuleFor(model => model.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O email e obrigatorio.")
                .EmailAddress().WithMessage("Informe um email valido.");
        }
    }
}
