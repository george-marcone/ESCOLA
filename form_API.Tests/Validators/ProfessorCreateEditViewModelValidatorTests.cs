using FluentValidation.TestHelper;
using form_API.Validators;
using form_API.ViewModels;

namespace form_API.Tests.Validators
{
    public class ProfessorCreateEditViewModelValidatorTests
    {
        private readonly ProfessorCreateEditViewModelValidator _validator = new();

        [Fact]
        public void Validate_WhenModelIsValid_ShouldNotHaveValidationErrors()
        {
            var model = new ProfessorCreateEditViewModel
            {
                Nome = "Vinicius"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenNameIsMissing_ShouldHaveValidationError()
        {
            var model = new ProfessorCreateEditViewModel();

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(professor => professor.Nome);
        }
    }
}
