import React, { useRef } from 'react';
import styled from 'styled-components';
import Icon from 'feather-icons-react';
import QRCode from 'react-qr-code'; 
import Button from '../../components/base/Button';

const handlePrint = () => {
    window.print();
};

const ModalOverlay = styled.div`
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background-color: rgba(0, 0, 0, 0.7);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    
    @media print {
        background-color: white;
        & > * {
            display: none; 
        }
    }
`;

const ModalContent = styled.div`
    background: var(--theme-card-bg);
    color: var(--theme-text);
    padding: 30px;
    border-radius: 8px;
    max-width: 400px;
    width: 100%;
    text-align: center;
    position: relative;
    
    @media print {
        display: block !important;
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        box-shadow: none;
        max-width: 100%;
        width: 100%;
    }
`;

const CloseIcon = styled(Icon)`
    position: absolute;
    top: 15px;
    right: 15px;
    cursor: pointer;
    color: var(--theme-text);

    @media print {
        display: none;
    }
`;

const PrintButton = styled(Button)`
    margin-top: 20px;
    @media print {
        display: none;
    }
`;

const PinQrCodeModal = ({ isOpen, onClose, pin }) => {
    const contentRef = useRef(null); 

    if (!isOpen) return null;

    return (
        <ModalOverlay onClick={onClose}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
                <CloseIcon icon="x" size={30} onClick={onClose} />

                <div ref={contentRef}>
                    <h2 style={{ marginTop: 0 }}>PIN de Check-in Global</h2>
                    <p>Este é o PIN e QRCode para o evento principal.</p>

                    <div style={{
                        border: '1px solid #ccc',
                        borderRadius: '8px',
                        padding: '15px',
                        marginTop: '20px',
                        backgroundColor: '#f9f9f9'
                    }}>
                        <h3 style={{
                            fontSize: '2rem',
                            letterSpacing: '5px',
                            margin: '15px 0',
                            color: '#333',
                            fontFamily: 'monospace'
                        }}>
                            {pin || "Carregando..."}
                        </h3>

                        <div style={{ background: 'white', padding: '16px', display: 'inline-block', borderRadius: '4px', margin: '10px 0' }}>
                            {pin ? (
                                <QRCode 
                                    value={pin}
                                    size={200} 
                                />
                            ) : (
                                <p>Carregando QRCode...</p>
                            )}
                        </div>
                    </div>
                </div>

                <PrintButton
                    onClick={handlePrint}
                    corPrimaria={"#007bff"}
                    corSecundaria={"#FFF"}
                    text={"Imprimir PIN e QR Code"}
                />

            </ModalContent>
        </ModalOverlay>
    );
};

export default PinQrCodeModal;