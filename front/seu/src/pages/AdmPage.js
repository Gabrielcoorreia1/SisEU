import React, { useState, useEffect } from 'react';
import Layout from '../components/Shared/Layout';
import styled from 'styled-components';
import Button from '../components/base/Button';
import ConfigureSessionModal from '../features/sessions/ConfigureSessionModal/index'; 
import SessionCard from '../features/sessions/components/SessionCard'; 
import { useNavigate } from 'react-router-dom'; 

import PinQrCodeModal from '../features/checkin/PinQrCodeModal';
import { EventoService } from '../service/EventoService';

import { useSessions } from '../hooks/useSessions'; 
import { RelatorioService } from '../service/RelatorioService';

import './Pages.css';
import '../components/PalestraCartao/PalestraCartao.css';

const SessionsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); 
  gap: 20px;
  padding: 20px 0;
`;
const getCardColor = (index) => {
  const colors = ['#3B5998', '#FF8C00', '#8A2BE2']; 
  return colors[index % colors.length];
};
const AdminContainer = styled.div`
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
`;
const HeaderAdmin = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;
    h1 {
        color: var(--theme-text);
        margin: 0;
    }
    .header-buttons {
        display: flex;
        gap: 15px;
    }
`;


const AdmPage = () => {
    const navigate = useNavigate();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isAdmin, setIsAdmin] = useState(false); 
    const [sessaoSelecionadaId, setSessaoSelecionadaId] = useState(null);
    
    const [isPinModalOpen, setIsPinModalOpen] = useState(false);
    const [eventoPin, setEventoPin] = useState(null);


    const { sessions: listaSessoes, loading, error, refetchSessions } = useSessions(); 

    useEffect(() => {
        const userRole = localStorage.getItem('userRole');
        if (userRole !== '4') { 
            alert("Acesso negado. Faça login como Administrador.");
            navigate('/'); 
        } else {
            setIsAdmin(true);
        }
    }, [navigate]);

    const handleAddSession = () => {
        setSessaoSelecionadaId(null)
        setIsModalOpen(true);
    };
    const handleEditSession = (sessaoId) => {
        setSessaoSelecionadaId(sessaoId); 
        setIsModalOpen(true);
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setSessaoSelecionadaId(null);
    };
    const handleSaveChangesAndClose = async () => {
        await refetchSessions();
        handleCloseModal(); 
    };

    const handleGenerateReports = () => {
        console.log("Gerando relatório global mocado...");
        RelatorioService.GerarRelatorioCSV();
    };
   const handleShowPinModal = async () => {
        try {
            
            const confirm = window.confirm("Você tem certeza? Gerar um novo PIN invalidará o PIN anterior.");
            
            if (confirm) {
                const response = await EventoService.gerarNovoPin(); 
                
                const pinAtivo = response.pin; 
                
                if (pinAtivo) {
                    setEventoPin(pinAtivo);
                    setIsPinModalOpen(true);
                } else {
                    console.error("API respondeu com sucesso, mas o PIN está vazio.");
                    alert("SUCESSO PARCIAL: O PIN não foi gerado. Verifique o InitBD.cs.");
                }
            }

        } catch (err) {
            console.error("Erro ao obter PIN da API:", err);
            alert("Erro de comunicação com a API ao tentar obter o PIN.");
        }
    };

    if (!isAdmin) {
        return <div>Verificando permissões...</div>; 
    }

    return (
        <Layout>
            <AdminContainer>
                <HeaderAdmin>
                   <h1>Gerenciamento de Sessões</h1>
                   <div className="header-buttons">
                        <div style={{ maxWidth: '200px' }}>
                            <Button 
                                corPrimaria={"var(--primary-brand)"} 
                                corSecundaria={"var(--branco)"}
                                text={"Gerar Relatórios"}
                                onClick={handleGenerateReports} 
                            />
                        </div>
                        <div style={{ maxWidth: '200px' }}>
                            <Button 
                                corPrimaria={"#007bff"} 
                                corSecundaria={"var(--branco)"}
                                text={"Gerar Novo PIN"}
                                onClick={handleShowPinModal} 
                            />
                        </div>
                   </div>
                </HeaderAdmin>
                
                <div style={{ textAlign: 'left', maxWidth: '300px', marginBottom: '20px' }}>
                    <Button 
                        corPrimaria={"var(--secondary-brand)"} 
                        corSecundaria={"var(--branco)"}
                        text={"Adicionar Nova Sessão"}
                        onClick={handleAddSession}
                    />
                </div>
                <SessionsGrid>
                    {!loading && !error && listaSessoes.length === 0 && (
                        <p>Nenhuma sessão encontrada no seu banco de dados.</p>
                    )}
                    
                    {listaSessoes.map((session, index) => (
                        <div 
                            key={session.id || index}
                            onClick={() => handleEditSession(session.id)}
                            style={{ cursor: 'pointer' }}
                        >
                            <SessionCard 
                                session={session} 
                                cardColor={getCardColor(index)}
                            />
                        </div>
                    ))}
                </SessionsGrid>
            </AdminContainer>
            
            <ConfigureSessionModal 
                isOpen={isModalOpen} 
                onClose={handleCloseModal} 
                sessaoId={sessaoSelecionadaId} 
                onChangeSessoes={handleSaveChangesAndClose} 
            />

            <PinQrCodeModal
                isOpen={isPinModalOpen}
                onClose={() => setIsPinModalOpen(false)}
                pin={eventoPin}
            />

        </Layout>
    );
};

export default AdmPage;