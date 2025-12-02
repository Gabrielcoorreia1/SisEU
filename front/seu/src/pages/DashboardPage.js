// src/pages/DashboardPage.js

import Button from '../components/base/Button/index';
import React from 'react';
import styled from 'styled-components';
import { useSessions } from '../hooks/useSessions';
import SessionCard from '../features/sessions/components/SessionCard';
import Layout from '../components/Shared/Layout/index'; 

import ModalSelecionarCheckIn from '../features/presence/ModalSelecionarCheckIn/Index'; 
import { useCheckInModal } from '../features/presence/hooks/useCheckInModal'; 

import { usePresenceStatus, PRESENCE_STATUS } from '../features/presence/hooks/usePresenceStatus'; 
import { useNavigate } from 'react-router-dom'; 

const CheckInBanner = styled.div`
  background-color: white;
  color: #333333;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;

  @media (max-width: 768px) {
    flex-direction: column;
    text-align: center;
  }
`;

const CheckInText = styled.p`
  font-size: 1.2rem;
  font-weight: 500;
  margin: 0;
  
  @media (max-width: 768px) {
    margin-bottom: 15px;
  }
`;
const SessionsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); 
  gap: 20px;
  padding: 20px 0;
  @media (min-width: 1400px) { 
      grid-template-columns: repeat(4, 1fr);
      justify-content: center;
  }
  @media (max-width: 768px) {
    grid-template-columns: 1fr;
  }
`;
const TabsContainer = styled.div`
  display: flex;
  border-bottom: 2px solid #ddd;
  margin-bottom: 20px;
`;

const Tab = styled.button`
  background: none;
  border: none;
  padding: 10px 20px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  color: ${props => (props.$active ? '#3B5998' : '#888')};
  border-bottom: ${props => (props.$active ? '2px solid #3B5998' : 'none')};
  margin-bottom: -2px; 

  &:hover {
    color: #3B5998;
  }
`;
const getCardColor = (index) => {
  const colors = ['#3B5998', '#FF8C00', '#8A2BE2']; 
  return colors[index % colors.length];
};


const DashboardPage = () => {
  const { sessions, loading } = useSessions();
  const [activeTab, setActiveTab] = React.useState('Todas'); 
  const { isOpen, openModal, closeModal, selectMethod } = useCheckInModal();
  const navigate = useNavigate();
  const { status: presenceStatus, loading: loadingPresence } = usePresenceStatus();

  const handleCheckOut = () => {
        localStorage.setItem('flowMode', 'CHECKOUT');
        navigate('/checkin/geolocation'); 
    };
    
  const isCheckedIn = presenceStatus === PRESENCE_STATUS.CHECKED_IN;
  const buttonText = isCheckedIn ? "Check-out" : "Check-in";
  const buttonAction = isCheckedIn ? handleCheckOut : openModal;

  if (loading || loadingPresence) {
    return <Layout><p>Carregando eventos e apresentações...</p></Layout>;
  }

  const filteredSessions = sessions; 

  return (
    <Layout>
      <CheckInBanner>
        <CheckInText>
          {isCheckedIn 
              ? "Finalize seu registro de presença (Check-out):" 
              : "Faça seu check-in para a creditação de horas:"}
        </CheckInText>
        <Button 
          corPrimaria="#3B5998" 
          corSecundaria="#FFF"
          text={buttonText}
          onClick={buttonAction}
          disabled={loading || loadingPresence}
        />
      </CheckInBanner>

      <h1>Sessões:</h1>
      <TabsContainer>
        <Tab $active={activeTab === 'Minhas Sessões'} onClick={() => setActiveTab('Minhas Sessões')}>
          Minhas sessões
        </Tab>
        <Tab $active={activeTab === 'Todas'} onClick={() => setActiveTab('Todas')}>
          Todas
        </Tab>
      </TabsContainer>

      <SessionsGrid>
        {filteredSessions.map((session, index) => (
          <SessionCard 
            key={session.id || index} 
            session={session} 
            cardColor={getCardColor(index)}
          />
        ))}
      </SessionsGrid>
      
      {filteredSessions.length === 0 && (
        <p>Nenhuma sessão encontrada para a categoria "{activeTab}".</p>
      )}
      <ModalSelecionarCheckIn 
        isOpen={isOpen} 
        onClose={closeModal} 
        onSelectMethod={selectMethod}
      />

    </Layout>
  );
};

export default DashboardPage;