using FluentValidation;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Validators
{
    public class DiretoriaCreateEditViewModelValidator : AbstractValidator<DiretoriaCreateEditViewModel>
    {
        public DiretoriaCreateEditViewModelValidator()
        {
            RuleFor(diretoria => diretoria.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O nome da diretoria e obrigatorio.")
                .MaximumLength(100).WithMessage("O nome da diretoria deve ter no maximo 100 caracteres.");

            RuleFor(diretoria => diretoria.IdUsuario)
                .GreaterThan(0)
                .When(diretoria => diretoria.IdUsuario.HasValue)
                .WithMessage("Informe um usuario valido.");
        }
    }
}
