import "./Button.css";

const Button = ({ onClick, boxShadow, corPrimaria, corSecundaria, text }) => {
  return (
    <div className="divBotao">
      <button
        onClick={onClick}
        className={`botao ${boxShadow ? "com-sombra" : ""}`}
        style={{
          backgroundColor: corPrimaria,
          color: corSecundaria,
        }}
      >
        {text}
      </button>
    </div>
  );
};

export default Button;
