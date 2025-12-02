// /home/fernanda/Sistema-Encontros-Universitarios/seu/src/components/Login/index.js

import { useState } from "react";
import Button from '../base/Button';
import CPF from "../base/TextFields/CPF/Index";
import "./Login.css";
import Senha from "../base/TextFields/Senha/Index";
import { useNavigate } from "react-router-dom";
import LogoSIEU from "../../Imagens/LogoSIEU.png";
import Alert from "../Shared/patterns/Alert/Index";
import Particles from "../External/Particles";

import { AuthService } from "../../api/modules/AuthService"; 

const Login = () => {
  const [cpf, setCpf] = useState("");
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState("");
  const [hasErro, setHasErro] = useState(false);
  const [erroKey, setErroKey] = useState(0);
  const [loading, setLoading] = useState(false); 
  const navigate = useNavigate();

  const submit = async (evento) => {
    evento.preventDefault();
    setLoading(true);
    setHasErro(false);
    const cpfWithoutCaracters = cpf.replace(/[^0-9]/g, "");

    console.log("[Login Component] Tentando fazer login com CPF:", cpfWithoutCaracters);

    try {
      const response = await AuthService.login(cpfWithoutCaracters, senha);
      console.log("[Login Component] Resposta da API recebida:", response);

     if (response && response.token && response.tipoUsuario !== undefined) { 

        const userRole = response.tipoUsuario;
        
        const userData = {
            id: response.usuarioId,
            nome: response.nomeCompleto,
            cpf: response.cpf,
            tipoUsuario: userRole
        };
        
        localStorage.setItem('currentUser', JSON.stringify(userData));
        localStorage.setItem('userRole', userRole);

        console.log("[Login Component] Login OK. Redirecionando:", userRole);

        if (userRole === ETipoUsuario.Admin) { 
          navigate("/AdmPage");
        } else {
          navigate("/dashboard");
        }
      } else {
        console.error("[Login Component] Resposta inválida da API:", response);
        setErroMessage("Resposta inesperada do servidor.");
      }

    } catch (error) {
      console.error("[Login Component] Erro durante a tentativa de login:", error);
      const apiErrorMessage = error.response?.data?.erros?.[0] || error.message;
      setErroMessage(apiErrorMessage || "Erro desconhecido ao logar.");
      
    } finally {
      setLoading(false);
    }
  };

  const setErroMessage = (errorMessage) => {
    setErro(errorMessage);
    setHasErro(true);
    setErroKey((prev) => prev + 1);
  };

  const cpfFormat = (valor) => {
    valor = valor.replace(/\D/g, "");
    valor = valor.substring(0, 11);
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d)/, "$1.$2");
    valor = valor.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    return valor;
  };

  const onChangeCPF = (valor) => {
    const formatedCPF = cpfFormat(valor);
    setCpf(formatedCPF);
  };

  return (
    <div style={{ width: "100vw", height: "99vh", position: "relative" }}>
      <Particles
        particleColors={["#ffffff", "#ffffff"]}
        particleCount={400}
        particleSpread={10}
        speed={0.2}
        particleBaseSize={200}
        moveParticlesOnHover={true}
        alphaParticles={false}
        disableRotation={true}
      />

      <section className="formulario">
        <form onSubmit={submit}>
          <img className="SisEuLogo" src={LogoSIEU} alt="Logo SIEU"></img>

          <h1>Encontros Universitários 2025</h1>
          <h4>Acesse sua conta para continuar</h4>

          <div className="inputs-login">
            <CPF
              obrigatorio={true}
              label="CPF"
              placeholder="Digite seu CPF"
              valor={cpf}
              aoAlterado={onChangeCPF}
              iconSize={"2rem"}
            />

            <Senha
              valor={senha}
              aoAlterado={(valor) => setSenha(valor)}
              iconSize={"2rem"}
            />
          </div>

          <div className="btnAndAlert">
            <Alert message={erro} show={hasErro} animationKey={erroKey} />

            <Button
              corPrimaria={"black"}
              corSecundaria={"#FFF"}
              text={loading ? "Carregando..." : "Entrar"} 
              disabled={loading} 
            ></Button>
          </div>
        </form>
      </section>
    </div>
  );
};

const ETipoUsuario = {
    Estudante: 1, 
    Professor: 2, 
    Avaliador: 3,
    Admin: 4     
};

export default Login;