import React, { useState, useEffect } from 'react';
import { Bell, Check, Trash2, RefreshCw } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { NotificationsService, type NotificationDto } from '../../lib/api';
import { NotificationType } from '../../lib/constants';

export const NotificationsPage: React.FC = () => {
    const [notifications, setNotifications] = useState<NotificationDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState<'all' | 'unread'>('all');

    useEffect(() => {
        fetchNotifications();
    }, [filter]);

    const fetchNotifications = async () => {
        setLoading(true);
        try {
            const data = await NotificationsService.getApiNotifications({
                unreadOnly: filter === 'unread'
            });
            setNotifications(data);
        } catch (error) {
            console.error('Failed to fetch notifications:', error);
        } finally {
            setLoading(false);
        }
    };

    const markAsRead = async (id: string) => {
        try {
            await NotificationsService.putApiNotificationsRead({ id });
            setNotifications(prev =>
                prev.map(n => n.id === id ? { ...n, isRead: true } : n)
            );
        } catch (error) {
            console.error('Failed to mark as read:', error);
        }
    };

    const deleteNotification = async (id: string) => {
        try {
            await NotificationsService.deleteApiNotifications({ id });
            setNotifications(prev => prev.filter(n => n.id !== id));
        } catch (error) {
            console.error('Failed to delete notification:', error);
        }
    };

    const getNotificationTypeColor = (type: number | undefined) => {
        switch (type) {
            case NotificationType.OrderCreated:
            case NotificationType.OrderConfirmed:
            case NotificationType.OrderInProgress:
            case NotificationType.OrderCompleted:
            case NotificationType.OrderAssigned:
                return 'bg-blue-100 text-blue-800 border-blue-200';
            case NotificationType.OrderCancelled:
            case NotificationType.PaymentFailed:
            case NotificationType.DriverApplicationRejected:
                return 'bg-red-100 text-red-800 border-red-200';
            case NotificationType.PaymentReceived:
            case NotificationType.DriverApplicationApproved:
                return 'bg-green-100 text-green-800 border-green-200';
            case NotificationType.System:
            case NotificationType.Promotion:
                return 'bg-gray-100 text-gray-800 border-gray-200';
            default: return 'bg-gray-100 text-gray-800 border-gray-200';
        }
    };

    const getNotificationTypeLabel = (type: number | undefined) => {
        switch (type) {
            case NotificationType.OrderCreated: return 'Order Created';
            case NotificationType.OrderConfirmed: return 'Order Confirmed';
            case NotificationType.OrderAssigned: return 'Order Assigned';
            case NotificationType.OrderInProgress: return 'Order In Progress';
            case NotificationType.OrderCompleted: return 'Order Completed';
            case NotificationType.OrderCancelled: return 'Order Cancelled';
            case NotificationType.PaymentReceived: return 'Payment Received';
            case NotificationType.PaymentFailed: return 'Payment Failed';
            case NotificationType.DriverApplicationApproved: return 'Driver Approved';
            case NotificationType.DriverApplicationRejected: return 'Driver Rejected';
            case NotificationType.System: return 'System';
            case NotificationType.Promotion: return 'Promotion';
            default: return 'Notification';
        }
    };

    return (
        <DashboardLayout role="Admin">
            <div className="p-6">
                <div className="flex justify-between items-center mb-6">
                    <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
                        <Bell className="w-8 h-8 text-cyan-500" />
                        Notifications
                    </h1>
                    <div className="flex gap-2">
                        <Button
                            onClick={fetchNotifications}
                            variant="secondary"
                        >
                            <RefreshCw className="w-4 h-4 mr-2" />
                            Refresh
                        </Button>
                        <Button
                            onClick={() => setFilter('all')}
                            variant={filter === 'all' ? 'primary' : 'secondary'}
                        >
                            All
                        </Button>
                        <Button
                            onClick={() => setFilter('unread')}
                            variant={filter === 'unread' ? 'primary' : 'secondary'}
                        >
                            Unread
                        </Button>
                    </div>
                </div>

                {loading ? (
                    <div className="flex items-center justify-center py-12">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-600"></div>
                    </div>
                ) : notifications.length === 0 ? (
                    <div className="text-center py-12 bg-white rounded-lg border border-gray-200">
                        <Bell className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                        <p className="text-gray-500 text-lg">No notifications found.</p>
                    </div>
                ) : (
                    <div className="space-y-4">
                        {notifications.map(notification => (
                            <div
                                key={notification.id}
                                className={`p-4 rounded-lg border-2 transition-all ${notification.isRead
                                    ? 'bg-white border-gray-200'
                                    : 'bg-cyan-50 border-cyan-300 shadow-md'
                                    }`}
                            >
                                <div className="flex justify-between items-start gap-4">
                                    <div className="flex-1">
                                        <div className="flex items-center gap-2 mb-2">
                                            <span className={`px-2 py-1 rounded text-xs font-semibold ${getNotificationTypeColor(notification.type)}`}>
                                                {getNotificationTypeLabel(notification.type)}
                                            </span>
                                            {!notification.isRead && (
                                                <span className="px-2 py-1 rounded bg-cyan-500 text-white text-xs font-semibold">
                                                    NEW
                                                </span>
                                            )}
                                        </div>
                                        <h3 className="font-semibold text-gray-900 text-lg">
                                            {notification.title}
                                        </h3>
                                        <p className="text-gray-600 mt-1">
                                            {notification.message}
                                        </p>
                                        <p className="text-sm text-gray-500 mt-2">
                                            {notification.createdAt ? new Date(notification.createdAt).toLocaleString() : 'N/A'}
                                        </p>
                                    </div>
                                    <div className="flex gap-2">
                                        {!notification.isRead && (
                                            <button
                                                onClick={() => markAsRead(notification.id!)}
                                                className="p-2 text-cyan-600 hover:bg-cyan-100 rounded-lg transition-colors"
                                                title="Mark as read"
                                            >
                                                <Check className="w-5 h-5" />
                                            </button>
                                        )}
                                        <button
                                            onClick={() => deleteNotification(notification.id!)}
                                            className="p-2 text-red-600 hover:bg-red-100 rounded-lg transition-colors"
                                            title="Delete"
                                        >
                                            <Trash2 className="w-5 h-5" />
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </DashboardLayout>
    );
};

export default NotificationsPage;
