// src/api/httpClient.js
import axios from 'axios';

const BASE_URL = 'http://localhost:8080/api';

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

api.interceptors.request.use(
  (config) => {

    const token = localStorage.getItem('authToken')
                || localStorage.getItem('accessToken')
                || localStorage.getItem('token')
                || '';
    console.log("HttpClient Interceptor - Token lido (comprimento):", token ? token.length : 0);

    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }

    console.log(`[HTTP] ${config.method?.toUpperCase() || 'GET'} ${config.baseURL}${config.url}`, {
      headers: {
        Authorization: config.headers?.Authorization,
        'Content-Type': config.headers?.['Content-Type']
      },
      params: config.params,
      data: config.data
    });

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => {
    console.log(`[HTTP-RESP] ${response.status} ${response.config.url}`, {
      data: response.data === '' ? '<empty>' : response.data
    });
    return response;
  },
  (error) => {
    if (error.response) {
      console.warn(`[HTTP-ERR] ${error.response.status} ${error.config?.url}`, {
        data: error.response.data
      });
    } else {
      console.error('[HTTP-ERR] sem resposta do servidor', error.message);
    }
    return Promise.reject(error);
  }
);

export default api;
