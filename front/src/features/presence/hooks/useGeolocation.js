// src/features/presence/hooks/useGeolocation.js
import { useState, useEffect } from 'react';

export const useGeolocation = (shouldStart) => {
  const [coords, setCoords] = useState(null);
  const [isPermitted, setIsPermitted] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    let watchId = null;

    if (shouldStart && navigator.geolocation) {
      const success = (position) => {
        setIsPermitted(true);
        setCoords({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
          accuracy: position.coords.accuracy
        });
        setError(null);
      };

      const failure = (err) => {
        setIsPermitted(false);
        setError(err.message || "Permissão de localização negada ou indisponível.");
        setCoords(null);
      };

      navigator.geolocation.getCurrentPosition(success, failure, {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 0,
      });

    } else if (shouldStart && !navigator.geolocation) {
      setError("Geolocalização não suportada pelo navegador.");
      setIsPermitted(false);
    }

    return () => {
      if (watchId !== null && navigator.geolocation.clearWatch) {
        navigator.geolocation.clearWatch(watchId);
      }
    };
  }, [shouldStart]);

  return { coords, isPermitted, error };
};
