using SisEUs.Application.Checkin.Abstraction;
using SisEUs.Application.Checkin.DTOs.Resposta;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.Servicos;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Checkin.Entidades;
using SisEUs.Domain.Checkin.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using EntidadeCheckin = SisEUs.Domain.Checkin.Entidades.Checkin;

namespace SisEUs.Application.Checkin
{
    public class PinService(
        ICheckinPinRepositorio repositorio,
        IUoW uow,
        ICheckinRepositorio checkinRepositorio,
        IUsuarioRepositorio usuarioRepositorio,
        IValidadorDeCoordenadas validadorDeCoordenadas) : IPinService
    {

        public async Task<Resultado<PinResposta>> GerarNovoPinAsync()
        {
            var resultadoPinAnterior = await repositorio.ObterPinAtivoAsync();

            if (resultadoPinAnterior != null)
            {
                resultadoPinAnterior.Invalidar();
                repositorio.Atualizar(resultadoPinAnterior);
            }

            Random random = new();
            string novoPinString = random.Next(100000, 1000000).ToString("D6");

            var novoPin = CheckinPin.Criar(novoPinString);
            repositorio.Adicionar(novoPin);

            await uow.CommitAsync();

            return Resultado<PinResposta>.Ok(
                new PinResposta 
                ( 
                    Pin: novoPinString, 
                    Id: novoPin.Id, 
                    DataGeracao: 
                    novoPin.DataGeracao 
                )
            );
        }

        public async Task<Resultado<PinResposta>> ObterPinAtivoAsync()
        {
            var pinAtivo = await repositorio.ObterPinAtivoAsync();

            if (pinAtivo == null)
            {
                return Resultado<PinResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum PIN ativo encontrado.");
            }

            return Resultado<PinResposta>.Ok(new PinResposta
            (
                Pin: pinAtivo.Pin,
                Id: pinAtivo.Id,
                DataGeracao: pinAtivo.DataGeracao
            ));
        }

        public async Task<Resultado> ValidarApenasPinAsync(string pin)
        {
            var pinAtivo = await repositorio.ObterPinAtivoAsync();

            if (pinAtivo == null || pinAtivo.Pin != pin || !pinAtivo.IsAtivo)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "PIN inválido ou expirado.");
            }

            return Resultado.Ok();
        }

        public async Task<Resultado> ValidarCheckinCompletoAsync(string pin, string latitude, string longitude, int usuarioId)
        {
            // Validar e converter coordenadas usando o serviço centralizado
            var resultadoCoordenadas = validadorDeCoordenadas.TryConverterCoordenadas(latitude, longitude, out double latDouble, out double lonDouble);
            if (!resultadoCoordenadas.Sucesso)
            {
                return resultadoCoordenadas;
            }

            // Validar se está em algum campus
            var resultadoCampus = validadorDeCoordenadas.ValidarLocalizacaoCampus(latDouble, lonDouble);
            if (!resultadoCampus.Sucesso)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Falha no check-in: " + resultadoCampus.Erros.First());
            }

            var pinAtivo = await repositorio.ObterPinAtivoAsync();

            if (pinAtivo == null || pinAtivo.Pin != pin || !pinAtivo.IsAtivo)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Falha no check-in: PIN inválido ou expirado.");
            }

            var checkinAberto = await checkinRepositorio.ObterCheckinAbertoAsync(usuarioId);

            if (checkinAberto != null)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Você já registrou o Check-in. Por favor, faça o Check-out.");
            }

            var novoCheckin = EntidadeCheckin.Criar(usuarioId, pinAtivo.Id, latDouble, lonDouble);

            checkinRepositorio.Adicionar(novoCheckin);
            await uow.CommitAsync();

            return Resultado.Ok();
        }

        public async Task<Resultado> RegistrarCheckOutAsync(string latitude, string longitude, int usuarioId)
        {
            // Validar e converter coordenadas usando o serviço centralizado
            var resultadoCoordenadas = validadorDeCoordenadas.TryConverterCoordenadas(latitude, longitude, out double latDouble, out double lonDouble);
            if (!resultadoCoordenadas.Sucesso)
            {
                return resultadoCoordenadas;
            }

            // Validar se está em algum campus
            var resultadoCampus = validadorDeCoordenadas.ValidarLocalizacaoCampus(latDouble, lonDouble);
            if (!resultadoCampus.Sucesso)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Você não está na área permitida para Check-out.");
            }

            var checkinAberto = await checkinRepositorio.ObterCheckinAbertoAsync(usuarioId);

            if (checkinAberto == null)
            {
                return Resultado.Falha(TipoDeErro.NaoEncontrado, "Nenhum Check-in aberto encontrado para fazer o Check-out.");
            }

            checkinAberto.RegistrarCheckOut();

            checkinRepositorio.Atualizar(checkinAberto);
            await uow.CommitAsync();

            return Resultado.Ok();
        }

        public async Task<Resultado<IEnumerable<RelatorioCheckinResposta>>> ObterDadosRelatorioCheckinAsync()
        {
            var checkins = await checkinRepositorio.ObterTodosCheckinsAsync();
            var pins = await repositorio.ObterTodosPinsAsync();
            var usuarios = await usuarioRepositorio.ObterTodosUsuariosAsync();

            var query = checkins.Select(checkin =>
            {
                var usuario = usuarios.FirstOrDefault(u => u.Id == checkin.UsuarioId);
                var pin = pins.FirstOrDefault(p => p.Id == checkin.PinId);

                if (usuario is null || pin is null) return null;

                string nomeCompletoLimpo = usuario.Nome.Nome;

                string dataCheckOut = checkin.DataHoraCheckOut.HasValue ? checkin.DataHoraCheckOut.Value.ToString("dd/MM/yyyy") : "";
                string horaCheckOut = checkin.DataHoraCheckOut.HasValue ? checkin.DataHoraCheckOut.Value.ToString("HH:mm:ss") : "";

                return new RelatorioCheckinResposta
                (
                    NomeCompleto: nomeCompletoLimpo,
                    Cpf: usuario.Cpf.ToString(),
                    Email: usuario.Email.ToString(),
                    Matricula: usuario.Matricula ?? "",
                    PinUsado: pin.Pin,
                    DataCheckin: checkin.DataHoraCheckIn.ToString("dd/MM/yyyy"),
                    HoraCheckin: checkin.DataHoraCheckIn.ToString("HH:mm:ss"),
                    DataCheckout: dataCheckOut,
                    HoraCheckout: horaCheckOut,
                    Latitude: checkin.Latitude,
                    Longitude: checkin.Longitude
                );
            }).Where(r => r != null).ToList();

            return Resultado<IEnumerable<RelatorioCheckinResposta>>.Ok(query!);
        }
    }
}