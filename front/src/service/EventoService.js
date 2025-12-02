// src/service/EventoService.js

import api from '../api/httpClient';

export const EventoService = {
    
    getPinAtivo: async () => { 
        try {
            const response = await api.get(`/Checkin/pin-ativo`); 
            return response.data; 
        } catch (error) {
            throw error;
        }
    },

    gerarNovoPin: async () => {
        try {
            const response = await api.post(`/Checkin/gerar-novo-pin`);
            return response.data;
        } catch (error) {
            throw error;
        }
    }
    
};