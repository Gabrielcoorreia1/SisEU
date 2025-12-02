// src/features/sessions/hooks/useSessionForm.js
import { useState } from 'react';
import { SessionService } from '../../../api/modules/SessionService';

const INITIAL_STATE = {
    titulo: '',
    tipo: 'Apresentação oral', 
    local: '',
    horario: '',
    data: '',
    organizadores: [],
    avaliadores: [],  
    apresentacoes: [] 
};

export const useSessionForm = (initialData = INITIAL_STATE) => {
    const [formData, setFormData] = useState(initialData);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);

    const handleChange = (field, value) => {
        setFormData(prev => ({ ...prev, [field]: value }));
    };

    const handleSubmit = async () => {
        if (!formData.titulo || !formData.local) { 
            setError("Título e Local são obrigatórios.");
            return;
        }

        setLoading(true);
        setError(null);
        setSuccess(false);

        try {
            await SessionService.createSession(formData);
            setSuccess(true);
        } catch (err) {
            const apiError = err.response?.data?.erros || "Erro ao cadastrar a sessão. Verifique os dados.";
            setError(apiError);
        } finally {
            setLoading(false);
        }
    };

    const resetForm = () => setFormData(INITIAL_STATE);

    return {
        formData,
        loading,
        error,
        success,
        handleChange,
        handleSubmit,
        resetForm
    };
};