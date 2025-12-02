import "./CodigoCheckIn.css";
import { useRef } from "react";
import { useEffect } from "react";

const CodigoCheckIn = ({ length = 6, setCodigo, codigo = "" }) => {
  const inputsRef = useRef([]);

  const handleChange = (e, index) => {
    const value = e.target.value;

    const valores = inputsRef.current.map((input) => input.value || "");
    valores[index] = value;
    setCodigo(valores.join(""));

    if (value.length === 1) {
      const input = inputsRef.current[index];

      input.classList.remove("digitado");

      void input.offsetWidth;

      input.classList.add("digitado");

      if (index < length - 1) {
        inputsRef.current[index + 1].focus();
      }
    }
  };

  const handleKeyDown = (e, index) => {
    if (e.key === "Backspace" && !e.target.value && index > 0) {
      inputsRef.current[index - 1].focus();
    }
  };

  useEffect(() => {
    if (codigo) {
      codigo.split("").forEach((char, i) => {
        if (inputsRef.current[i]) {
          inputsRef.current[i].value = char;
        }
      });
    }
  }, [codigo]);

  return (
    <div className="codigo-input-container">
      {Array.from({ length }).map((_, index) => (
        <input
          key={index}
          type="text"
          maxLength={1}
          className="codigo-input"
          onChange={(e) => handleChange(e, index)}
          onKeyDown={(e) => handleKeyDown(e, index)}
          ref={(el) => (inputsRef.current[index] = el)}
        />
      ))}
    </div>
  );
};

export default CodigoCheckIn;
