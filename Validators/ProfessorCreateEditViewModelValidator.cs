using FluentValidation;
using form_API.ViewModels;

namespace form_API.Validators
{
    public class ProfessorCreateEditViewModelValidator : AbstractValidator<ProfessorCreateEditViewModel>
    {
        public ProfessorCreateEditViewModelValidator()
        {
            RuleFor(professor => professor.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O nome do professor e obrigatorio.")
                .MaximumLength(100).WithMessage("O nome do professor deve ter no maximo 100 caracteres.");

            RuleFor(professor => professor.IdUsuario)
                .GreaterThan(0)
                .When(professor => professor.IdUsuario.HasValue)
                .WithMessage("Informe um usuario valido.");
        }
    }
}
