using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ESCOLA_API.Data
{
    public sealed class DataContextDesignTimeFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=ESCOLA_API_DESIGN;Trusted_Connection=True;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new DataContext(options);
        }
    }
}
