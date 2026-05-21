using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using form_API.Models;
using Microsoft.EntityFrameworkCore;

namespace form_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Professor> Professores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Professor>().HasData(CreateProfessores());
            builder.Entity<Aluno>().HasData(CreateAlunos());
        }

        private static IEnumerable<Professor> CreateProfessores()
        {
            var nomes = new[]
            {
                "Vinicius", "Paula", "Suzana", "Carlos", "Mariana", "Roberto", "Fernanda", "Ricardo",
                "Patricia", "Marcelo", "Aline", "Eduardo", "Juliana", "Renato", "Camila", "Gustavo",
                "Beatriz", "Felipe", "Larissa", "Diego", "Tatiane", "Rafael", "Carolina", "Henrique",
                "Vanessa", "Leonardo", "Priscila", "Andre", "Simone", "Thiago", "Monica", "Fabio",
                "Daniela", "Rodrigo", "Leticia", "Sergio", "Bruna", "Caio", "Gabriela", "Samuel",
                "Isabela", "Lucas", "Natalia", "Paulo", "Bianca", "Matheus", "Renata", "Vitor",
                "Amanda", "Leandro"
            };

            return nomes.Select((nome, index) => new Professor
            {
                Id = index + 1,
                Nome = nome
            });
        }

        private static IEnumerable<Aluno> CreateAlunos()
        {
            var nomes = new[]
            {
                "Maria", "Joao", "Alex", "Ana", "Bruno", "Carla", "Daniel", "Elisa", "Fabio",
                "Gabriela", "Hugo", "Isabela", "Jonas", "Karina", "Luis", "Manuela", "Nicolas",
                "Olivia", "Pedro", "Rafaela", "Sofia", "Tiago", "Ursula", "Victor", "Wesley",
                "Yasmin", "Zoe", "Arthur", "Bianca", "Caio", "Debora", "Enzo", "Flavia",
                "Guilherme", "Helena", "Igor", "Julia", "Kevin", "Laura", "Miguel", "Natalia",
                "Otavio", "Pamela", "Rafael", "Sabrina", "Tales", "Vanessa", "William", "Xenia",
                "Yuri"
            };

            var sobrenomes = new[]
            {
                "Solano", "Gomes", "Alves", "Silva", "Santos", "Oliveira", "Souza", "Pereira",
                "Costa", "Rodrigues", "Almeida", "Nascimento", "Lima", "Araujo", "Ferreira",
                "Carvalho", "Ribeiro", "Martins", "Rocha", "Barbosa", "Dias", "Teixeira",
                "Correia", "Mendes", "Cardoso", "Ramos", "Castro", "Fernandes", "Moreira",
                "Moura", "Batista", "Freitas", "Monteiro", "Campos", "Vieira", "Pinto",
                "Cavalcanti", "Farias", "Cunha", "Duarte", "Lopes", "Reis", "Pires", "Tavares",
                "Mello", "Assis", "Peixoto", "Nunes", "Macedo", "Brito"
            };

            return nomes.Select((nome, index) => new Aluno
            {
                Id = index + 1,
                Nome = nome,
                Sobrenome = sobrenomes[index],
                DataNasc = index switch
                {
                    0 => "25/02/1982",
                    1 => "25/01/2000",
                    2 => "22/02/2002",
                    _ => $"{(index % 28) + 1:00}/{(index % 12) + 1:00}/{1980 + (index % 25)}"
                },
                ProfessorId = index + 1
            });
        }
    }
}
