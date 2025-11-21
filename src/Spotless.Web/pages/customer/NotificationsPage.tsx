import { useState, useEffect } from 'react';
import { useToast } from '../../components/ui/Toast';
import { NotificationsService, type NotificationDto, NotificationType } from '../../lib/api';

export function NotificationsPage() {
    const [notifications, setNotifications] = useState<NotificationDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState<'all' | 'unread'>('all');
    const { addToast } = useToast();

    useEffect(() => {
        fetchNotifications();
    }, [filter]);

    const fetchNotifications = async () => {
        try {
            setLoading(true);
            const unreadOnly = filter === 'unread';
            const response = await NotificationsService.getApiNotifications({ unreadOnly });
            setNotifications(response || []);
        } catch (error) {
            addToast('Failed to load notifications', 'error');
            console.error('Error fetching notifications:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleMarkAsRead = async (id: string) => {
        try {
            await NotificationsService.putApiNotificationsRead({ id });
            setNotifications(notifications.map(n =>
                n.id === id ? { ...n, isRead: true } : n
            ));
            addToast('Notification marked as read', 'success');
        } catch {
            addToast('Failed to mark notification as read', 'error');
        }
    };

    const handleDelete = async (id: string) => {
        try {
            await NotificationsService.deleteApiNotifications({ id });
            setNotifications(notifications.filter(n => n.id !== id));
            addToast('Notification deleted', 'success');
        } catch {
            addToast('Failed to delete notification', 'error');
        }
    };

    const getNotificationIcon = (type?: number) => {
        // Handle both enum number and potential string response if any
        const typeValue = typeof type === 'string' ? parseInt(type) : type;

        switch (typeValue) {
            case NotificationType.OrderUpdate: // 0
                return '📦';
            case NotificationType.PaymentUpdate: // 1
                return '💳';
            case NotificationType.DriverAssignment: // 2
                return '🚚';
            case NotificationType.General: // 3
                return '🔔';
            default:
                return '📢';
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900">Notifications</h1>
                    <p className="text-gray-600 mt-2">Stay updated with your latest activities</p>
                </div>

                {/* Filter Tabs */}
                <div className="bg-white rounded-lg shadow-sm mb-6">
                    <div className="flex border-b border-gray-200">
                        <button
                            onClick={() => setFilter('all')}
                            className={`flex-1 px-6 py-4 text-sm font-medium ${filter === 'all'
                                ? 'text-blue-600 border-b-2 border-blue-600'
                                : 'text-gray-500 hover:text-gray-700'
                                }`}
                        >
                            All Notifications
                        </button>
                        <button
                            onClick={() => setFilter('unread')}
                            className={`flex-1 px-6 py-4 text-sm font-medium ${filter === 'unread'
                                ? 'text-blue-600 border-b-2 border-blue-600'
                                : 'text-gray-500 hover:text-gray-700'
                                }`}
                        >
                            Unread
                        </button>
                    </div>
                </div>

                {/* Notifications List */}
                <div className="space-y-4">
                    {loading ? (
                        <div className="flex items-center justify-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                        </div>
                    ) : notifications.length === 0 ? (
                        <div className="bg-white rounded-lg shadow-sm p-12 text-center">
                            <p className="text-gray-500 text-lg">No notifications found</p>
                        </div>
                    ) : (
                        notifications.map((notification) => (
                            <div
                                key={notification.id}
                                className={`bg-white rounded-lg shadow-sm p-6 ${!notification.isRead ? 'border-l-4 border-blue-600' : ''
                                    }`}
                            >
                                <div className="flex items-start gap-4">
                                    <div className="text-3xl">{getNotificationIcon(notification.type)}</div>
                                    <div className="flex-1">
                                        <div className="flex items-start justify-between">
                                            <div>
                                                <h3 className="text-lg font-semibold text-gray-900">
                                                    {notification.title}
                                                </h3>
                                                <p className="text-gray-600 mt-1">{notification.message}</p>
                                                <p className="text-sm text-gray-400 mt-2">
                                                    {notification.createdAt ? new Date(notification.createdAt).toLocaleString() : ''}
                                                </p>
                                            </div>
                                            <div className="flex gap-2">
                                                {!notification.isRead && notification.id && (
                                                    <button
                                                        onClick={() => handleMarkAsRead(notification.id!)}
                                                        className="text-blue-600 hover:text-blue-700 text-sm font-medium"
                                                    >
                                                        Mark as read
                                                    </button>
                                                )}
                                                {notification.id && (
                                                    <button
                                                        onClick={() => handleDelete(notification.id!)}
                                                        className="text-red-600 hover:text-red-700 text-sm font-medium"
                                                    >
                                                        Delete
                                                    </button>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </div>
        </div>
    );
}


