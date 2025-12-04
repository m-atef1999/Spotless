import React, { useState, useEffect } from 'react';
import { Navigation, AlertCircle, Loader2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { DriversService } from '../../lib/api';
import { Map } from '../../components/ui/Map';

export const LocationPage: React.FC = () => {
    const [isUpdating, setIsUpdating] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [location, setLocation] = useState<{ lat: number; lng: number } | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    // Fetch location from driver profile on mount
    useEffect(() => {
        const fetchLocation = async () => {
            try {
                const profile = await DriversService.getApiDriversProfile();
                if (profile.currentLocation?.latitude && profile.currentLocation?.longitude) {
                    setLocation({
                        lat: profile.currentLocation.latitude,
                        lng: profile.currentLocation.longitude
                    });
                } else {
                    // Fallback to localStorage if not in profile
                    const saved = localStorage.getItem('driverLocation');
                    if (saved) {
                        setLocation(JSON.parse(saved));
                    }
                }
            } catch (err) {
                console.error('Failed to fetch driver profile', err);
                // Fallback to localStorage
                const saved = localStorage.getItem('driverLocation');
                if (saved) {
                    setLocation(JSON.parse(saved));
                }
            } finally {
                setIsLoading(false);
            }
        };
        fetchLocation();
    }, []);

    const getCurrentLocation = () => {
        if (!navigator.geolocation) {
            setError('Geolocation is not supported by your browser');
            return;
        }

        setIsUpdating(true);
        setError(null);
        setSuccessMessage(null);

        navigator.geolocation.getCurrentPosition(
            async (position) => {
                const { latitude, longitude } = position.coords;
                const newLocation = { lat: latitude, lng: longitude };
                setLocation(newLocation);
                localStorage.setItem('driverLocation', JSON.stringify(newLocation));

                try {
                    await DriversService.putApiDriversLocation({
                        requestBody: {
                            latitude,
                            longitude,
                        }
                    });
                    setSuccessMessage('Location updated successfully!');
                } catch (err) {
                    console.error('Failed to update location', err);
                    setError('Failed to update location on server.');
                } finally {
                    setIsUpdating(false);
                }
            },
            (err) => {
                console.error('Geolocation error', err);
                setError('Failed to get current location. Please enable location services.');
                setIsUpdating(false);
            }
        );
    };

    if (isLoading) {
        return (
            <DashboardLayout role="Driver">
                <div className="flex items-center justify-center h-96">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Driver">
            <div className="max-w-2xl mx-auto space-y-8">
                <div className="text-center">
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white flex items-center justify-center gap-3">
                        <Navigation className="w-8 h-8 text-cyan-500" />
                        Location & Tracking
                    </h1>
                    <p className="text-slate-500 dark:text-slate-400 mt-2">
                        Update your current location to receive nearby job offers.
                    </p>
                </div>

                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 text-center">
                    {location && (
                        <div className="mb-6">
                            <Map
                                lat={location.lat}
                                lng={location.lng}
                                popupText="You are here"
                                height="300px"
                            />
                            <div className="mt-4 flex justify-center gap-8 text-slate-600 dark:text-slate-300 font-mono">
                                <div>
                                    <span className="text-xs text-slate-400 block uppercase tracking-wider">Latitude</span>
                                    {location.lat.toFixed(6)}
                                </div>
                                <div>
                                    <span className="text-xs text-slate-400 block uppercase tracking-wider">Longitude</span>
                                    {location.lng.toFixed(6)}
                                </div>
                            </div>
                            <p className="text-xs text-slate-400 mt-2">
                                Last updated: {new Date().toLocaleTimeString()}
                            </p>
                        </div>
                    )}

                    {!location && (
                        <div className="mb-8 text-slate-500 dark:text-slate-400 py-12 bg-slate-50 dark:bg-slate-800/50 rounded-lg">
                            Location not yet updated.
                        </div>
                    )}

                    {error && (
                        <div className="mb-6 p-4 bg-red-50 text-red-700 rounded-lg border border-red-100 flex items-center justify-center gap-2">
                            <AlertCircle className="w-5 h-5" />
                            {error}
                        </div>
                    )}

                    {successMessage && (
                        <div className="mb-6 p-4 bg-green-50 text-green-700 rounded-lg border border-green-100">
                            {successMessage}
                        </div>
                    )}

                    <Button
                        onClick={getCurrentLocation}
                        isLoading={isUpdating}
                        className="w-full sm:w-auto min-w-[200px] bg-cyan-600 hover:bg-cyan-700 text-white"
                    >
                        <Navigation className="w-4 h-4 mr-2" />
                        {location ? 'Update Location' : 'Share My Location'}
                    </Button>
                </div>
            </div>
        </DashboardLayout>
    );
};
