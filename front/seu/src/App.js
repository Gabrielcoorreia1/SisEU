import { Routes, Route } from "react-router-dom";
import Login from "./components/Login/index";
import AdmPage from "./pages/AdmPage";
import React from "react";
import Sobre from "./pages/Sobre";
import Configuracoes from "./pages/Configuracoes";
import DashboardPage from './pages/DashboardPage'; 
import PrivateRoute from './components/Shared/PrivateRoute'; 
import CheckInByPinPage from './pages/CheckInByPinPage';
import CheckInByGeolocationPage from './pages/CheckInByGeolocationPage';
import CheckInByQrPage from './pages/CheckInByQrPage';

function App() {
  return (
    <Routes>
      <Route path="/" element={<Login/>} />
      
      <Route 
        path="/AdmPage" 
        element={<AdmPage />}
      />
      <Route 
        path="/dashboard" 
       element={<DashboardPage />} 
      /> 
      
      <Route 
        path="/configuracoes" 
        element={<PrivateRoute element={<Configuracoes />} />} 
      />
      <Route 
        path="/checkin/pin" 
        element={<CheckInByPinPage />} 
      />
      <Route 
        path="/checkin/qr" 
        element={<CheckInByQrPage />} 
      />
      <Route 
        path="/checkin/geolocation" 
        element = {<CheckInByGeolocationPage />}
      />

    </Routes>
  );
}

export default App;