// src/service/MockSessionService.js
import { mockSessoes as sessoesIniciais } from "../api/mockData";

const SESSION_KEY = 'mockSessoes';

function formatTime(isoString) {
  if (!isoString || !isoString.includes('T')) return '00:00';
  const data = new Date(isoString);
  const horas = data.getUTCHours().toString().padStart(2, '0');
  const minutos = data.getUTCMinutes().toString().padStart(2, '0');
  return `${horas}:${minutos}`;
}

function formatDate(isoString) {
  if (!isoString || !isoString.includes('T')) return 'dd/mm/aaaa';

  const data = new Date(isoString);
  return data.toLocaleDateString('pt-BR', { timeZone: 'UTC' });
}

const eventTypeMap = { 0: "Nenhum", 1: "Pitch", 2: "Oral", 3: "Banner" };

const getSessions = () => {
  try {
    const sessoes = localStorage.getItem(SESSION_KEY);
    return sessoes ? JSON.parse(sessoes) : sessoesIniciais; 
  } catch (e) {
    return sessoesIniciais;
  }
};

const saveSessions = (sessoes) => {
  localStorage.setItem(SESSION_KEY, JSON.stringify(sessoes));
};

const generateId = () => {
  return `sessao-${Math.random().toString(36).substr(2, 9)}`;
};

export const MockSessionService = {
  
  loadAllSessions: () => {
    return getSessions();
  },

  loadSession: (sessaoId) => {
    const sessoes = getSessions();
    const sessao = sessoes.find(s => s.id === sessaoId);
    
    if (sessao) {
        const dataInicio = new Date(sessao.dataInicio); 
        const dataFim = new Date(sessao.dataFim);
        
        return {
            ...sessao,
            tipo: sessao.eventType?.toString() || "1",
            data: dataInicio.toISOString().split('T')[0], 
            horaInicio: formatTime(sessao.dataInicio), 
            horaFim: formatTime(sessao.dataFim), 
        };
    }
    return null;
  },

  createSession: (request) => { 
    const sessoes = getSessions();

    const novaSessao = {
        id: generateId(),

        titulo: request.Titulo,
        tema: request.Titulo, 
        tipoApresentacao: eventTypeMap[request.ETipoEvento] || "Indefinido",
        sala: request.Local.Sala,
        bloco: request.Local.Bloco,
        horarioInicio: formatTime(request.DataInicio),
        horarioFim: formatTime(request.DataFim),
        data: formatDate(request.DataInicio),

        eventType: request.ETipoEvento,
        dataInicio: request.DataInicio, 
        dataFim: request.DataFim,     
        local: request.Local,
        avaliadores: request.Avaliadores,
        organizadores: request.Organizadores,
        apresentacoes: request.Apresentacoes,
        imgUrl: request.ImgUrl || "/Imagens/brasao-ufc.png",
        codigoUnico: request.CodigoUnico
    };
    
    const novasSessoes = [...sessoes, novaSessao];
    saveSessions(novasSessoes);
    console.log("MOCK: Sessão criada (Formato Card)", novaSessao);
  },

  updateSession: (sessaoId, request) => { 
    let sessoes = getSessions();

    const sessaoAtualizada = {
        id: sessaoId,

        titulo: request.Titulo,
        tema: request.Titulo,
        tipoApresentacao: eventTypeMap[request.ETipoEvento] || "Indefinido",
        sala: request.Local.Sala,
        bloco: request.Local.Bloco,
        horarioInicio: formatTime(request.DataInicio),
        horarioFim: formatTime(request.DataFim),
        data: formatDate(request.DataInicio),

        eventType: request.ETipoEvento,
        dataInicio: request.DataInicio,
        dataFim: request.DataFim,
        local: request.Local,
        avaliadores: request.Avaliadores,
        organizadores: request.Organizadores,
        apresentacoes: request.Apresentacoes,
        imgUrl: request.ImgUrl || "/Imagens/brasao-ufc.png",
        codigoUnico: request.CodigoUnico
    };
    
    sessoes = sessoes.map(s => (s.id === sessaoId ? sessaoAtualizada : s));
    saveSessions(sessoes);
    console.log("MOCK: Sessão atualizada (Formato Card)", sessaoAtualizada);
  },

  deleteSession: (sessaoId) => {
    let sessoes = getSessions();
    sessoes = sessoes.filter(s => s.id !== sessaoId);
    saveSessions(sessoes);
    console.log("MOCK: Sessão deletada", sessaoId);
  }
};