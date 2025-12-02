using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SisEUs.Infrastructure.Repositorios;
using System.IO;

namespace SisEUs.API
{
    // Esta classe é a ponte de Design-Time que o dotnet ef precisa
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Configuração para ler a string de conexão (conexao)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var conexao = configuration.GetConnectionString("conexao");

            // Define o provedor SQLite e o assembly de migrações
            optionsBuilder.UseSqlite(conexao, 
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}