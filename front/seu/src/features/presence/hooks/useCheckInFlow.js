import { useState } from 'react';
import { useNavigate } from 'react-router-dom'; 
import { CheckinService } from '../../../api/modules/CheckinService'; // CORRIGIDO

export const useCheckInFlow = () => {
    const navigate = useNavigate(); 
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const clearError = () => setError(null);

    const validarPin = async (inputPin) => {
        if (inputPin.length !== 6) {
            setError("O código deve ter 6 dígitos.");
            return;
        }

        setLoading(true);
        setError(null);
        
        try {
            
            await CheckinService.validarPin(inputPin); // CORRIGIDO
            
            localStorage.setItem('currentPin', inputPin); 
            
            navigate('/checkin/geolocation'); 
            
        } catch (err) {
            const apiError = err || "Código de Check-in inválido ou expirado.";
            setError(apiError);
        } finally {
            setLoading(false);
        }
    };
    
    return {
        loading,
        error,
        validarPin,
        clearError,
    };
};