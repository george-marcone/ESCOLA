using System.Threading.Tasks;
using ESCOLA_API.Controllers;
using ESCOLA_API.Services;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ESCOLA_API.Tests.Controllers
{
    public class AlunoControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOkResultWithAlunos()
        {
            var service = new Mock<IAlunoService>();
            service.Setup(s => s.GetAllAsync(true)).ReturnsAsync(new[]
            {
                new AlunoViewModel { Id = 1, Nome = "Maria", Sobrenome = "Solano", DataNasc = "25/02/1982", ProfessorId = 1 }
            });

            var logger = new Mock<ILogger<AlunoController>>();
            var controller = new AlunoController(service.Object, logger.Object);

            var actionResult = await controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = Assert.IsType<AlunoViewModel[]>(okResult.Value!);
            Assert.Single(value);
            Assert.Equal("Maria", value[0].Nome);
        }

        [Fact]
        public async Task Post_ReturnsCreatedAtAction()
        {
            var viewModel = new AlunoCreateEditViewModel
            {
                Nome = "Alex",
                Sobrenome = "Alves",
                DataNasc = "22/02/2002",
                ProfessorId = 3
            };

            var expected = new AlunoViewModel
            {
                Id = 3,
                Nome = viewModel.Nome,
                Sobrenome = viewModel.Sobrenome,
                DataNasc = viewModel.DataNasc,
                ProfessorId = viewModel.ProfessorId
            };

            var service = new Mock<IAlunoService>();
            service.Setup(s => s.AddAsync(viewModel)).ReturnsAsync(expected);

            var logger = new Mock<ILogger<AlunoController>>();
            var controller = new AlunoController(service.Object, logger.Object);

            var actionResult = await controller.Post(viewModel);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
            var value = Assert.IsType<AlunoViewModel>(createdResult.Value!);

            Assert.Equal(expected.Id, value.Id);
            Assert.Equal(expected.Nome, value.Nome);
        }
    }
}
