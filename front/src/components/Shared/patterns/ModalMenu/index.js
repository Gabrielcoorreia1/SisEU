import "./ModalMenu.css";
import Icon from "feather-icons-react";

const ModalMenu = ({ openMenu, setOpenMenu }) => {
  window.addEventListener("resize", closeMenuModal);

  function closeMenuModal() {
    if (window.innerWidth > 768) {
      handleClose();
    }
  }

  const handleClose = () => {
    setOpenMenu(false);
  };

  return (
    <div
      className={`modal-overlay ${openMenu ? "show" : ""}`}
      onClick={handleClose}
    >
      <div className="modal-menu-content" onClick={(e) => e.stopPropagation()}>
        <ul>
          <li>
            <Icon className="icon" icon="home" />
            <a href="/paginaAdmin">Página Inicial</a>
          </li>
          <li>
            <Icon className="icon" icon="smile" />
            <a href="/sobre">Sobre</a>
          </li>
        </ul>
      </div>
    </div>
  );
};

export default ModalMenu;
