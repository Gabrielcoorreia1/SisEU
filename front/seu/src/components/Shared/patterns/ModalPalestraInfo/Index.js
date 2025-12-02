import Icon from "feather-icons-react";
import "./ModalPalestraInfo.css";
import Apresentacao from "../../apresentacao";

const ModalPalestraInfo = ({
  isOpen,
  onClose,
  palestra,
  organizadores,
  avaliadores,
  apresentacoes,
}) => {
  if (!isOpen || !palestra) return null;

  return (
    <div className={`modal-overlay ${isOpen ? "show" : ""}`} onClick={onClose}>
      <div
        className="modal-palestrainfo-content"
        onClick={(e) => e.stopPropagation()}
      >
        <div
          className="modal-palestrainfo-image"
          style={{ backgroundImage: `url(${palestra.imgUrl})` }}
        >
          <Icon onClick={onClose} icon="x" size={"40px"} />
        </div>
        <div className="modal-palestrainfo-scrollable">
          <div className="modal-palestra-info">
            <h1>{palestra.titulo}</h1>

            <h4>{palestra.nomeCampus}</h4>

            <div>
              <h4>
                {palestra.local.sala} - {palestra.local.bloco} -{" "}
                {palestra.local.campus}
              </h4>

              <div className="palestra-data">
                {palestra.dataInicio.dataPorExtenso} |{" "}
                {palestra.dataInicio.hora} - {palestra.dataFim.hora}
              </div>
            </div>
          </div>

          <hr class="custom-hr" />

          <div>
            <h2>Organizadores:</h2>
            {organizadores.map((organizador, index) => {
              return <h4 key={index}>{organizador.nomeCompleto}</h4>;
            })}
          </div>

          <hr class="custom-hr" />

          <div>
            <h2>Professores avaliadores:</h2>
            {avaliadores.map((avaliador, index) => {
              return <h4 key={index}>{avaliador}</h4>;
            })}
          </div>

          <hr class="custom-hr" />

          <div>
            <h2>Apresentações:</h2>
            <div className="apresentacoes-area">
              {apresentacoes.map((apresentacao, index) => (
                <Apresentacao key={index} apresentacao={apresentacao} />
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ModalPalestraInfo;
