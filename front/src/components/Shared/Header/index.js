import "./Header.css";
import LogoSIEU from "../../../Imagens/LogoSIEU.png";
import Icon from "feather-icons-react";
import { NavLink } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { useLocation } from "react-router-dom";
import { useTheme } from '../../../context/ThemeContext';

const Header = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { theme, toggleTheme } = useTheme();

  const routes = [
    { page: "Dashboard", path: "/dashboard", value: 0 },
    { page: "Home", path: "/AdmPage", value: 1 },
    { page: "Sobre", path: "/sobre", value: 2 },
    { page: "Configurações", path: "/configuracoes", value: 3 },
  ];

  useEffect(() => {
    const lastPage = localStorage.getItem("lastPage");

    if (lastPage) {
      const actualRoute = location.pathname;
      const actualRouteElement = routes.find(
        (route) => route.path === actualRoute
      );
      const lastRouteElement = routes.find((route) => route.path === lastPage);

      if (actualRouteElement && lastRouteElement) {
          const elementToActivate = document.querySelector(
            `.navBar a[href="${actualRoute}"]`
          );

          if (actualRouteElement.value < lastRouteElement.value)
            elementToActivate.classList.add("activeRight");
          else elementToActivate.classList.add("activeLeft");
      }
    }
  }, [location.pathname]); 

  const handleNavLinkClick = (event, path) => {
    event.preventDefault();

    if (path === location.pathname) return;

    localStorage.setItem("lastPage", location.pathname);

    const itemToAnimate = document.querySelector(".active");

    if (itemToAnimate) {
      const actualRouteElement = routes.find(
        (route) => route.path === location.pathname
      );
      const toElement = routes.find((route) => route.path === path);

      if (actualRouteElement && toElement) {
          const classToAdd =
            actualRouteElement.value < toElement.value
              ? "removingFromRight"
              : "removingFromLeft";

          itemToAnimate.classList.add(classToAdd);
          itemToAnimate.classList.add("removing");

          setTimeout(() => {
            itemToAnimate.classList.add(classToAdd);
            itemToAnimate.classList.remove("removing");

            navigate(path);
          }, 300);
      } else {
        navigate(path);
      }
    } else {
      navigate(path);
    }
  };
  
  const handleLogout = () => {
      localStorage.removeItem("authToken"); 
      localStorage.removeItem("lastPage");
      navigate("/");
  }

  return (
    <div className="header" data-theme={theme}>
      <div className="right">
        <div className="logo">
          <img className="logo" src={LogoSIEU} alt="Logo" />
        </div>

        <div className="title">
          <span className="firstWord">Encontros </span>{" "}
          <span className="secondWord">Universitários</span>
        </div>
      </div>
      <div className="titleMinifyed">
        <span>SIs</span>
        <span style={{ color: "var(--second-color)" }}>EUs</span>
      </div>
      
      <div className="left">
        
        <div className="header-icon-group">
          <Icon onClick={handleLogout} className="icon" icon="log-out" />
          <Icon 
            onClick={toggleTheme} 
            className="icon theme-toggle-btn" 
            icon={theme === 'light' ? 'moon' : 'sun'} 
            title={theme === 'light' ? 'Mudar para Escuro' : 'Mudar para Claro'}
          /> 
        </div>
      </div>
    </div>
  );
};

export default Header;