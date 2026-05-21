using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace form_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedFiftyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Professores",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 4, "Carlos" },
                    { 5, "Mariana" },
                    { 6, "Roberto" },
                    { 7, "Fernanda" },
                    { 8, "Ricardo" },
                    { 9, "Patricia" },
                    { 10, "Marcelo" },
                    { 11, "Aline" },
                    { 12, "Eduardo" },
                    { 13, "Juliana" },
                    { 14, "Renato" },
                    { 15, "Camila" },
                    { 16, "Gustavo" },
                    { 17, "Beatriz" },
                    { 18, "Felipe" },
                    { 19, "Larissa" },
                    { 20, "Diego" },
                    { 21, "Tatiane" },
                    { 22, "Rafael" },
                    { 23, "Carolina" },
                    { 24, "Henrique" },
                    { 25, "Vanessa" },
                    { 26, "Leonardo" },
                    { 27, "Priscila" },
                    { 28, "Andre" },
                    { 29, "Simone" },
                    { 30, "Thiago" },
                    { 31, "Monica" },
                    { 32, "Fabio" },
                    { 33, "Daniela" },
                    { 34, "Rodrigo" },
                    { 35, "Leticia" },
                    { 36, "Sergio" },
                    { 37, "Bruna" },
                    { 38, "Caio" },
                    { 39, "Gabriela" },
                    { 40, "Samuel" },
                    { 41, "Isabela" },
                    { 42, "Lucas" },
                    { 43, "Natalia" },
                    { 44, "Paulo" },
                    { 45, "Bianca" },
                    { 46, "Matheus" },
                    { 47, "Renata" },
                    { 48, "Vitor" },
                    { 49, "Amanda" },
                    { 50, "Leandro" }
                });

            migrationBuilder.InsertData(
                table: "Alunos",
                columns: new[] { "Id", "DataNasc", "Nome", "ProfessorId", "Sobrenome" },
                values: new object[,]
                {
                    { 4, "04/04/1983", "Ana", 4, "Silva" },
                    { 5, "05/05/1984", "Bruno", 5, "Santos" },
                    { 6, "06/06/1985", "Carla", 6, "Oliveira" },
                    { 7, "07/07/1986", "Daniel", 7, "Souza" },
                    { 8, "08/08/1987", "Elisa", 8, "Pereira" },
                    { 9, "09/09/1988", "Fabio", 9, "Costa" },
                    { 10, "10/10/1989", "Gabriela", 10, "Rodrigues" },
                    { 11, "11/11/1990", "Hugo", 11, "Almeida" },
                    { 12, "12/12/1991", "Isabela", 12, "Nascimento" },
                    { 13, "13/01/1992", "Jonas", 13, "Lima" },
                    { 14, "14/02/1993", "Karina", 14, "Araujo" },
                    { 15, "15/03/1994", "Luis", 15, "Ferreira" },
                    { 16, "16/04/1995", "Manuela", 16, "Carvalho" },
                    { 17, "17/05/1996", "Nicolas", 17, "Ribeiro" },
                    { 18, "18/06/1997", "Olivia", 18, "Martins" },
                    { 19, "19/07/1998", "Pedro", 19, "Rocha" },
                    { 20, "20/08/1999", "Rafaela", 20, "Barbosa" },
                    { 21, "21/09/2000", "Sofia", 21, "Dias" },
                    { 22, "22/10/2001", "Tiago", 22, "Teixeira" },
                    { 23, "23/11/2002", "Ursula", 23, "Correia" },
                    { 24, "24/12/2003", "Victor", 24, "Mendes" },
                    { 25, "25/01/2004", "Wesley", 25, "Cardoso" },
                    { 26, "26/02/1980", "Yasmin", 26, "Ramos" },
                    { 27, "27/03/1981", "Zoe", 27, "Castro" },
                    { 28, "28/04/1982", "Arthur", 28, "Fernandes" },
                    { 29, "01/05/1983", "Bianca", 29, "Moreira" },
                    { 30, "02/06/1984", "Caio", 30, "Moura" },
                    { 31, "03/07/1985", "Debora", 31, "Batista" },
                    { 32, "04/08/1986", "Enzo", 32, "Freitas" },
                    { 33, "05/09/1987", "Flavia", 33, "Monteiro" },
                    { 34, "06/10/1988", "Guilherme", 34, "Campos" },
                    { 35, "07/11/1989", "Helena", 35, "Vieira" },
                    { 36, "08/12/1990", "Igor", 36, "Pinto" },
                    { 37, "09/01/1991", "Julia", 37, "Cavalcanti" },
                    { 38, "10/02/1992", "Kevin", 38, "Farias" },
                    { 39, "11/03/1993", "Laura", 39, "Cunha" },
                    { 40, "12/04/1994", "Miguel", 40, "Duarte" },
                    { 41, "13/05/1995", "Natalia", 41, "Lopes" },
                    { 42, "14/06/1996", "Otavio", 42, "Reis" },
                    { 43, "15/07/1997", "Pamela", 43, "Pires" },
                    { 44, "16/08/1998", "Rafael", 44, "Tavares" },
                    { 45, "17/09/1999", "Sabrina", 45, "Mello" },
                    { 46, "18/10/2000", "Tales", 46, "Assis" },
                    { 47, "19/11/2001", "Vanessa", 47, "Peixoto" },
                    { 48, "20/12/2002", "William", 48, "Nunes" },
                    { 49, "21/01/2003", "Xenia", 49, "Macedo" },
                    { 50, "22/02/2004", "Yuri", 50, "Brito" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 50);
        }
    }
}
