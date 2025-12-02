import "./SecaoExplorarEventos.css";
import PalestraCartao from "../../PalestraCartao/PalestraCartao";
import { useEffect, useState } from "react";
import axios from "axios";

const SecaoExplorarEventos = ({
  onSelecionarPalestra,
  tituloBotoes,
  children,
  reloadCount,
}) => {
  const [palestras, setPalestras] = useState([]);

  useEffect(() => {
    const carregarPalestras = async () => {
      try {
        const response = await axios.get(
          "https://localhost:7102/api/Eventos/ObterEventos"
        );
        setPalestras(response.data);
      } catch (erro) {
        console.error("Erro ao buscar palestras:", erro);
      }
    };

    carregarPalestras();
  }, [reloadCount]);

  return (
    <div className="explorar-eventos-body">
      {children}

      <div className="explorar-eventos-palestras">
        {palestras.map((palestra, Index) => (
          <PalestraCartao
            key={Index}
            imagem={palestra.imgUrl}
            titulo={palestra.titulo}
            tipo={palestra.eTipoEvento}
            dataInicio={palestra.dataInicio}
            dataFim={palestra.dataFim}
            horario={palestra.horario}
            localizacao={palestra.local}
            palestraId={palestra.id}
            onSelecionar={onSelecionarPalestra}
            tituloBotoes={tituloBotoes}
          />
        ))}
      </div>
    </div>
  );
};

export default SecaoExplorarEventos;
