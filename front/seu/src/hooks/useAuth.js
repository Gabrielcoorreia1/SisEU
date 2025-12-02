// src/hooks/useAuth.js

import { useState, useEffect } from 'react';

export const useAuth = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userProfile, setUserProfile] = useState(null); 
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem('authToken'); 
    
    const userString = localStorage.getItem('currentUser');
    
    if (token && userString) {
      try {
        const user = JSON.parse(userString);
        
        setIsAuthenticated(true);
        setUserProfile(user.tipoUsuario); 
        
      } catch (e) {
        setIsAuthenticated(false);
        setUserProfile(null);
        localStorage.removeItem('authToken');
        localStorage.removeItem('currentUser');
      }
    } else {
      setIsAuthenticated(false);
      setUserProfile(null);
    }

    setIsReady(true);
  }, []);
  
  return { isAuthenticated, userProfile, isReady };
};