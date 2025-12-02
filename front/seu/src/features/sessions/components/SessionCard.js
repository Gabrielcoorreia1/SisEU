import React from 'react';
import styled from 'styled-components';
import Button from '../../../components/base/Button'; 
import defaultBannerImage from '../../../Imagens/bannerEUs.png';


const CardContainer = styled.div`
  background-color: #f8f8f8;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  width: 100%;
  
  @media (max-width: 768px) {
    margin-bottom: 20px;
  }
`;

const SessionHeader = styled.div`
  height: 100px;
  background-image: url(${props => props.imageUrl}); 
  background-size: cover; 
  background-position: center; 
  color: white; 
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  padding: 10px;
  text-align: center;
  text-shadow: 2px 2px 4px rgba(0,0,0,0.8); 
  font-weight: bold;
`;

const SessionDetails = styled.div`
  padding: 15px;
  & h3 {
    margin-top: 5px;
    font-size: 1.2rem;
    color: #000000 !important;
    
    white-space: normal;
    word-wrap: break-word;
    overflow-wrap: break-word;
  }
  & p {
    margin: 12px 0;
    font-size: 0.9rem;
    color: #555;

    white-space: normal;
    word-wrap: break-word;
    overflow-wrap: break-word;
  }
`;

const ApresentacoesList = styled.ul`
  font-size: 0.85rem;
  color: #333;
  padding-left: 20px;
  margin-top: 10px;
  margin-bottom: 15px;
`;

const SessionCard = ({ session }) => { 

  const local = session.local || {};
  
  const dataInicio = session.dataInicio || {};
  const dataFim = session.dataFim || {};

  const apresentacoes = session.apresentacoes || [];

  const getTipoEventoText = (type) => {
    switch (type) {
      case 0: return "Nenhum";
      case 1: return "Pitch";
      case 2: return "Oral";
      case 3: return "Banner";
      default: return "Desconhecido";
    }
  };

  const tituloSessao = session.titulo || session.Titulo;

  return (
    <CardContainer>
      <SessionHeader imageUrl={defaultBannerImage}> 
      </SessionHeader>
      <SessionDetails>
        <h3>{tituloSessao}</h3>
        <p>Tipo de Evento: {getTipoEventoText(session.eventType)}</p>
        <p>Local: {local.sala ? `Sala ${local.sala}` : ''} {local.bloco ? `- Bloco ${local.bloco}` : ''} {local.departamento ? `(${local.departamento})` : ''}</p>
        <p>Horário: {dataInicio.hora} - {dataFim.hora}</p>
        <p>Data: {dataInicio.dataPorExtenso}</p>

        <Button 
          text="Saiba mais"
          corPrimaria="#3B5998"
          corSecundaria="#FFF"
          onClick={() => console.log('Detalhes da Sessão:', session.id)}
        />
      </SessionDetails>
    </CardContainer>
  );
};

export default SessionCard;