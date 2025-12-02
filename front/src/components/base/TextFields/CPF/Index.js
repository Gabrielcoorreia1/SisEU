import "../CampoTexto.css";
import Icon from "feather-icons-react";

const formatCPF = (value) => {
    value = value.replace(/\D/g, ""); 
    return value.substring(0, 11);
};

const CPF = ({ valor, aoAlterado, iconSize }) => {
  const aoDigitado = (evento) => {
    const sanitizedValue = formatCPF(evento.target.value);
    
    aoAlterado(sanitizedValue); 
  };
  return (
    <div className="campo-texto">
      <label>CPF</label>
      <div className="campo-texto__container">
        <span className="campo-texto_icone_esquerda">
          <Icon className="icon" icon="user" />
        </span>

        <input
          className="campo-texto-cpf"
          value={valor}
          onChange={aoDigitado}
          required={true}
          placeholder="Digite seu CPF"
        />
      </div>
    </div>
  );
};

export default CPF;
