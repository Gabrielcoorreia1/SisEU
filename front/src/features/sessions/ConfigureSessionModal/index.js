import "./ConfigureSessionModal.css";
import Icon from "feather-icons-react";
import Select from "react-select";
import { useEffect, useState } from "react";
import Button from '../../../components/base/Button';

import { SessionService } from "../../../api/modules/SessionService"; 


const ConfigureSessionModal = ({
  isOpen,
  onClose, 
  sessaoId,
  onChangeSessoes,
}) => {
  const eventType = [
    { label: "Nenhum", value: 0 },
    { label: "Pitch", value: 1 },
    { label: "Oral", value: 2 },
    { label: "Banner", value: 3 },
  ];

  const campusOptions = [
    { label: "Fortaleza", value: 0 },
    { label: "Crateus", value: 1 },
  ];

  function gerarCodigoAleatorio(tamanho = 6) {
    const caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    let codigo = "";
    for (let i = 0; i < tamanho; i++) {
      const indice = Math.floor(Math.random() * caracteres.length);
      codigo += caracteres[indice];
    }
    return codigo;
  }

  useEffect(() => {
    const carregarSessao = async () => {
      if (!isOpen) return;

      if (!sessaoId) {
        setSessao({
          titulo: "",
          tipo: "1",
          local: { campus: "0", departamento: "", bloco: "", sala: "" },
          data: "",
          horaInicio: "",
          horaFim: "",
          avaliadores: [""],
          apresentacoes: [{ Titulo: "", NomeAutor: "", NomeOrientador: "" }],
          codigoUnico: gerarCodigoAleatorio(),
        });
        return;
      }

      try {
        const dadosSessao = await SessionService.loadSession(sessaoId);
        const dataInicio = new Date(dadosSessao.dataInicio);
        const dataFim = new Date(dadosSessao.dataFim);

        setSessao({
          titulo: dadosSessao.titulo || "",
          tipo: dadosSessao.tipoEvento?.toString() || "1", 
          local: {
            campus: dadosSessao.local?.campus?.toString() || "0",
            departamento: dadosSessao.local?.departamento || "",
            bloco: dadosSessao.local?.bloco || "",
            sala: dadosSessao.local?.sala || "",
          },
          data: dataInicio.toISOString().split('T')[0] || "",
          horaInicio: dataInicio.toTimeString().split(' ')[0].substring(0, 5) || "",
          horaFim: dataFim.toTimeString().split(' ')[0].substring(0, 5) || "",
          avaliadores: dadosSessao.avaliadores?.map((a) => a || "") || [""], 
          apresentacoes: dadosSessao.apresentacoes?.map((apres) => ({
            Titulo: apres.titulo || "",
            NomeAutor: apres.nomeAutor || "",
            NomeOrientador: apres.nomeOrientador || "",
          })) || [{ Titulo: "", NomeAutor: "", NomeOrientador: "" }],
          codigoUnico: dadosSessao.codigoUnico || "",
        });
      } catch (error) {
        console.error("Erro ao carregar dados da sessão (API):", error);
      }
    };

    carregarSessao();
  }, [sessaoId, isOpen]);


  const [sessao, setSessao] = useState({
    titulo: "",
    tipo: "1",
    local: { campus: "0", departamento: "", bloco: "", sala: "" },
    horaInicio: "",
    horaFim: "",
    data: "",
    avaliadores: [""],
    apresentacoes: [{ Titulo: "", NomeAutor: "", NomeOrientador: "" }],
    codigoUnico: "",
  });

  if (!isOpen) return null;

  const handleChange = (e) => {
    const { name, value } = e.target;
    if (name.startsWith("local.")) {
      const localField = name.split(".")[1];
      setSessao((prev) => ({
        ...prev,
        local: {
          ...prev.local,
          [localField]: value,
        },
      }));
    } else {
      setSessao((prev) => ({ ...prev, [name]: value }));
    }
  };
  
  const handleSalvar = async () => {

    if (!sessao.titulo || sessao.titulo.trim() === "") {
        return alert("O título da sessão é obrigatório.");
    }
    if (!sessao.data || !sessao.horaInicio || !sessao.horaFim) {
        return alert("Data, Hora de Início e Hora de Fim são obrigatórias.");
    }

    const avaliadoresParaAPI = sessao.avaliadores.filter((a) => a.trim() !== "");
    const apresentacoesLimpa = sessao.apresentacoes.filter(apres => apres.Titulo && apres.Titulo.trim() !== '');
    
    let codigoUnicoParaEnvio = sessao.codigoUnico;
    if (!sessaoId && !codigoUnicoParaEnvio) {
        codigoUnicoParaEnvio = gerarCodigoAleatorio();
    } else if (!codigoUnicoParaEnvio) {
        codigoUnicoParaEnvio = gerarCodigoAleatorio();
    }

    const request = {
        Titulo: sessao.titulo,
        DataInicio: `${sessao.data}T${sessao.horaInicio}:00`,
        DataFim: `${sessao.data}T${sessao.horaFim}:00`,
        Local: {
          Campus: Number(sessao.local.campus),
          Departamento: sessao.local.departamento,
          Bloco: sessao.local.bloco,
          Sala: sessao.local.sala,
        },
        ETipoEvento: Number(sessao.tipo),
        Avaliadores: avaliadoresParaAPI,
        ImgUrl: "https://i.imgur.com/arsE55j.png",
        Apresentacoes: apresentacoesLimpa,
        CodigoUnico: codigoUnicoParaEnvio,
    };

    if (sessaoId) {
      request.Id = sessaoId;
    }

    
    try {
        console.log("Enviando para a API C#...", request);
        
        if (!sessaoId) {
            await SessionService.createSession(request);
        } else {
            await SessionService.updateSession(sessaoId, request);
        }

        onChangeSessoes();

    } catch (error) {
        console.error(
          "Erro ao salvar evento (API):",
          error.response?.data || error.message
        );
        alert("Erro ao salvar: " + (error.response?.data?.erros?.[0] || error.message || error.message));
    }
  };

  const handleExcluir = async () => {
    try {
      await SessionService.deleteSession(sessaoId);
      onChangeSessoes();
    } catch (error) {
      console.error(
        "Erro ao excluir evento (API):",
        error.response?.data || error.message
      );
    }
  };

  return (
    <div
      className="modal-sessao-overlay"
      onClick={() => {
        onClose();
      }}
    >
      <div 
        className="modal-sessao" 
        onClick={(e) => e.stopPropagation()}
        style={{ display: 'flex', flexDirection: 'column' }}
      >
        <div className="x-icon" >
          <Icon
            onClick={() => {
              onClose();
            }}
            icon="x"
            size={"40px"}
            className="x-icon"
          />
        </div>
        <h2>{!sessaoId ? "Adicionar Nova" : "Editar"} Sessão</h2>


        <div style={{ flexGrow: 1, overflowY: 'auto', paddingRight: '15px' }}>
            
            <div className="linha-dupla">
            <div className="campo campo-restante">
                <label>Título</label>
                <input
                type="text"
                name="titulo"
                placeholder="Título da Sessão"
                value={sessao.titulo}
                onChange={handleChange}
                />
            </div>
            <div className="campo campo-pequeno">
                <label>Tipo</label>
                <select name="tipo" value={sessao.tipo} onChange={handleChange}>
                {eventType.map((tipo) => (
                    <option key={tipo.value} value={tipo.value}>
                    {tipo.label}
                    </option>
                ))}
                </select>
            </div>
            </div>

            <h3>Local</h3>
            <div className="linha-dupla">
            <div className="campo campo-restante">
                <label>Departamento</label>
                <input
                type="text"
                name="local.departamento"
                placeholder="Departamento"
                value={sessao.local?.departamento || ''}
                onChange={handleChange}
                />
            </div>
            <div className="campo campo-pequeno">
                <label>Campus</label>
                <select
                name="local.campus"
                value={sessao.local?.campus || '0'}
                onChange={handleChange}
                >
                {campusOptions.map((campus) => (
                    <option key={campus.value} value={campus.value}>
                    {campus.label}
                    </option>
                ))}
                </select>
            </div>
            </div>
            <div className="linha-dupla">
            <div className="campo">
                <label>Bloco</label>
                <input
                type="text"
                name="local.bloco"
                placeholder="Bloco"
                value={sessao.local?.bloco || ''}
                onChange={handleChange}
                />
            </div>
            <div className="campo">
                <label>Sala</label>
                <input
                type="text"
                name="local.sala"
                placeholder="Sala"
                value={sessao.local?.sala || ''}
                onChange={handleChange}
                />
            </div>
            </div>

            <h3 style={{ marginTop: "20px", marginBottom: "10px" }}>Horários</h3>
            <div className="linha-dupla"> 
            <div className="campo">
                <label>Data</label>
                <input type="date" name="data" value={sessao.data} onChange={handleChange} />
            </div>
            <div className="campo">
                <label>Hora Início</label>
                <input type="time" name="horaInicio" value={sessao.horaInicio} onChange={handleChange} />
            </div>
            <div className="campo">
                <label>Hora Fim</label>
                <input type="time" name="horaFim" value={sessao.horaFim} onChange={handleChange} />
            </div>
            </div>

            <h3 style={{ marginTop: "20px", marginBottom: "10px" }}>Participantes e Avaliação</h3>

            <label style={{ display: "flex", alignItems: "center", gap: "8px" }}>
            Avaliadores
            <button
                type="button"
                onClick={() =>
                setSessao((prev) => ({
                    ...prev,
                    avaliadores: [...prev.avaliadores, ""],
                }))
                }
                className="btn-adicionar-avaliador"
            >
                +
            </button>
            </label>
            {sessao.avaliadores.map((avaliador, index) => (
            <div
                key={index}
                style={{
                display: "flex",
                alignItems: "center",
                gap: "8px",
                marginBottom: "8px",
                }}
            >
                <input
                type="text"
                placeholder="Nome do Avaliador e instituição"
                value={avaliador}
                onChange={(e) => {
                    const novosAvaliadores = [...sessao.avaliadores];
                    novosAvaliadores[index] = e.target.value;
                    setSessao((prev) => ({
                    ...prev,
                    avaliadores: novosAvaliadores,
                    }));
                }}
                />
                {index > 0 && (
                <button
                    type="button"
                    onClick={() => {
                    setSessao((prev) => ({
                        ...prev,
                        avaliadores: prev.avaliadores.filter((_, i) => i !== index),
                    }));
                    }}
                    className="btn-remover-avaliador"
                >
                    -
                </button>
                )}
            </div>
            ))}


            <h3 style={{ marginTop: "20px", marginBottom: "10px" }}>Apresentações</h3>
            <label style={{ display: "flex", alignItems: "center", gap: "8px" }}>
            Adicionar Apresentação
            <button
                type="button"
                onClick={() =>
                setSessao((prev) => ({
                    ...prev,
                    apresentacoes: [ 
                    ...prev.apresentacoes,
                    { Titulo: "", NomeAutor: "", NomeOrientador: "" },
                    ],
                }))
                }
                className="btn-adicionar-avaliador"
            >
                +
            </button>
            </label>
            {sessao.apresentacoes.map((apres, index) => (
            <div
                key={index}
                style={{
                display: "flex",
                flexDirection: "column",
                gap: "10px",
                marginBottom: "15px",
                padding: "25px",
                border: "1px solid #ccc",
                borderRadius: "8px",
                }}
            >
                <label style={{fontWeight: 'bold'}}>Apresentação {index + 1}:</label>
                <input
                type="text"
                placeholder="Título da Apresentação"
                value={apres.Titulo}
                onChange={(e) => {
                    const novas = [...sessao.apresentacoes];
                    novas[index].Titulo = e.target.value;
                    setSessao((prev) => ({ ...prev, apresentacoes: novas }));
                }}
                />
                <input
                type="text"
                placeholder="Nome do autor"
                value={apres.NomeAutor}
                onChange={(e) => {
                    const novas = [...sessao.apresentacoes];
                    novas[index].NomeAutor = e.target.value;
                    setSessao((prev) => ({ ...prev, apresentacoes: novas }));
                }}
                />
                <input
                type="text"
                placeholder="Professor orientador"
                value={apres.NomeOrientador}
                onChange={(e) => {
                    const novas = [...sessao.apresentacoes];
                    novas[index].NomeOrientador = e.target.value;
                    setSessao((prev) => ({ ...prev, apresentacoes: novas }));
                }}
                />
                {index > 0 && (
                <button
                    type="button"
                    onClick={() => {
                    setSessao((prev) => ({
                        ...prev,
                        apresentacoes: prev.apresentacoes.filter(
                        (_, i) => i !== index
                        ),
                    }));
                    }}
                    className="btn-remover-avaliador"
                    style={{ width: "fit-content", marginTop: "4px", alignSelf: 'flex-end' }}
                >
                    Remover
                </button>
                )}
            </div>
            ))}
        </div>
        <div 
            style={{ 
                paddingTop: '20px', 
                marginTop: '20px', 
                borderTop: '1px solid black', 
                display: 'flex', 
                flexDirection: 'column', 
                gap: '10px',
                backgroundColor: 'white', 
                position: 'sticky', 
                bottom: 0,
                zIndex: 100 
            }}
        >
            <div className="modal-sessao-rodape">
              <div className="rodape-left" style={{flex: 1}} />
              <div className="rodape-buttons">
                <Button
                  className="botao-salvar-custom"
                  corPrimaria={"#28a745"}
                  corSecundaria={"#FFF"}
                  onClick={handleSalvar}
                  text={"Salvar"}
                />
             <Button
                className="botao-excluir-custom"
                corPrimaria={"#DA370F"}
                corSecundaria={"#FFF"}
                onClick={async () => { 
                  if (sessaoId) {
                    const confirmDelete = window.confirm("Tem certeza que deseja excluir esta sessão?");
                    if (!confirmDelete) return;
                    await handleExcluir(); 
                  } else {
                    onClose();
                  }
                }}
                text={sessaoId ? "Excluir" : "Cancelar"}
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ConfigureSessionModal;