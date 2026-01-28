using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SisEUs.Application;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Checkin.Interfaces;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.Comum.Token;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Infrastructure.Repositorios;
using SisEUs.Infrastructure.Servicos;

namespace SisEUs.Infrastructure
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            AddDatabase(services, config);
            AddRepositories(services);
            AddToken(services, config);
            AddLoggedUser(services);
            
            services.AddApplication();

            return services;
        }

        private static void AddDatabase(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var conexao = config.GetConnectionString("DefaultConnection");

                options.UseMySql(conexao, ServerVersion.AutoDetect(conexao),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICheckinPinRepositorio, CheckinPinRepositorio>();
            services.AddScoped<ICheckinRepositorio, CheckinRepositorio>();
            services.AddScoped<IUoW, UoW>();

            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<IEventoRepositorio, EventoRepositorio>();
            services.AddScoped<IPresencaRepositorio, PresencaRepositorio>();
            services.AddScoped<IApresentacaoRepositorio, ApresentacaoRepositorio>();
            services.AddScoped<IAvaliacaoRepositorio, AvaliacaoRepositorio>();
        }

        private static void AddToken(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>(option =>
                new SisEUs.Infrastructure.Token.JwtTokenGenerator(expirationTimeMinutes, signingKey!));

            services.AddScoped<IAccessTokenValidator>(option =>
                new SisEUs.Infrastructure.Token.JwtTokenValidator(signingKey!));
        }

        private static void AddLoggedUser(IServiceCollection services)
        {
            services.AddScoped<ILoggedUser, LoggedUser.LoggedUser>();
        }
    }
}