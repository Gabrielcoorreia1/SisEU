// src/service/MockAuthService.js

const CURRENT_USER_KEY = 'currentUser';
const AUTH_TOKEN_KEY = 'authToken'; 

export const MockAuthService = {

  login: (cpf, senha) => {
    const todosUsuarios = JSON.parse(localStorage.getItem('mockUsuarios') || '[]');
    const usuarioEncontrado = todosUsuarios.find(
      user => user.cpf === cpf && user.senha === senha
    );

    if (!usuarioEncontrado) {
      localStorage.removeItem(CURRENT_USER_KEY);
      localStorage.removeItem(AUTH_TOKEN_KEY); 
      return null;
    }

    const { senha: _, ...usuarioParaSalvar } = usuarioEncontrado;
    
    localStorage.setItem(CURRENT_USER_KEY, JSON.stringify(usuarioParaSalvar));
    localStorage.setItem(AUTH_TOKEN_KEY, 'fake-mock-token-12345');
    
    return usuarioParaSalvar;
  },

  logout: () => {
    localStorage.removeItem(CURRENT_USER_KEY);
    localStorage.removeItem(AUTH_TOKEN_KEY);
  },

  getCurrentUser: () => {
    try {
      const user = localStorage.getItem(CURRENT_USER_KEY);
      return user ? JSON.parse(user) : null;
    } catch (error) {
      console.error("Erro ao ler usuário do localStorage", error);
      return null;
    }
  },

  isAdmin: () => {
    const user = MockAuthService.getCurrentUser();
    return user && user.tipoUsuario === 'ADMIN';
  }
};