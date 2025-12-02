// src/features/presence/hooks/usePresenceStatus.js (CORRIGIDO)
import { useState, useEffect } from 'react';

export const PRESENCE_STATUS = {
    CHECKED_IN: 'CHECKED_IN',
    CHECKED_OUT: 'CHECKED_OUT',
    NO_STATUS: 'NO_STATUS',
};

export const usePresenceStatus = () => {
    const [status, setStatus] = useState(PRESENCE_STATUS.NO_STATUS);
    const [loading, setLoading] = useState(true);

    const PRESENCE_KEY = 'user_presence_status'; 

    useEffect(() => {
        const checkStatus = () => {
            setLoading(true);
            const currentStatus = localStorage.getItem(PRESENCE_KEY) || PRESENCE_STATUS.NO_STATUS;
            setStatus(currentStatus);
            setLoading(false);
        };
        checkStatus();
    }, []);

    const markCheckedIn = () => {
        localStorage.setItem(PRESENCE_KEY, PRESENCE_STATUS.CHECKED_IN);
        setStatus(PRESENCE_STATUS.CHECKED_IN);
    };

    const markCheckedOut = () => {
        localStorage.setItem(PRESENCE_KEY, PRESENCE_STATUS.CHECKED_OUT);
        setStatus(PRESENCE_STATUS.CHECKED_OUT);
    };

    return {
        status,
        loading,
        isLoggedIn: status === PRESENCE_STATUS.CHECKED_IN,
        markCheckedIn,
        markCheckedOut,
    };
};