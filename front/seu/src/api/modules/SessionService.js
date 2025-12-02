// src/api/modules/SessionService.js
import api from '../httpClient';

const MOCK_TOKEN = "MOCK_ADMIN_TOKEN";

export const SessionService = {
  
  getSessions: async () => {
    try {
      const response = await api.get('/Eventos/ObterEventos'); 
      return response.data;
    } catch (error) {
      return []; 
    }
  },

  loadSession: async (sessaoId) => {
    if (sessaoId === "MOCK-ID") {
        await new Promise(resolve => setTimeout(resolve, 500));
        return { 
            titulo: "Sessão de Teste Mockada", 
            eventType: 2, 
            local: { campus: 0, departamento: "SI", bloco: "A", sala: "101" },
        };
    }
    try {
        const response = await api.get(`/Eventos/${sessaoId}`);
        return response.data;
    } catch (error) {
        throw error;
    }
  },

  createSession: async (request) => {
    const token = localStorage.getItem('authToken') || MOCK_TOKEN;
    
    if (request.Titulo === "TESTE") {
        await new Promise(resolve => setTimeout(resolve, 500));
        return { success: true, message: "Sessão criada via Mock." };
    }
    try {
      const response = await api.post('/Eventos', request, {
        headers: {
            Authorization: `Bearer ${token}`, 
        },
      });
      return response.data; 
    } catch (error) {
      throw error;
    }
  },

  updateSession: async (sessaoId, request) => {
    const token = localStorage.getItem('authToken') || MOCK_TOKEN;

    try {
        const response = await api.put(`/Eventos/${sessaoId}`, request, {
            headers: { Authorization: `Bearer ${token}` },
        });
        return response.data;
    } catch (error) {
        throw error;
    }
  },
  deleteSession: async (sessaoId) => {
    const token = localStorage.getItem('authToken') || MOCK_TOKEN;
    
    try {
        const response = await api.delete(`/Eventos/${sessaoId}`, {
            headers: { Authorization: `Bearer ${token}` },
        });
        return response.data;
    } catch (error) {
        throw error;
    }
  },
  buscarOrganizadores: async (nome) => {
    if (nome === "mock") {
        return [{ id: 1, nomeCompleto: "Mock Organizador" }];
    }
  
    try {
        const response = await api.get(`/Authenticacoes/buscar?nome=${nome}`);
        return response.data.organizadores || []; 
    } catch (error) {
        throw error;
    }
  },
};