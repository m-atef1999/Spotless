import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuthStore } from '../store/authStore';
import { useToast } from '../components/ui/Toast';

interface NotificationContextType {
    connection: signalR.HubConnection | null;
}

const NotificationContext = createContext<NotificationContextType>({ connection: null });

export const useNotification = () => useContext(NotificationContext);

export const NotificationProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const { token } = useAuthStore();
    const { addToast } = useToast();

    useEffect(() => {
        if (!token) {
            if (connection) {
                connection.stop();
                setConnection(null);
            }
            return;
        }

        const apiUrl = import.meta.env.VITE_API_URL || 'https://spotless.runasp.net';
        // Ensure no trailing slash
        const baseUrl = apiUrl.endsWith('/') ? apiUrl.slice(0, -1) : apiUrl;

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${baseUrl}/notificationHub`, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, [token]);

    useEffect(() => {
        if (connection) {
            const handleNotification = (data: any) => {
                console.log('Context: Notification received', data);
                const message = data.Message || data.message;


                if (message) {
                    addToast(message, 'success');
                }
            };

            connection.start()
                .then(() => {
                    console.log('SignalR Connected');
                    connection.on('ReceiveNotification', handleNotification);
                })
                .catch(err => console.error('SignalR Connection Error: ', err));

            return () => {
                connection.off('ReceiveNotification', handleNotification);
                connection.stop();
            };
        }
    }, [connection, addToast]);

    return (
        <NotificationContext.Provider value={{ connection }}>
            {children}
        </NotificationContext.Provider>
    );
};
