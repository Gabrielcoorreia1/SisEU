import api from '../api/httpClient';

function formatarData(isoString) {
  if (!isoString) return "N/A";
  const data = new Date(isoString);
  return data.toLocaleDateString('pt-BR'); 
}

function formatarHora(isoString) {
  if (!isoString) return "N/A";
  const data = new Date(isoString);
  return data.toLocaleTimeString('pt-BR'); 
}

export const RelatorioService = {
  
 GerarRelatorioCSV: async () => {

    let dadosRelatorio = [];
    
    try {
      const response = await api.get('/Checkin/relatorio');
      
      if (!response.data || response.data.length === 0) {
        alert("Nenhum dado de check-in real encontrado no banco de dados.");
        return;
      }
      
      dadosRelatorio = response.data.map(item => ({
        nome: item.nomeCompleto,
        cpf: item.cpf,
        email: item.email,
        pinUsado: item.pinUsado,
        dataCheckIn: formatarData(item.dataCheckin),
        horaCheckIn: formatarHora(item.dataCheckin),
        latitude: item.latitude,
        longitude: item.longitude,
      }));

    } catch (error) {
      console.error("Erro ao buscar relatório da API:", error);
      alert("Falha ao buscar dados reais do relatório. Verifique a API.");
      return;
    }
    
    let csvContent = "data:text/csv;charset=utf-8,";
    csvContent += "Nome,CPF,Email,PIN Utilizado,Data CheckIn,Hora CheckIn,Latitude,Longitude\r\n";
    
    dadosRelatorio.sort((a, b) => a.nome.localeCompare(b.nome));

    dadosRelatorio.forEach(linha => {
      csvContent += `"${linha.nome}","${linha.cpf}","${linha.email}",${linha.pinUsado},${linha.dataCheckIn},${linha.horaCheckIn},${linha.latitude},${linha.longitude}\r\n`;
    });
    
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    const nomeArquivo = `Relatorio_Global_Presenca_${new Date().toISOString().slice(0, 10)}.csv`;
    link.setAttribute("download", nomeArquivo);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },
  
  gerarRelatorioPresencaCSV: (sessaoId, tituloSessao = 'Sessao') => {
    console.warn("Função 'gerarRelatorioPresencaCSV' (por sessão) não está em uso.");
  }
};