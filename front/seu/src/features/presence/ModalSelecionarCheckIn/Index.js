// src/features/presence/ModalSelecionarCheckIn/Index.js
import React from 'react';
import styled from 'styled-components';
import Button from '../../../components/base/Button'; 
import Icon from 'feather-icons-react';

import { useGeolocation } from '../hooks/useGeolocation'; 
const ModalOverlay = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1050; 
`;

const ModalContent = styled.div`
  /* Fundo fixo para garantir contraste (o que você queria) */
  background: white; 
  padding: 30px;
  border-radius: 12px;
  width: 90%; 
  max-width: 400px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  position: relative;
  text-align: center;
  /* Cor base do texto no modal (preto, para contraste com o fundo branco) */
  color: #333333; 
`;

const CloseButton = styled(Icon)`
  position: absolute;
  top: 10px;
  right: 10px;
  cursor: pointer;
  /* Ícone de fechar SEMPRE preto */
  color: #000000;
`;

const GpsMessage = ({ isPermitted, error }) => (
    <div style={{ 
        marginTop: '20px', 
        padding: '10px', 
        border: `1px solid ${isPermitted ? 'darkgreen' : 'red'}`, 
        borderRadius: '8px', 
        color: 'white', 
        backgroundColor: isPermitted ? 'darkgreen' : 'red', 
        fontSize: '0.9rem' 
    }}>
        <Icon 
            icon={isPermitted ? 'check-circle' : 'alert-triangle'} 
            size={16} 
            style={{ marginRight: '8px' }} 
        />
        {isPermitted ? 'Localização OK! Prossiga com o Check-in.' : `O GPS precisa estar ligado. ${error ? `(${error})` : ''}`}
    </div>
);
const StyledButton = ({ method, text, onSelectMethod, isQr, isGpsReady }) => (
    <Button
      corPrimaria={isQr ? 'var(--primary-brand)' : 'var(--secondary-brand)'}
      corSecundaria="var(--branco)"
      text={text}
      onClick={() => onSelectMethod(method)}

      disabled={isQr && !isGpsReady}
      
      style={{ 
        width: '100%', 
        margin: '10px 0',
        border: isQr ? `2px solid var(--primary-brand)` : 'none',
      }}
    />
);


const ModalSelecionarCheckIn = ({ isOpen, onClose, onSelectMethod }) => {
  const { coords, isPermitted, error } = useGeolocation(isOpen); 

  if (!isOpen) return null;

  const isGpsReady = isPermitted && coords;

  return (
    <ModalOverlay onClick={onClose}>
      <ModalContent onClick={e => e.stopPropagation()}>
        <CloseButton icon="x" size={24} onClick={onClose} />
        <h3 style={{ marginBottom: '20px', color: '#000000' }}>Escolha o Método de Check-in</h3>
        <StyledButton 
            method="QR_CODE" 
            text="QR Code" 
            onSelectMethod={onSelectMethod} 
            isQr={true} 
            isGpsReady={isGpsReady} 
        />
        
        <p style={{ margin: '15px 0', fontWeight: 'bold' }}>Ou</p>
        <StyledButton 
            method="PIN" 
            text="Código de Check-in" 
            onSelectMethod={onSelectMethod} 
            isQr={false} 
        />
        <GpsMessage isPermitted={isPermitted} error={error} />
        
      </ModalContent>
    </ModalOverlay>
  );
};

export default ModalSelecionarCheckIn;