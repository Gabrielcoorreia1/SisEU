// src/api/modules/AuthService.js
import api from '../httpClient'; 

const AUTH_TOKEN_KEY = 'authToken';
const CURRENT_USER_KEY = 'currentUser'; 

export const AuthService = {

  login: async (cpf, password) => {
    
    const targetPath = '/Authenticacoes/login'; 
    const baseUrl = api.defaults.baseURL;

    const fullUrl = `${baseUrl}${targetPath}`;

    console.log(`[AuthService] Tentando POST para: ${fullUrl} com CPF: ${cpf}`);

    try {

      const response = await api.post('/Authenticacoes/login', { cpf, Senha: password });

      console.log("[AuthService] Resposta da API:", response.data);
      const { token, user } = response.data;
      if (token) {
        localStorage.setItem(AUTH_TOKEN_KEY, token);
        console.log("[AuthService] Token salvo.");
        if (user) {
            localStorage.setItem(CURRENT_USER_KEY, JSON.stringify(user));
            console.log("[AuthService] User salvo.");
        }
      } else {
         console.warn("[AuthService] API OK, mas sem token.");
         localStorage.removeItem(AUTH_TOKEN_KEY);
         localStorage.removeItem(CURRENT_USER_KEY);
      }
      return response.data;

    } catch (error) {
      console.error("[AuthService] Erro na chamada da API:", error.response?.status, error.response?.data);
      localStorage.removeItem(AUTH_TOKEN_KEY);
      localStorage.removeItem(CURRENT_USER_KEY);
      throw error;
    }
  },
  
  logout: () => {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    localStorage.removeItem(CURRENT_USER_KEY);
    console.log("[AuthService] Logout.");
  }
};