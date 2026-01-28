using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Eventos.Mappers;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;

namespace SisEUs.Application.Apresentacoes.Mappers
{
    public static class ApresentacaoMapper
    {
        public static async Task<ApresentacaoResposta> ToResponseDtoAsync(
            this Apresentacao apresentacao,
            IEventoRepositorio eventoRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            CancellationToken cancellationToken)
        {
            var evento = await eventoRepositorio.ObterEventoPorIdAsync(apresentacao.EventoId, cancellationToken);

            var eventoDto = (evento != null)
                ? await evento.ToResponseDtoAsync(usuarioRepositorio, cancellationToken)
                : null;

            // Buscar autor e orientador
            var autor = await usuarioRepositorio.ObterPorIdAsync(apresentacao.AutorId, cancellationToken);
            var orientador = await usuarioRepositorio.ObterPorIdAsync(apresentacao.OrientadorId, cancellationToken);

            var autorDto = autor != null 
                ? new UsuarioResposta(autor.Id, $"{autor.Nome.Nome} {autor.Nome.Sobrenome}", autor.Cpf.Valor, autor.Email.Valor)
                : new UsuarioResposta(0, "Autor não encontrado", "Sem cpf", "Sem email");

            var orientadorDto = orientador != null 
                ? new UsuarioResposta(orientador.Id, $"{orientador.Nome.Nome} {orientador.Nome.Sobrenome}", orientador.Cpf.Valor, orientador.Email.Valor)
                : new UsuarioResposta(0, "Orientador não encontrado", "Sem cpf", "Sem email");

            return new ApresentacaoResposta
            (
                apresentacao.Id,
                eventoDto!,
                apresentacao.Titulo.Valor,
                autorDto,
                orientadorDto
            );
        }
    }
}
