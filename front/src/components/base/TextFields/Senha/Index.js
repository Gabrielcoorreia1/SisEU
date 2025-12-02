// /src/components/base/TextFields/Senha/Index.js

import "../CampoTexto.css";
import { useState } from "react";
import Icon from "feather-icons-react"; 

const Senha = ({ valor, aoAlterado, iconSize }) => {
  const [visivel, setVisivel] = useState(false);

  const alternarVisibilidade = () => {
    setVisivel(!visivel);
  };

  return (
    <div className="campo-texto">
      <label>Senha</label>
      <div className="campo-texto__container">
        <span className="campo-texto_icone_esquerda">
          <Icon className="icon" icon="lock" />
        </span>

        <input
          className="input-senha"
          type={visivel ? "text" : "password"}
          value={valor}
          onChange={(e) => aoAlterado(e.target.value)}
          required={true}
          placeholder="Digite sua Senha"
          autoComplete="current-password" 
        />
        <span className="campo-texto__icone" onClick={alternarVisibilidade}>
          {visivel ? (
            <Icon className="icon" icon="eye-off" style={{ color:'black'}} />
          ) : (
            <Icon className="icon" icon="eye" style={{color:'black'}} />
          )}
        </span>
      </div>
    </div>
  );
};

export default Senha;