import React from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix for default marker icon missing in Leaflet with Webpack/Vite
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

const DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41],
});

L.Marker.prototype.options.icon = DefaultIcon;

interface MapProps {
    lat: number;
    lng: number;
    zoom?: number;
    height?: string;
    popupText?: string;
}

const ChangeView: React.FC<{ center: [number, number]; zoom: number }> = ({ center, zoom }) => {
    const map = useMap();
    map.setView(center, zoom);
    return null;
};

export const Map: React.FC<MapProps> = ({ lat, lng, zoom = 13, height = '300px', popupText }) => {
    return (
        <div className="rounded-xl overflow-hidden border border-slate-200 dark:border-slate-800 z-0 relative">
            <MapContainer center={[lat, lng]} zoom={zoom} style={{ height, width: '100%' }}>
                <ChangeView center={[lat, lng]} zoom={zoom} />
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                <Marker position={[lat, lng]}>
                    {popupText && <Popup>{popupText}</Popup>}
                </Marker>
            </MapContainer>
        </div>
    );
};
