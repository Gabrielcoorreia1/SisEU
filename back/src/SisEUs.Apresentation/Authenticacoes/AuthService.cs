using SisEUs.Application.Authenticacoes.Abstractions;
using SisEUs.Application.Authenticacoes.DTOs.Resposta;
using SisEUs.Application.Authenticacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Token;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SisEUs.Application.Authenticacoes
{
    public class AuthService(
        IUsuarioRepositorio usuarioRepositorio,
        IAccessTokenGenerator tokenGenerator,
        IUoW uow) : IAuthService
    {
        public async Task<Resultado<LoginResposta>> LogarAsync(LogarSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                var cpf = Cpf.Criar(request.Cpf);

                var usuario = await usuarioRepositorio.ObterPorCpfAsync(cpf, cancellationToken);

                if (usuario is null)
                {
                    return Resultado<LoginResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                var senhaValida = BCryptNet.Verify(request.Senha, usuario.Senha.Valor);

                if (!senhaValida)
                {
                    return Resultado<LoginResposta>.Falha(TipoDeErro.Validacao, "Senha incorreta.");
                }

                var token = tokenGenerator.Generate(usuario);

                return Resultado<LoginResposta>.Ok(new LoginResposta
                (
                    token,
                    usuario.EUserType,
                    usuario.Nome.ToString(),
                    usuario.Cpf.ToString(),
                    usuario.Id
                ));
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<LoginResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                return Resultado<LoginResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<UsuarioResposta>> RegistrarAsync(RegistrarSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                var cpf = Cpf.Criar(request.Cpf);
                var email = Email.Criar(request.Email);

                if (await usuarioRepositorio.CpfJaExisteAsync(cpf, cancellationToken))
                {
                    return Resultado<UsuarioResposta>.Falha(TipoDeErro.Conflito, "CPF já cadastrado no sistema.");
                }

                if (await usuarioRepositorio.EmailJaExisteAsync(email, cancellationToken))
                {
                    return Resultado<UsuarioResposta>.Falha(TipoDeErro.Conflito, "Email já cadastrado no sistema.");
                }

                var nome = NomeCompleto.Criar(request.PrimeiroNome, request.Sobrenome);
                var senhaHash = BCryptNet.HashPassword(request.Senha);
                var senha = Senha.Criar(senhaHash);

                var usuario = Usuario.CriarEstudante(nome, cpf, email, senha);

                await usuarioRepositorio.AdicionarAsync(usuario, cancellationToken);
                await uow.CommitAsync(cancellationToken);

                return Resultado<UsuarioResposta>.Ok(new UsuarioResposta(
                    Id: usuario.Id,
                    NomeCompleto: $"{usuario.Nome.Nome} {usuario.Nome.Sobrenome}",
                    Cpf: usuario.Cpf.Valor,
                    Email: usuario.Email.Valor
                ));
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<UsuarioResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                return Resultado<UsuarioResposta>.Falha(TipoDeErro.Inesperado, $"Erro inesperado ao registrar usuário: {ex.Message}");
            }
        }

        public async Task<Resultado<BuscarUsuariosResposta>> BuscarPorNomeProfessorAsync(string nome, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return Resultado<BuscarUsuariosResposta>.Falha(TipoDeErro.Validacao, "Nome não pode ser vazio.");
            }

            var usuarios = await usuarioRepositorio.BuscarPorNomeProfessorAsync(nome, cancellationToken);

            if (usuarios == null || !usuarios.Any())
            {
                return Resultado<BuscarUsuariosResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum professor encontrado com esse nome.");
            }

            var usuariosResposta = usuarios.Select(u => new UsuarioResposta(
                Id: u.Id,
                NomeCompleto: $"{u.Nome.Nome} {u.Nome.Sobrenome}",
                Cpf: u.Cpf.Valor,
                Email: u.Email.Valor
            )).ToList();

            return Resultado<BuscarUsuariosResposta>.Ok(new BuscarUsuariosResposta(usuariosResposta));
        }

        public async Task<Resultado<UsuarioResposta>> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return Resultado<UsuarioResposta>.Falha(TipoDeErro.Validacao, "ID inválido.");
            }

            var usuario = await usuarioRepositorio.ObterPorIdAsync(id, cancellationToken);

            if (usuario is null)
            {
                return Resultado<UsuarioResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
            }

            return Resultado<UsuarioResposta>.Ok(new UsuarioResposta(
                Id: usuario.Id,
                NomeCompleto: $"{usuario.Nome.Nome} {usuario.Nome.Sobrenome}",
                Cpf: usuario.Cpf.Valor,
                Email: usuario.Email.Valor
            ));
        }

        public async Task<Resultado<UsuarioResposta>> TornarProfessorAsync(int usuarioId, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await usuarioRepositorio.ObterPorIdAsync(usuarioId, cancellationToken);

                if (usuario is null)
                    return Resultado<UsuarioResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");

                usuario.TornarProfessor();
                
                usuarioRepositorio.Atualizar(usuario);
                await uow.CommitAsync(cancellationToken);

                return Resultado<UsuarioResposta>.Ok(new UsuarioResposta(
                    Id: usuario.Id,
                    NomeCompleto: $"{usuario.Nome.Nome} {usuario.Nome.Sobrenome}",
                    Cpf: usuario.Cpf.Valor,
                    Email: usuario.Email.Valor
                ));
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<UsuarioResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<UsuarioResposta>> TornarAvaliadorAsync(int usuarioId, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await usuarioRepositorio.ObterPorIdAsync(usuarioId, cancellationToken);

                if (usuario is null)
                    return Resultado<UsuarioResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");

                usuario.TornarAvaliador();
                
                usuarioRepositorio.Atualizar(usuario);
                await uow.CommitAsync(cancellationToken);

                return Resultado<UsuarioResposta>.Ok(new UsuarioResposta(
                    Id: usuario.Id,
                    NomeCompleto: $"{usuario.Nome.Nome} {usuario.Nome.Sobrenome}",
                    Cpf: usuario.Cpf.Valor,
                    Email: usuario.Email.Valor
                ));
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<UsuarioResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }
    }
}