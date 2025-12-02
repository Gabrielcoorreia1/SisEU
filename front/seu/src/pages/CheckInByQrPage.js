// src/pages/CheckInByQrPage.js
import React, { useState, useRef, useEffect } from 'react';
import styled from 'styled-components';
import Layout from '../components/Shared/Layout';
import Button from '../components/base/Button';
import Icon from 'feather-icons-react';
import { useNavigate } from 'react-router-dom';
import Alert from '../components/Shared/patterns/Alert/Index';

import { useCheckInFlow } from '../features/presence/hooks/useCheckInFlow'; 
import { useCameraPermission } from '../features/presence/hooks/useCameraPermission'; 

const VideoPlayer = ({ stream }) => {
    const videoRef = useRef(null);
    
    useEffect(() => {
        if (videoRef.current && stream) {
            videoRef.current.srcObject = stream;
        }
    }, [stream]);
    
    return (
        <video
            ref={videoRef}
            autoPlay
            playsInline
            muted
            style={{ width: '100%', height: '100%', objectFit: 'cover', borderRadius: '8px' }}
        />
    );
};

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
    max-width: 500px;
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

const CameraPlaceholder = styled.div`
    width: 100%;
    height: 350px; 
    background-color: #f0f0f0; 
    border: 1px solid var(--primary-brand);
    border-radius: 8px;
    margin-bottom: 20px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    color: #333; 
    font-size: 0.9rem;
    overflow: hidden; /* Garante que o vídeo se encaixe no placeholder */
`;


const CheckInByQrPage = () => {
    const navigate = useNavigate();
    const { loading, error, validarPin, clearError } = useCheckInFlow(); 
    
    const { permissionStatus, isGranted, error: cameraError, stream } = useCameraPermission(true); 
    const [qrCodeData, setQrCodeData] = useState(null); 

    const handleQrCodeScan = (data) => {
        if (loading || qrCodeData) return;
        setQrCodeData(data); 
        validarPin(data);
    };
    
    const handleClose = () => {
       if (!loading) {
        clearError(); 
        navigate('/dashboard');
       }
    };

    const renderCameraContent = () => {
        if (cameraError) {
            return <p style={{ color: 'red' }}>ERRO: {cameraError}</p>;
        }
        if (permissionStatus === 'pending') {
            return <p>Aguardando permissão de câmera...</p>;
        }
        if (permissionStatus === 'denied') {
            return <p style={{ color: 'red', padding: '10px' }}>ACESSO NEGADO: Habilite a câmera nas configurações do navegador/dispositivo.</p>;
        }
        if (isGranted && stream) {
             return <VideoPlayer stream={stream} />;
        }
        
        return (
            <p>
                Câmera Inativa.
            </p>
        );
    };


    return (
        <Layout>
            <PageContainer>
                <ContentBox>
                    <CloseIcon icon="x" size={24} onClick={handleClose} />
                    
                    <h2 style={{ color: 'var(--theme-text)', marginBottom: '30px' }}>
                        Leitura de QR Code
                    </h2>

                    <CameraPlaceholder>
                        {renderCameraContent()}
                    </CameraPlaceholder>

                    {isGranted && !qrCodeData && (
                        <Button
                            corPrimaria={"var(--primary-brand)"}
                            corSecundaria={"var(--branco)"}
                            text={loading ? "Verificando..." : "Simular Leitura (PIN: 123456)"}
                            onClick={() => handleQrCodeScan('123456')}
                            disabled={loading}
                            style={{ width: '100%' }}
                        />
                    )}
                    
                    {(error || qrCodeData) && (
                        <Alert message={error || "Código capturado. Verificando..."} show={!!(error || qrCodeData)} animationKey={1} />
                    )}
                    
                </ContentBox>
            </PageContainer>
        </Layout>
    );
};

export default CheckInByQrPage;