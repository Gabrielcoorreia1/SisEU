// src/features/presence/hooks/usePresenceStatus.js

import { useState, useEffect } from 'react';

export const PRESENCE_STATUS = {
    CHECKED_IN: 'CHECKED_IN',
    CHECKED_OUT: 'CHECKED_OUT',
    NO_STATUS: 'NO_STATUS',
};

export const usePresenceStatus = () => {
    const PRESENCE_KEY = 'user_presence_status'; 
    const getInitialStatus = () => localStorage.getItem(PRESENCE_KEY) || PRESENCE_STATUS.NO_STATUS;

    const [status, setStatus] = useState(getInitialStatus);
    
    const loading = false; 
    const markCheckedIn = () => {
        localStorage.setItem(PRESENCE_KEY, PRESENCE_STATUS.CHECKED_IN);
        setStatus(PRESENCE_STATUS.CHECKED_IN);
    };

    const markCheckedOut = () => {
        localStorage.setItem(PRESENCE_KEY, PRESENCE_STATUS.NO_STATUS);
        setStatus(PRESENCE_STATUS.NO_STATUS);
    };


    return {
        status,
        loading,
        isCheckedIn: status === PRESENCE_STATUS.CHECKED_IN,
        markCheckedIn,
        markCheckedOut,
    };
};