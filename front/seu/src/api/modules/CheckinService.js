// src/api/modules/CheckinService.js
import api from '../httpClient';

export const CheckinService = {
  validarPin: async (pin) => {
    const request = { Pin: pin };

    try {
      const response = await api.post('/Checkin/validar-pin', request);
      return response.data;
    } catch (error) {
      const message = error?.response?.data?.message
                    || error?.response?.data
                    || error?.message
                    || "Erro ao validar PIN.";
      throw new Error(typeof message === 'string' ? message : JSON.stringify(message));
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
      if (response.status === 204) {
        throw new Error("Validação retornou sem conteúdo (204). Verifique autenticação / parâmetros.");
      }

      if (!response.data) {
        throw new Error("Resposta vazia da API ao validar geolocalização.");
      }

      return response.data;
    } catch (error) {
      const message = error?.response?.data?.message
                    || error?.response?.data
                    || error?.message
                    || "Erro ao validar Geolocalização.";
      throw new Error(typeof message === 'string' ? message : JSON.stringify(message));
    }
  }
};
