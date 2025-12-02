import './Mapa.css'
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
    iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
    iconUrl: require('leaflet/dist/images/marker-icon.png'),
    shadowUrl: require('leaflet/dist/images/marker-shadow.png'),
});

const Mapa = ({ latitude, longitude }) => {   
    return (
        <div className="confirmar-localizacao-container">

            <div className="mapa-container">
                <MapContainer center={[latitude, longitude]} zoom={50} scrollWheelZoom={true} zoomControl={false} style={{ height: "100%", width: "100%" }}>
                    <TileLayer
                        attribution='&copy; <a href="https://osm.org/copyright">OpenStreetMap</a>'
                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                    <Marker position={[latitude, longitude]}>
                        <Popup>Você está aqui</Popup>
                    </Marker>
                </MapContainer>
            </div>
        </div>
    );
}

export default Mapa