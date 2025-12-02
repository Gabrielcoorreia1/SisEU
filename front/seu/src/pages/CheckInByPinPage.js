// src/pages/CheckInByPinPage.js
import React, { useState } from 'react';
import styled from 'styled-components';
import Layout from '../components/Shared/Layout';
import Button from '../components/base/Button';
import CodigoCheckIn from '../components/base/TextFields/CodigoCheckIn/index'; 
import Icon from 'feather-icons-react';
import { useNavigate } from 'react-router-dom';
import Alert from '../components/Shared/patterns/Alert/Index';  

import { useCheckInFlow } from '../features/presence/hooks/useCheckInFlow'; 

const PinInputWrapper = styled.div`
  color: var(--theme-text); 
  
  input {
    color: var(--theme-text) !important; 
    text-align: center;
    background-color: transparent !important; 
  }
`;

const PageContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - var(--height-xl));
  padding: var(--padding-m);
`;

const ContentBox = styled.div`
  background: var(--theme-card-bg);
  color: var(--theme-text);
  padding: 40px 30px;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  width: 90%;
  max-width: 450px;
  text-align: center;
  position: relative;
`;

const CloseIcon = styled(Icon)`
    position: absolute;
    top: 15px;
    right: 15px;
    cursor: pointer;
    color: var(--theme-text);
    z-index: 10;
`;

const CheckInByPinPage = () => {
    const navigate = useNavigate();
    const [pinInput, setPinInput] = useState(''); 
    const { loading, error, validarPin, clearError } = useCheckInFlow(); 
    
    const handlePinChange = (value) => {
        const numericValue = value.replace(/[^0-9]/g, ''); 
        
        if (error) clearError();
        
        setPinInput(numericValue);
    };

    const handleConfirm = () => {
        validarPin(pinInput); 
    };

    const handleClose = () => {
        if (!loading) {
            navigate('/dashboard');
        }
    };

    return (
        <Layout>
            <PageContainer>
                <ContentBox>
                    <CloseIcon icon="x" size={24} onClick={handleClose} />
                    
                    <h2 style={{ color: 'var(--theme-text)', marginBottom: '30px' }}>
                        Digite o código de Check-in:
                    </h2>
                    
                    <PinInputWrapper>
                        <CodigoCheckIn 
                            setCodigo={handlePinChange} 
                            length={6}
                            valor={pinInput} 
                        />
                    </PinInputWrapper>

                    <Button 
                        corPrimaria={"var(--primary-brand)"} 
                        corSecundaria={"var(--branco)"}
                        text={loading ? "Verificando..." : "Confirmar"}
                        onClick={handleConfirm}
                        disabled={loading || pinInput.length !== 6} 
                        style={{ marginTop: '40px', width: '100%' }}
                    />

                    {error && <Alert message={error} show={!!error} animationKey={1} />}
                    
                </ContentBox>
            </PageContainer>
        </Layout>
    );
};

export default CheckInByPinPage;