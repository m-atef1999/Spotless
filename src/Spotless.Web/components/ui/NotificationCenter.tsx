import React, { useState, useEffect } from 'react';
import { Bell } from 'lucide-react';
import { useNotification } from '../../contexts/NotificationContext';
import { motion, AnimatePresence } from 'framer-motion';

interface Notification {
    id: string;
    title: string;
    message: string;
    timestamp: Date;
    read: boolean;
}

export const NotificationCenter: React.FC = () => {
    const { connection } = useNotification();
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [isOpen, setIsOpen] = useState(false);
    const [unreadCount, setUnreadCount] = useState(0);

    useEffect(() => {
        if (connection) {
            connection.on('ReceiveNotification', (data: { Title: string, Message: string, Timestamp: string }) => {
                const newNotification: Notification = {
                    id: Math.random().toString(36).substr(2, 9),
                    title: data.Title,
                    message: data.Message,
                    timestamp: new Date(data.Timestamp || Date.now()),
                    read: false
                };
                setNotifications(prev => [newNotification, ...prev]);
                setUnreadCount(prev => prev + 1);
            });
        }
    }, [connection]);

    const toggleOpen = () => {
        setIsOpen(!isOpen);
        if (!isOpen) {
            // Mark all as read when opening (simplified logic)
            setUnreadCount(0);
            setNotifications(prev => prev.map(n => ({ ...n, read: true })));
        }
    };

    return (
        <div className="relative">
            <button
                onClick={toggleOpen}
                className="relative p-2 rounded-full hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors"
            >
                <Bell className="w-6 h-6 text-slate-600 dark:text-slate-300" />
                {unreadCount > 0 && (
                    <span className="absolute top-0 right-0 w-5 h-5 bg-red-500 text-white text-xs font-bold rounded-full flex items-center justify-center">
                        {unreadCount}
                    </span>
                )}
            </button>

            <AnimatePresence>
                {isOpen && (
                    <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, y: 10 }}
                        className="absolute right-0 mt-2 w-80 bg-white dark:bg-slate-900 rounded-xl shadow-lg border border-slate-100 dark:border-slate-800 overflow-hidden z-50"
                    >
                        <div className="p-4 border-b border-slate-100 dark:border-slate-800">
                            <h3 className="font-semibold text-slate-900 dark:text-white">Notifications</h3>
                        </div>
                        <div className="max-h-96 overflow-y-auto">
                            {notifications.length === 0 ? (
                                <div className="p-8 text-center text-slate-500 dark:text-slate-400">
                                    No notifications
                                </div>
                            ) : (
                                <div className="divide-y divide-slate-100 dark:divide-slate-800">
                                    {notifications.map(notification => (
                                        <div key={notification.id} className="p-4 hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <h4 className="font-medium text-slate-900 dark:text-white text-sm">{notification.title}</h4>
                                            <p className="text-slate-500 dark:text-slate-400 text-xs mt-1">{notification.message}</p>
                                            <span className="text-xs text-slate-400 mt-2 block">
                                                {notification.timestamp.toLocaleTimeString()}
                                            </span>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};
