// src/components/Shared/PrivateRoute.js

import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
const PrivateRoute = ({ element, requiredRole }) => {
  const { isAuthenticated, userProfile, isReady } = useAuth();
  
  if (!isReady) {
    return <div>Carregando...</div>; 
  }

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  if (requiredRole && userProfile !== requiredRole) {
    return <Navigate to="/dashboard" replace />;
  }
  return element;
};

export default PrivateRoute;