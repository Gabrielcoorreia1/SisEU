using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.Comum.Token;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Infrastructure.Repositorios;
using SisEUs.Infrastructure.Servicos;
using SisEUs.Infrastructure.Token;

using SisEUs.Domain.Checkin.Interfaces;
using SisEUs.Apresentation.Checkin.Abstractions;
using SisEUs.Apresentation.Checkin;

// --- USINGS PARA SERVIÇOS DE APLICAÇÃO ---
using SisEUs.Apresentation.Authenticacoes; 
using SisEUs.Apresentation.Authenticacoes.Abstractions;
using SisEUs.Application.Eventos.Abstracoes;
using SisEUs.Application.Eventos;
using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Application.Presencas;
// -----------------------------------------

namespace SisEUs.Infrastructure
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            AddMySql(services, config);
            AddServices(services); // Repositórios
            AddToken(services, config);
            AddLoggedUser(services);
            
            // --- INJEÇÃO DA CAMADA APRESENTAÇÃO (Serviços) ---
            services.AddScoped<IAuthService, AuthService>();
            
            // Estas linhas estavam faltando e causavam o erro 500 no ObterEventos:
            services.AddScoped<IEventoServico, EventoServico>();
            services.AddScoped<IApresentacaoServico, ApresentacaoServico>();
            services.AddScoped<IPresencaServico, PresencaServico>();
            // --------------------------------------------------

            return services;
        }

        private static void AddMySql(IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
            {
                var conexao = config.GetConnectionString("conexao");

                options.UseSqlite(conexao,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
        }


        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ICheckinPinRepositorio, CheckinPinRepositorio>();
            services.AddScoped<ICheckinRepositorio, CheckinRepositorio>();
            services.AddScoped<IPinService, PinService>();
            services.AddScoped<IUoW, UoW>();

            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<IEventoRepositorio, EventoRepositorio>();
            services.AddScoped<IPresencaRepositorio, PresencaRepositorio>();
            services.AddScoped<IApresentacaoRepositorio, ApresentacaoRepositorio>();
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