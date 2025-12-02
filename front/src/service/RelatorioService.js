// src/service/RelatorioService.js
import api from '../api/httpClient';

const MOCK_TOKEN = "MOCK_ADMIN_TOKEN";
const formatReportData = (data) => {
    const headers = [
        "Nome Completo", "CPF", "Email", "Matricula", 
        "Data Checkin", "Hora Checkin", "Data Checkout", "Hora Checkout", 
        "PIN Usado", "Latitude", "Longitude"
    ];

    const rows = data.map(item => [
        `"${item.nomeCompleto || ''}"`,
        `"${item.cpf || ''}"`,
        `"${item.email || ''}"`,
        `"${item.matricula || ''}"`, 
        `"${item.dataCheckin || ''}"`,
        `"${item.horaCheckin || ''}"`,
        `"${item.dataCheckout || ''}"`, 
        `"${item.horaCheckout || ''}"`, 
        `"${item.pinUsado || ''}"`,
        `"${item.latitude || ''}"`,
        `"${item.longitude || ''}"`,
    ]);

    return [headers.join(','), ...rows.map(row => row.join(','))].join('\n');
};

export const RelatorioService = {

    generateReport: async () => {
        const token = localStorage.getItem('authToken') || MOCK_TOKEN;

        try {
            const response = await api.get('/Checkin/relatorio', {
                headers: {
                    Authorization: `Bearer ${token}`, 
                },
            });
            
            const csvString = formatReportData(response.data);
            
            return new Blob([csvString], { type: 'text/csv;charset=utf-8;' });

        } catch (error) {
            console.error("Erro ao gerar relatório real:", error);
            throw new Error("Falha ao buscar dados reais do relatório. Verifique a API.");
        }
    },
};