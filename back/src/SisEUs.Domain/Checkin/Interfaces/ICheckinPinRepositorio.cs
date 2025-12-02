using SisEUs.Domain.Checkin.Entidades;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SisEUs.Domain.Checkin.Interfaces
{
    public interface ICheckinPinRepositorio
    {
        Task<CheckinPin?> ObterPinAtivoAsync();
        void Adicionar(CheckinPin pin);
        void Atualizar(CheckinPin pin);
        Task<IEnumerable<CheckinPin>> ObterTodosPinsAsync();
    }
}