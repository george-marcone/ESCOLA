using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using form_API.Data;
using form_API.Services;
using form_API.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AlunoCreateEditViewModelValidator>();
builder.Services.AddCors();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<IProfessorService, ProfessorService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Form API",
        Version = "v1",
        Description = "API para gerenciamento de alunos e professores."
    });

    var xmlFileName = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

    if (File.Exists(xmlFilePath))
    {
        options.IncludeXmlComments(xmlFilePath);
    }
});
builder.Services.AddDbContext<DataContext>(
    x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Apply pending migrations at startup (ensures database schema exists)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();
app.MapControllers();
app.Run();
