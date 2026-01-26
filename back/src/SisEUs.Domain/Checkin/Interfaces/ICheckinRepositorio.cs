using EntidadeCheckin = SisEUs.Domain.Checkin.Entidades.Checkin;

namespace SisEUs.Domain.Checkin.Interfaces
{
    public interface ICheckinRepositorio
    {
        Task<bool> VerificarCheckinExistenteAsync(int usuarioId, int pinId);
        void Adicionar(EntidadeCheckin checkin);
        Task<IEnumerable<EntidadeCheckin>> ObterTodosCheckinsAsync();
        Task<EntidadeCheckin?> ObterCheckinAbertoAsync(int usuarioId);
        void Atualizar(EntidadeCheckin checkin);
    }
}