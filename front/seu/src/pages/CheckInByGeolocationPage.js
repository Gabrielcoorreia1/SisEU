import React, { useState, useEffect } from 'react';
import styled from 'styled-components';
import Layout from '../components/Shared/Layout';
import Button from '../components/base/Button';
import Icon from 'feather-icons-react';
import { useNavigate } from 'react-router-dom';
import Alert from '../components/Shared/patterns/Alert/Index';
import { useGeolocation } from '../features/presence/hooks/useGeolocation';
import { CheckinService } from '../api/modules/CheckinService'; 
import { usePresenceStatus } from '../features/presence/hooks/usePresenceStatus'; 

const GEOLOC_STEPS = { GEOLOC_CONFIRM: 1, DADOS_REVISAO: 2, SUCESSO: 3 };

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
  h2, p {
      color: var(--theme-text);
  }
`;

const CloseIcon = styled(Icon)`
    position: absolute;
    top: 15px;
    right: 15px;
    cursor: pointer;
    color: var(--theme-text);
    z-index: 10;
`;

const MapPlaceholder = styled.div`
    width: 100%;
    max-width: 280px; 
    height: 280px;
    background-color: #f0f0f0;
    border-radius: 50%;
    margin: 30px auto;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
`;

const CheckInByGeolocationPage = () => {
    const navigate = useNavigate();
    const [subStep, setSubStep] = useState(GEOLOC_STEPS.GEOLOC_CONFIRM);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const { coords, isPermitted, error: gpsError } = useGeolocation(true); 
    
    const { markCheckedIn, markCheckedOut } = usePresenceStatus(); 
    const flowMode = localStorage.getItem('flowMode') || 'CHECKIN';
    const isCheckout = flowMode === 'CHECKOUT';
    const eventPin = localStorage.getItem('currentPin');
    const nomeUsuario = "Maria Fernanda Ferreira Paulino"; 
    const horaAtual = new Date().toLocaleTimeString();

    const updatePresenceStatus = isCheckout ? markCheckedOut : markCheckedIn;


    useEffect(() => {
        if (!eventPin) {
            alert("Erro de segurança: Código de Check-in não encontrado.");
            localStorage.removeItem('flowMode');
            localStorage.removeItem('currentPin');
            navigate('/dashboard');
        }
    }, [eventPin, navigate]);

    const handleConfirmarDados = async () => {
        if (!eventPin || !coords) {
            setError("Dados de localização não obtidos. Tente novamente.");
            return;
        }

        setLoading(true);
        setError(null);
        
        try {
            if (isCheckout) {
                console.warn("Fluxo de Checkout não implementado na API C#");
                throw new Error("Funcionalidade de Check-out ainda não disponível.");
            } else {
                await CheckinService.validarGeolocalizacao(eventPin, coords.latitude, coords.longitude);
            }
            
            updatePresenceStatus(); 
            setSubStep(GEOLOC_STEPS.SUCESSO); 
            
        } catch (err) {
            const apiError = err.message || err || `Falha na validação de ${isCheckout ? 'Check-out' : 'Check-in'}.`;
            setError(apiError);
        } finally {
            setLoading(false);
        }
    }


    const renderStepContent = () => {
        const currentActionText = isCheckout ? "Check-out" : "Check-in";
        
        if (!isPermitted || gpsError) {
            return (
                <div style={{ color: 'var(--theme-text)', textAlign: 'center' }}>
                    <Icon icon="alert-octagon" size={50} style={{ color: 'red' }} />
                    <h2 style={{ color: 'var(--theme-text)' }}>GPS Necessário</h2>
                    <p style={{ color: 'red' }}>Não foi possível obter sua localização. Verifique as permissões.</p>
                </div>
            );
        }
        if (!coords) {
            return <p style={{ color: 'var(--theme-text)', textAlign: 'center' }}>Obtendo sua localização...</p>;
        }

        switch (subStep) {
            case GEOLOC_STEPS.GEOLOC_CONFIRM:
                return (
                    <div style={{ textAlign: 'center' }}>
                        <h2 style={{ color: 'var(--theme-text)', marginBottom: '30px' }}>
                            Está é a sua localização?
                        </h2>
                        
                        <MapPlaceholder>
                           <div style={{ color: '#333', padding: '20px', textAlign: 'center', fontWeight: 'bold' }}>
                               <p style={{color: '#000'}}>Localização Adquirida</p>
                               <p style={{ fontSize: '0.9rem', marginTop: '10px', color: '#000'}}>Lat: {coords.latitude.toFixed(6)}</p>
                               <p style={{ fontSize: '0.9rem', color: '#000' }}>Lon: {coords.longitude.toFixed(6)}</p>
                           </div>
                        </MapPlaceholder>

                        <Button 
                            corPrimaria={"var(--primary-brand)"}
                            corSecundaria={"var(--branco)"}
                            text={"Confirmar Localização"}
                            onClick={() => setSubStep(GEOLOC_STEPS.DADOS_REVISAO)}
                            style={{ marginTop: '40px', width: '100%' }}
                        />
                    </div>
                );

            case GEOLOC_STEPS.DADOS_REVISAO:
                return (
                    <div style={{ textAlign: 'left', color: 'var(--theme-text)' }}>
                        <h2 style={{ marginBottom: '30px', fontWeight: 'bold', color: 'var(--theme-text)' }}>
                            Dados de {currentActionText}:
                        </h2>
                        
                        <p style={{ fontWeight: 'bold' }}>Nome: {nomeUsuario}</p>
                        <p style={{ fontWeight: 'bold' }}>Horário: {horaAtual}</p>
                        <p style={{ fontWeight: 'bold', marginBottom: '10px' }}>Coordenadas:</p>

                        <div style={{ width: '100%', height: '150px', backgroundColor: '#ccc', margin: '0 auto', padding: '10px' }}>
                           <div style={{ color: '#000', padding: '10px', textAlign: 'left' }}>
                               <p style={{ color: '#000'}}>Latitude: {coords.latitude.toFixed(6)}</p>
                               <p style={{ color: '#000'}}>Longitude: {coords.longitude.toFixed(6)}</p>
                           </div>
                        </div>
                        
                        <Button 
                            corPrimaria={"var(--primary-brand)"}
                            corSecundaria={"var(--branco)"}
                            text={loading ? `Confirmando ${currentActionText}...` : `Confirmar Dados`}
                            disabled={loading}
                            onClick={handleConfirmarDados}
                            style={{ marginTop: '40px', width: '100%' }}
                        />
                    </div>
                );
                
            case GEOLOC_STEPS.SUCESSO:
                 return (
                    <div style={{ textAlign: 'center' }}>
                        <Icon icon="check-circle" size={80} style={{ color: 'green', marginBottom: '20px' }} />
                        <h2 style={{ color: 'green' }}>{currentActionText} Registrado!</h2>
                        <p style={{ color: 'var(--theme-text)', marginBottom: '30px' }}>Seu registro foi concluído com sucesso às {horaAtual}.</p>
                        <Button
                            corPrimaria={"var(--primary-brand)"}
                            corSecundaria={"var(--branco)"}
                            text="Voltar ao Dashboard"
                            onClick={() => {
                                localStorage.removeItem('flowMode');
                                localStorage.removeItem('currentPin');
                                navigate('/dashboard');
                            }}
                        />
                    </div>
                );

            default:
                return <p style={{ color: 'red' }}>Erro no fluxo.</p>;
        }
    };

    return (
        <Layout>
            <PageContainer>
                <ContentBox>
                    <CloseIcon icon="x" size={24} onClick={() => {
                        localStorage.removeItem('flowMode');
                        localStorage.removeItem('currentPin');
                        navigate('/dashboard');
                    }} />
                    
                    {error && <Alert message={error} show={!!error} animationKey={Date.now()} />}
                    
                    {renderStepContent()}
                    
                </ContentBox>
            </PageContainer>
        </Layout>
    );
};

export default CheckInByGeolocationPage;