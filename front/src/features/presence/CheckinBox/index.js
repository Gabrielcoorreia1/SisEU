import "./CheckinBox.css";

const CheckinBox = ({ setOpen }) => {
  return (
    <div className="checkin-box">
      <p className="checkin-text">
        Faça seu check-in para a creditação de horas complementares e/ou
        registro de presença:
      </p>
      <button className="checkin-button" onClick={() => setOpen(true)}>
        Check-in
      </button>
    </div>
  );
};

export default CheckinBox;
