using FluentValidation;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Validators
{
    public class DisciplinaCreateUpdateViewModelValidator : AbstractValidator<DisciplinaCreateUpdateViewModel>
    {
        public DisciplinaCreateUpdateViewModelValidator()
        {
            RuleFor(model => model.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O nome da disciplina e obrigatorio.")
                .MaximumLength(100).WithMessage("O nome da disciplina deve ter no maximo 100 caracteres.");
        }
    }

    public class CadernetaDigitalCreateUpdateViewModelValidator : AbstractValidator<CadernetaDigitalCreateUpdateViewModel>
    {
        public CadernetaDigitalCreateUpdateViewModelValidator()
        {
            RuleFor(model => model.IdAlunoUsuario)
                .GreaterThan(0).WithMessage("Informe um aluno valido.");

            RuleFor(model => model.IdDisciplina)
                .GreaterThan(0).WithMessage("Informe uma disciplina valida.");

            RuleFor(model => model.Notas)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Informe ao menos uma nota.")
                .Must(notas => notas.Length <= 4).WithMessage("Informe no maximo quatro notas.");

            RuleForEach(model => model.Notas)
                .InclusiveBetween(0, 10).WithMessage("Cada nota deve estar entre 0 e 10.");

            RuleFor(model => model.Presencas)
                .GreaterThanOrEqualTo(0).WithMessage("O numero de presencas nao pode ser negativo.");

            RuleFor(model => model.Faltas)
                .GreaterThanOrEqualTo(0).WithMessage("O numero de faltas nao pode ser negativo.");
        }
    }
}
