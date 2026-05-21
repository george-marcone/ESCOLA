using FluentValidation.TestHelper;
using form_API.Validators;
using form_API.ViewModels;

namespace form_API.Tests.Validators
{
    public class AlunoCreateEditViewModelValidatorTests
    {
        private readonly AlunoCreateEditViewModelValidator _validator = new();

        [Fact]
        public void Validate_WhenModelIsValid_ShouldNotHaveValidationErrors()
        {
            var model = new AlunoCreateEditViewModel
            {
                Nome = "Maria",
                Sobrenome = "Solano",
                DataNasc = "25/02/1982",
                ProfessorId = 1
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenRequiredFieldsAreMissing_ShouldHaveValidationErrors()
        {
            var model = new AlunoCreateEditViewModel();

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(aluno => aluno.Nome);
            result.ShouldHaveValidationErrorFor(aluno => aluno.Sobrenome);
            result.ShouldHaveValidationErrorFor(aluno => aluno.DataNasc);
            result.ShouldHaveValidationErrorFor(aluno => aluno.ProfessorId);
        }

        [Fact]
        public void Validate_WhenBirthDateIsInvalid_ShouldHaveValidationError()
        {
            var model = new AlunoCreateEditViewModel
            {
                Nome = "Maria",
                Sobrenome = "Solano",
                DataNasc = "31/02/1982",
                ProfessorId = 1
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(aluno => aluno.DataNasc);
        }
    }
}
