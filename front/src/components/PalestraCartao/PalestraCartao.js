import "./PalestraCartao.css";
import Botao from "../Botao";

const PalestraCartao = ({
  imagem,
  titulo,
  dataInicio,
  dataFim,
  tituloBotoes,
  localizacao,
  tipo,
  onSelecionar,
  palestraId,
}) => {
  const handleClick = () => {
    if (onSelecionar) {
      onSelecionar(palestraId);
    }
  };

  return (
    <div onClick={handleClick} className="palestra-cartao">
      <div
        className="palestra-cartao-imagem"
        style={{ backgroundImage: `url(${imagem})` }}
      ></div>

      <div className="palestra-cartao-info">
        <h2 className="palestra-nome">{titulo}</h2>

        <h4>{tipo}</h4>

        <div className="palestra-meta">
          <div className="palestra-localizacao">
            {" "}
            {localizacao.sala}, {localizacao.bloco}
          </div>

          <div className="palestra-data">
            Horario: {dataInicio.hora} - {dataFim.hora}
          </div>

          <p>{dataInicio.dataPorExtenso}</p>

          <Botao
            corPrimaria={"#6b5acd"}
            corSecundaria={"#FFF"}
            margin={"24px 0 0 0"}
          >
            {tituloBotoes}
          </Botao>
        </div>
      </div>
    </div>
  );
};

export default PalestraCartao;
