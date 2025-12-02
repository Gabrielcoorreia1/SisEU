using Microsoft.Extensions.DependencyInjection;
using SisEUs.Apresentation.Authenticacoes;
using SisEUs.Apresentation.Authenticacoes.Abstractions;
using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes;
using SisEUs.Application.Eventos.Abstracoes;
using SisEUs.Application.Eventos;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Application.Presencas;
using SisEUs.Apresentation.Checkin;
using SisEUs.Apresentation.Checkin.Abstractions;

namespace SisEUs.Application
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IApresentacaoServico, ApresentacaoServico>();
            services.AddScoped<IEventoServico, EventoServico>();
            services.AddScoped<IPresencaServico, PresencaServico>();
            services.AddScoped<IPinService, PinService>();
            
            return services;
        }
    }
}