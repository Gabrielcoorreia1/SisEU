using SisEUs.Domain.Checkin.Entidades;
using System.Threading.Tasks;
using System.Collections.Generic;

// --- CORREÇÃO CRÍTICA: Alias para a Entidade ---
using EntidadeCheckin = SisEUs.Domain.Checkin.Entidades.Checkin;

namespace SisEUs.Domain.Checkin.Interfaces
{
    public interface ICheckinRepositorio
    {
        Task<bool> VerificarCheckinExistenteAsync(int usuarioId, int pinId);
        
        // Usa o Alias corrigido
        void Adicionar(EntidadeCheckin checkin); 
        Task<IEnumerable<EntidadeCheckin>> ObterTodosCheckinsAsync();

        Task<EntidadeCheckin?> ObterCheckinAbertoAsync(int usuarioId); 
        void Atualizar(EntidadeCheckin checkin); 
    }
}