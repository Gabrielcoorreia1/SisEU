// src/api/mockData.js

export const mockUsuarios = [
  { 
    id: 'user-1', 
    nomeCompleto: 'Alice Silva', 
    cpf: '11111111111', 
    senha: '123', 
    tipoUsuario: 'COMUM' 
  },
  { 
    id: 'user-2', 
    nomeCompleto: 'Bruno Riccelli', 
    cpf: '22222222222', 
    senha: 'admin', 
    tipoUsuario: 'ADMIN' 
  },
  { 
    id: 'user-3', 
    nomeCompleto: 'Carla Dias', 
    cpf: '33333333333', 
    senha: '123', 
    tipoUsuario: 'COMUM' 
  },
  { 
    id: 'user-4', 
    nomeCompleto: 'Daniel Moreira',
    cpf: '44444444444', 
    senha: '123', 
    tipoUsuario: 'COMUM' 
  },
];
export const mockSessoes = [
  { id: 'sessao-a', titulo: 'Apresentação de IA' },
  { id: 'sessao-b', titulo: 'Workshop de React' },
];


const presencasIniciais = [


  { id: 'p1', usuarioId: 'user-1', sessaoId: 'sessao-a', tipo: 'CHECK_IN',  horario: '2025-10-27T09:00:15Z' },
  { id: 'p2', usuarioId: 'user-1', sessaoId: 'sessao-a', tipo: 'CHECK_OUT', horario: '2025-10-27T10:55:00Z' },
  
  { id: 'p3', usuarioId: 'user-2', sessaoId: 'sessao-a', tipo: 'CHECK_IN',  horario: '2025-10-27T09:02:30Z' },
  { id: 'p4', usuarioId: 'user-2', sessaoId: 'sessao-a', tipo: 'CHECK_OUT', horario: '2025-10-27T11:01:00Z' },

  { id: 'p5', usuarioId: 'user-3', sessaoId: 'sessao-a', tipo: 'CHECK_IN',  horario: '2025-10-27T09:05:00Z' },
  { id: 'p6', usuarioId: 'user-3', sessaoId: 'sessao-a', tipo: 'CHECK_OUT', horario: '2025-10-27T09:30:00Z' }, 
  { id: 'p7', usuarioId: 'user-3', sessaoId: 'sessao-a', tipo: 'CHECK_IN',  horario: '2025-10-27T09:45:00Z' },
  { id: 'p8', usuarioId: 'user-3', sessaoId: 'sessao-a', tipo: 'CHECK_OUT', horario: '2025-10-27T11:00:00Z' },

  { id: 'p9', usuarioId: 'user-4', sessaoId: 'sessao-a', tipo: 'CHECK_IN',  horario: '2025-10-27T09:10:00Z' },

  { id: 'p10', usuarioId: 'user-1', sessaoId: 'sessao-b', tipo: 'CHECK_IN',  horario: '2025-10-27T14:00:00Z' },
  { id: 'p11', usuarioId: 'user-1', sessaoId: 'sessao-b', tipo: 'CHECK_OUT', horario: '2025-10-27T16:00:00Z' },
];

export const popularLocalStorage = () => {
  if (!localStorage.getItem('mockPresencas')) {
    localStorage.setItem('mockPresencas', JSON.stringify(presencasIniciais));
  }
  if (!localStorage.getItem('mockUsuarios')) {
    localStorage.setItem('mockUsuarios', JSON.stringify(mockUsuarios));
  }
  if (!localStorage.getItem('mockSessoes')) {
    localStorage.setItem('mockSessoes', JSON.stringify(mockSessoes));
  }
};