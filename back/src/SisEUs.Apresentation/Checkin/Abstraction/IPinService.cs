using SisEUs.Application.Comum.Resultados;
using SisEUs.Apresentation.Checkin.DTOs.Resposta;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace SisEUs.Apresentation.Checkin.Abstractions
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