using SisEUs.Domain.Comum.Excecoes;
using System;

namespace SisEUs.Domain.ContextoDeUsuario.Excecoes
{
    public class UsuarioJaEProfessorExcecao : ExcecaoDeDominio
    {
        public UsuarioJaEProfessorExcecao() : base("O usuário já possui o cargo de Professor.")
        {
        }
    }
}