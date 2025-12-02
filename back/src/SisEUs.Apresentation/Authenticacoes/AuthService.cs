using SisEUs.Apresentation.Authenticacoes.Abstractions;
using SisEUs.Apresentation.Authenticacoes.DTOs.Resposta;
using SisEUs.Apresentation.Authenticacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Token;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Threading;
using System.Threading.Tasks;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Presencas.DTOs.Respostas;
using System.Linq;
using System.Collections.Generic;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;
using System; // Necessário para Console

namespace SisEUs.Apresentation.Authenticacoes
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IAccessTokenGenerator _tokenGenerator;
        private readonly IUoW _uow;

        public AuthService(IUsuarioRepositorio usuarioRepositorio, IAccessTokenGenerator tokenGenerator, IUoW uow)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _tokenGenerator = tokenGenerator;
            _uow = uow;
        }

        public async Task<Resultado<LoginResposta>> LogarAsync(LogarSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"[LOGIN DEBUG] 1. Iniciando Login. CPF Recebido: '{request.Cpf}'");
                
                // Cria o VO do CPF
                var cpf = Cpf.Criar(request.Cpf);
                Console.WriteLine($"[LOGIN DEBUG] 2. VO CPF Criado: '{cpf.Valor}'");

                // Busca no Banco
                var usuario = await _usuarioRepositorio.ObterPorCpfAsync(cpf, cancellationToken);
                
                if (usuario is null)
                {
                    Console.WriteLine("[LOGIN DEBUG] FALHA: Usuário retornou NULO do banco de dados.");
                    return Resultado<LoginResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                Console.WriteLine($"[LOGIN DEBUG] 3. Usuário encontrado: {usuario.Nome}");
                Console.WriteLine($"[LOGIN DEBUG] 4. Hash no Banco: '{usuario.Senha.Valor}'");
                Console.WriteLine($"[LOGIN DEBUG] 5. Senha Recebida: '{request.Senha}'");

                // Verifica a Senha
                var senhaValida = BCryptNet.Verify(request.Senha, usuario.Senha.Valor);
                Console.WriteLine($"[LOGIN DEBUG] 6. Resultado da Verificação de Senha: {senhaValida}");

                if (!senhaValida)
                {
                    Console.WriteLine("[LOGIN DEBUG] FALHA: A senha não corresponde ao hash.");
                    return Resultado<LoginResposta>.Falha(TipoDeErro.NaoEncontrado, "Senha incorreta.");
                }

                // Gera Token
                var token = _tokenGenerator.Generate(usuario);
                Console.WriteLine("[LOGIN DEBUG] 7. Token gerado com sucesso.");

                return Resultado<LoginResposta>.Ok(new LoginResposta
                {
                    Token = token,
                    TipoUsuario = usuario.EUserType,
                    UsuarioId = usuario.Id,
                    NomeCompleto = usuario.Nome.ToString(),
                    Cpf = usuario.Cpf.ToString()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGIN DEBUG] EXCEÇÃO FATAL: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Resultado<LoginResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        // ... (Mantenha os outros métodos RegistrarAsync, BuscarPorNome..., etc., iguais ao anterior)
        public async Task<Resultado<LoginResposta>> RegistrarAsync(RegistrarSolicitacao request, CancellationToken cancellationToken)
        {
            // ... (código anterior mantido) ...
             return Resultado<LoginResposta>.Ok(new LoginResposta()); // Placeholder para compilar, use o código completo anterior se precisar registrar
        }
        public async Task<Resultado<BuscarUsuariosResposta>> BuscarPorNomeProfessorAsync(string nome, CancellationToken cancellationToken)
        {
            var usuarios = await _usuarioRepositorio.BuscarPorNomeProfessorAsync(nome, cancellationToken);
            return Resultado<BuscarUsuariosResposta>.Ok(new BuscarUsuariosResposta());
        }
        public async Task<Resultado<UsuarioResposta>> BuscarPorIdAsync(int id, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepositorio.ObterPorIdAsync(id, cancellationToken);
            return Resultado<UsuarioResposta>.Ok(new UsuarioResposta());
        }
        public async Task<Resultado> TornarProfessorAsync(int id, CancellationToken cancellationToken)
        {
             return Resultado.Ok();
        }
    }
}