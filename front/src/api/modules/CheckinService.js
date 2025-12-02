// src/api/modules/CheckinService.js

import api from '../httpClient';

const extractApiError = (error) => {
    const apiError = error.response?.data?.erros?.[0] 
                   || error.response?.data 
                   || error.message;
    if (typeof apiError === 'string') {
        return apiError;
    }
    return "Erro desconhecido ao comunicar com a API.";
};


export const CheckinService = {
  
  validarPin: async (pin) => {
    const request = { Pin: pin };

    try {
      const response = await api.post('/Checkin/validar-pin', request);
      return response.data;
    } catch (error) {
      throw new Error(extractApiError(error));
    }
  },

  validarGeolocalizacao: async (pin, latitude, longitude) => {
    try {
      const params = new URLSearchParams({
        pin: String(pin), 
        latitude: String(latitude),
        longitude: String(longitude)
      });

      const response = await api.get(`/Checkin/validar-geolocalizacao?${params.toString()}`);
      if (!response.data && response.status === 200) {
        return { message: "Check-in efetuado." };
      }

      return response.data;
    } catch (error) {
      throw new Error(extractApiError(error));
    }
  },
  registrarCheckout: async (latitude, longitude) => {
    try {
      const params = new URLSearchParams({
        latitude: String(latitude),
        longitude: String(longitude)
      })
      const response = await api.get(`/Checkin/registrar-checkout?${params.toString()}`); 
      
      if (!response.data) {
        return { message: "Check-out efetuado." };
      }
      
      return response.data;
    } catch (error) {
    }
  }
};