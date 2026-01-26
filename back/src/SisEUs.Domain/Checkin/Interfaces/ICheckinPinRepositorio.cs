using SisEUs.Domain.Checkin.Entidades;

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