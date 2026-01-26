using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SisEUs.Infrastructure.Repositorios;

namespace SisEUs.Infrastructure.Migracao
{
    public static class InicializarBancoDeDados
    {
        public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

                dbContext.Database.EnsureCreated();
                await InitBD.SeedAsync(dbContext);

            }
            catch (Exception ex)
            {

                Console.WriteLine("--- ERRO AO INICIAR O BANCO DE DADOS ---");
                Console.WriteLine(ex.Message);
                Console.WriteLine("----------------------------------");
                throw;
            }
        }
    }
}