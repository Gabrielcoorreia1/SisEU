using Microsoft.Extensions.DependencyInjection;
using SisEUs.Application.Apresentacoes;
using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Authenticacoes;
using SisEUs.Application.Authenticacoes.Abstractions;
using SisEUs.Application.Avaliacoes;
using SisEUs.Application.Avaliacoes.Abstracoes;
using SisEUs.Application.Checkin;
using SisEUs.Application.Checkin.Abstraction;
using SisEUs.Application.Comum.Mapeamento;
using SisEUs.Application.Comum.Servicos;
using SisEUs.Application.Eventos;
using SisEUs.Application.Eventos.Abstracoes;
using SisEUs.Application.Presencas;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Domain.ContextoDeEvento.Servicos;

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
            services.AddScoped<IAvaliacaoServico, AvaliacaoServico>();

            services.AddScoped<GeolocalizacaoValidador>();
            services.AddScoped<IGeolocalizacaoValidador>(sp => sp.GetRequiredService<GeolocalizacaoValidador>());

            services.AddScoped<IValidadorDeCoordenadas, ValidadorDeCoordenadas>();

            services.AddScoped<IMapeadorDeEntidades, MapeadorDeEntidades>();

            return services;
        }
    }
}