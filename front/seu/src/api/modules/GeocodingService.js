// src/api/modules/GeocodingService.js
import axios from 'axios';

const GEOCODING_API_KEY = "SUA_CHAVE_DE_API_AQUI";
const REVERSE_GEOCODING_URL = "https://us1.locationiq.com/v1/reverse.php";

export const GeocodingService = {
  fetchAddress: async (latitude, longitude) => {

    try {
      const response = await axios.get(REVERSE_GEOCODING_URL, {
        params: {
          key: GEOCODING_API_KEY,
          lat: latitude,
          lon: longitude,
          format: 'json',
        },
        timeout: 8000
      });

      return response.data;
    } catch (error) {
      console.error("Erro no Reverse Geocoding:", error?.message || error);
      return {
        error: true,
        display_name: "Não foi possível carregar o endereço. Tente novamente."
      };
    }
  }
};

export default GeocodingService;
