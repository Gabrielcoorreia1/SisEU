// /Sistema-Encontros-Universitarios/seu/src/hooks/useSessions.js

import { useState, useEffect } from 'react';
import { SessionService } from '../api/modules/SessionService'; 


export const useSessions = () => {
    const [sessions, setSessions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    
    const [reloadCounter, setReloadCounter] = useState(0); 

    const fetchSessions = async () => {
        setLoading(true);
        try {
            const data = await SessionService.getSessions(); 
            
            setSessions(data);
            setError(null);
        } catch (err) {
            console.error("Erro ao buscar sessões:", err);
            setError(err.message || 'Falha ao buscar sessões da API.');
            setSessions([]); 
        } finally {
            setLoading(false);
        }
    };

    const refetchSessions = () => {
        setReloadCounter(prev => prev + 1);
    };


    useEffect(() => {
        fetchSessions();
    }, [reloadCounter]);

    return { sessions, loading, error, refetchSessions };
};