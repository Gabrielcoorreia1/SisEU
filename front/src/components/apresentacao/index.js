import "./apresentacao.css";

const Apresentacao = ({ apresentacao }) => {
  return (
    <div className="apresentacao">
      <h3>{apresentacao.titulo}</h3>
      <hr class="custom-hr" />
      <h4>Autor: {apresentacao.nomeAutor}</h4>
      <h4>Professor Orientador: {apresentacao.nomeOrientador}</h4>
    </div>
  );
};

export default Apresentacao;
