// src/features/presence/hooks/useCameraPermission.js
import { useState, useEffect } from 'react';

export const useCameraPermission = (startCheck) => {
    const [permissionStatus, setPermissionStatus] = useState('pending');
    const [error, setError] = useState(null);
    const [stream, setStream] = useState(null);

    useEffect(() => {
        if (!startCheck) return;

        const checkPermission = async () => {
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                setError("Câmera não suportada neste navegador/dispositivo.");
                setPermissionStatus('error');
                return;
            }
            
            try {
                const videoStream = await navigator.mediaDevices.getUserMedia({ video: true });
                
                setStream(videoStream);
                setPermissionStatus('granted');
                
            } catch (err) {
                if (err.name === 'NotAllowedError' || err.name === 'PermissionDeniedError') {
                    setError("Permissão de câmera negada pelo usuário.");
                    setPermissionStatus('denied');
                } else {
                    setError(`Erro ao acessar câmera: ${err.name}`);
                    setPermissionStatus('error');
                }
            }
        };

        checkPermission();

        return () => {
            if (stream) {
                stream.getTracks().forEach(track => track.stop());
            }
        };
    }, [startCheck, stream]);

    return {
        permissionStatus,
        isGranted: permissionStatus === 'granted',
        error,
        stream, 
    };
};