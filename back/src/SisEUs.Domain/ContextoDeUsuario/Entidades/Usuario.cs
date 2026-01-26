using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;
using SisEUs.Domain.ContextoDeUsuario.Excecoes;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeUsuario.Entidades
{
    public class Usuario : Entidade
    {
        private Usuario() { }

        public NomeCompleto Nome { get; private set; } = null!;
        public Cpf Cpf { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Senha Senha { get; private set; } = null!;
        public ETipoUsuario EUserType { get; private set; } = ETipoUsuario.Estudante;
        public Guid UserIdentifier { get; private set; }
        public string? Matricula { get; private set; }

        public static Usuario CriarEstudante(NomeCompleto nome, Cpf cpf, Email email, Senha senha, string? matricula = null)
        {
            var usuario = new Usuario
            {
                Nome = nome,
                Cpf = cpf,
                Email = email,
                Senha = senha,
                EUserType = ETipoUsuario.Estudante,
                UserIdentifier = Guid.NewGuid(),
                Matricula = matricula
            };
            return usuario;
        }
        public static Usuario CriarProfessor(NomeCompleto nome, Cpf cpf, Email email, Senha senha, string? matricula = null)
        {
            return new Usuario
            {
                Nome = nome,
                Cpf = cpf,
                Email = email,
                Senha = senha,
                EUserType = ETipoUsuario.Professor,
                UserIdentifier = Guid.NewGuid(),
                Matricula = matricula
            };
        }
        public static Usuario CriarAdmin(NomeCompleto nome, Cpf cpf, Email email, Senha senha)
        {
            var usuario = new Usuario
            {
                Nome = nome,
                Cpf = cpf,
                Email = email,
                Senha = senha,
                EUserType = ETipoUsuario.Admin,
                UserIdentifier = Guid.NewGuid(),
                Matricula = null
            };
            return usuario;
        }

        public void TornarProfessor()
        {
            if (EUserType == ETipoUsuario.Professor)
                throw new UsuarioJaEProfessorExcecao();

            EUserType = ETipoUsuario.Professor;
        }

        public void TornarAvaliador()
        {
            if (EUserType == ETipoUsuario.Avaliador)
                throw new UsuarioJaEAvaliadorExcecao();

            EUserType = ETipoUsuario.Avaliador;
        }

        public void DefinirMatricula(string matricula)
        {
            if (string.IsNullOrWhiteSpace(matricula))
                throw new ArgumentException("A matrícula não pode ser nula ou vazia.", nameof(matricula));
            Matricula = matricula;
        }
    }
}

