import "./Alert.css";
import Icon from "feather-icons-react";
import { useEffect, useRef } from "react";

const Alert = ({ message, show, animationKey }) => {
  const alertRef = useRef(null);

  useEffect(() => {
    if (show && alertRef.current) {
      const el = alertRef.current;

      el.classList.remove("shake");
      void el.offsetWidth;
      el.classList.add("shake");
    }
  }, [animationKey]);

  return (
    <div
      ref={alertRef}
      className="alert"
      style={{
        display: show ? "flex" : "none",
        visibility: show ? "visible" : "hidden",
        opacity: show ? 1 : 0,
        pointerEvents: show ? "auto" : "none",
        transition: "opacity 0.3s ease",
      }}
    >
      <Icon icon="alert-circle" />
      <h4>{message}</h4>
    </div>
  );
};

export default Alert;
