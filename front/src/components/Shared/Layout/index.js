import React from "react";
import Header from "../Header";
import { useTheme } from '../../../context/ThemeContext';

const Layout = ({ children }) => {
  const { theme } = useTheme(); 

  return (
    <div data-theme={theme} className="app-container"> 
      <Header /> 
      
      <main style={{ 
          paddingTop: 'var(--height-xl)', 
          backgroundColor: 'var(--theme-background)',
          color: 'var(--theme-text)', 
          minHeight: '100vh', 
      }}> 
        {children}
      </main>
    </div>
  );
};

export default Layout;