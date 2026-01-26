using SisEUs.Application.Checkin.DTOs.Resposta;
using SisEUs.Application.Comum.Resultados;

namespace SisEUs.Application.Checkin.Abstraction
{
    public interface IPinService
    {
        Task<Resultado<PinResposta>> GerarNovoPinAsync();
        Task<Resultado<PinResposta>> ObterPinAtivoAsync();
        Task<Resultado> ValidarApenasPinAsync(string pin);
        Task<Resultado> ValidarCheckinCompletoAsync(string pin, string latitude, string longitude, int usuarioId);
        Task<Resultado> RegistrarCheckOutAsync(string latitude, string longitude, int usuarioId);

        Task<Resultado<IEnumerable<RelatorioCheckinResposta>>> ObterDadosRelatorioCheckinAsync();
    }
}