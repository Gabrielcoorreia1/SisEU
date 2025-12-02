using SisEUs.Apresentation.Checkin.DTOs.Resposta; 
using SisEUs.Domain.Checkin.Interfaces; 
using SisEUs.Application.Comum.UoW; 
using SisEUs.Domain.Checkin.Entidades; 
using SisEUs.Apresentation.Checkin.Abstractions;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Apresentation.Comum.Utils; 
using System;
using System.Threading.Tasks;
using EntidadeCheckin = SisEUs.Domain.Checkin.Entidades.Checkin;
using System.Linq;
using System.Collections.Generic;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using System.Globalization;
using SisEUs.Apresentation.Checkin.DTOs.Resposta;

namespace SisEUs.Apresentation.Checkin
{
    public class PinService : IPinService
    {
        private readonly ICheckinPinRepositorio _repositorio;
        private readonly IUoW _uow;
        private readonly ICheckinRepositorio _checkinRepositorio; 
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public PinService(
            ICheckinPinRepositorio repositorio, 
            IUoW uow,
            ICheckinRepositorio checkinRepositorio,
            IUsuarioRepositorio usuarioRepositorio) 
        {
            _repositorio = repositorio;
            _uow = uow;
            _checkinRepositorio = checkinRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<Resultado<PinResposta>> GerarNovoPinAsync()
        {
            var resultadoPinAnterior = await _repositorio.ObterPinAtivoAsync();

            if (resultadoPinAnterior != null)
            {
                resultadoPinAnterior.Invalidar();
                _repositorio.Atualizar(resultadoPinAnterior);
            }

            Random random = new Random();
            string novoPinString = random.Next(100000, 1000000).ToString("D6");

            var novoPin = CheckinPin.Criar(novoPinString);
            _repositorio.Adicionar(novoPin);

            await _uow.CommitAsync();

            return Resultado<PinResposta>.Ok(new PinResposta { Pin = novoPinString, Id = novoPin.Id, DataGeracao = novoPin.DataGeracao });
        }

        public async Task<Resultado<PinResposta>> ObterPinAtivoAsync()
        {
            var pinAtivo = await _repositorio.ObterPinAtivoAsync();

            if (pinAtivo == null)
            {
                return Resultado<PinResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum PIN ativo encontrado.");
            }

            return Resultado<PinResposta>.Ok(new PinResposta 
            { 
                Pin = pinAtivo.Pin,
                Id = pinAtivo.Id, 
                DataGeracao = pinAtivo.DataGeracao 
            });
        }

        public async Task<Resultado> ValidarApenasPinAsync(string pin)
        {
            var pinAtivo = await _repositorio.ObterPinAtivoAsync();
            
            if (pinAtivo == null || pinAtivo.Pin != pin || !pinAtivo.IsAtivo)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "PIN inválido ou expirado.");
            }

            return Resultado.Ok();
        }

        public async Task<Resultado> ValidarCheckinCompletoAsync(string pin, string latitude, string longitude, int usuarioId)
        {
            if (!double.TryParse(latitude, CultureInfo.InvariantCulture, out double latDouble) || 
                !double.TryParse(longitude, CultureInfo.InvariantCulture, out double lonDouble))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Formato de coordenadas inválido. Use '.' como separador decimal.");
            }

            if (!GeolocalizacaoUtils.EstaDentroDoRaioDoCampus(latDouble, lonDouble))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Falha no check-in: Você não está na área permitida do Campus.");
            }

            var pinAtivo = await _repositorio.ObterPinAtivoAsync();
            
            if (pinAtivo == null || pinAtivo.Pin != pin || !pinAtivo.IsAtivo)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Falha no check-in: PIN inválido ou expirado.");
            }
            
            var checkinAberto = await _checkinRepositorio.ObterCheckinAbertoAsync(usuarioId);
            
            if (checkinAberto != null)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Você já registrou o Check-in. Por favor, faça o Check-out.");
            }

            var novoCheckin = EntidadeCheckin.Criar(usuarioId, pinAtivo.Id, latDouble, lonDouble); 
            
            _checkinRepositorio.Adicionar(novoCheckin);
            await _uow.CommitAsync();

            return Resultado.Ok(); // Check-in registrado com sucesso!
        }

        public async Task<Resultado> RegistrarCheckOutAsync(string latitude, string longitude, int usuarioId)
        {
            if (!double.TryParse(latitude, CultureInfo.InvariantCulture, out double latDouble) || 
                !double.TryParse(longitude, CultureInfo.InvariantCulture, out double lonDouble))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Formato de coordenadas inválido.");
            }
            if (!GeolocalizacaoUtils.EstaDentroDoRaioDoCampus(latDouble, lonDouble))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Você não está na área permitida para Check-out.");
            }
            
            var checkinAberto = await _checkinRepositorio.ObterCheckinAbertoAsync(usuarioId);
            
            if (checkinAberto == null)
            {
                return Resultado.Falha(TipoDeErro.NaoEncontrado, "Nenhum Check-in aberto encontrado para fazer o Check-out.");
            }
            
            checkinAberto.RegistrarCheckOut(latDouble, lonDouble);
            
            _checkinRepositorio.Atualizar(checkinAberto);
            await _uow.CommitAsync();

            return Resultado.Ok();
        }

        public async Task<Resultado<IEnumerable<RelatorioCheckinResposta>>> ObterDadosRelatorioCheckinAsync()
        {
            var checkins = await _checkinRepositorio.ObterTodosCheckinsAsync();
            var pins = await _repositorio.ObterTodosPinsAsync();
            var usuarios = await _usuarioRepositorio.ObterTodosUsuariosAsync();

            var query = checkins.Select(checkin => 
            {
                var usuario = usuarios.FirstOrDefault(u => u.Id == checkin.UsuarioId);
                var pin = pins.FirstOrDefault(p => p.Id == checkin.PinId);

                if (usuario is null || pin is null) return null; 

                // CORREÇÃO DE NOME: Serializar Nome Completo como string LIMPA
                string nomeCompletoLimpo = usuario.Nome.Nome;


                // 2. Formatação de datas para CSV
                string dataCheckOut = checkin.DataHoraCheckOut.HasValue ? checkin.DataHoraCheckOut.Value.ToString("dd/MM/yyyy") : "";
                string horaCheckOut = checkin.DataHoraCheckOut.HasValue ? checkin.DataHoraCheckOut.Value.ToString("HH:mm:ss") : "";

                return new RelatorioCheckinResposta
                {
                    NomeCompleto = nomeCompletoLimpo, 
                    Cpf = usuario.Cpf.ToString(),
                    Email = usuario.Email.ToString(),
                    Matricula = usuario.Matricula ?? "",
                    PinUsado = pin.Pin,
                    
                    DataCheckin = checkin.DataHoraCheckIn.ToString("dd/MM/yyyy"),
                    HoraCheckin = checkin.DataHoraCheckIn.ToString("HH:mm:ss"),
                    DataCheckout = dataCheckOut,
                    HoraCheckout = horaCheckOut,
                    
                    Latitude = checkin.Latitude,
                    Longitude = checkin.Longitude
                };
            }).Where(r => r != null).ToList();

            return Resultado<IEnumerable<RelatorioCheckinResposta>>.Ok(query!);
        }
    }
}