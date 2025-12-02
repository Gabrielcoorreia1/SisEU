// src/features/presence/hooks/useCheckInModal.js
import { useState } from 'react';
import { useNavigate } from 'react-router-dom'; 

export const useCheckInModal = () => {

    const navigate = useNavigate(); 
    const [isOpen, setIsOpen] = useState(false);

    const openModal = () => setIsOpen(true);
    const closeModal = () => setIsOpen(false);

    const selectMethod = (method) => {
        closeModal();
        if (method === 'PIN') {
            navigate('/checkin/pin'); 
        } else if (method === 'QR_CODE') {
            navigate('/checkin/qr');
        } else {
            navigate('/dashboard');
        }
    };

    return { isOpen, openModal, closeModal, selectMethod };
};