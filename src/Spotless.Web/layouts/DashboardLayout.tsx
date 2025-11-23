import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Link, useNavigate } from 'react-router-dom';
import {
    LogOut,
    Menu,
    User,
    Bell,
    LayoutDashboard,
    PlusCircle,
    History,
    CreditCard,
    Settings
} from 'lucide-react';
import { useAuthStore } from '../store/authStore';
import logo from '../logos/spotless_logo.png';
import { AiChatWidget } from '../components/ai/AiChatWidget';
import { ThemeToggle } from '../components/ui/ThemeToggle';
import { NotificationsService, type NotificationDto } from '../lib/api';
import { NotificationType } from '../lib/constants';
import { BackToTop } from '../components/ui/BackToTop';


interface DashboardLayoutProps {
    children: React.ReactNode;
    role: 'Customer' | 'Driver' | 'Admin';
}

const mockNotifications: NotificationDto[] = [
    {
        id: 'mock-welcome',
        title: 'Welcome to Spotless!',
        message: 'Get started by placing your first order.',
        type: NotificationType.System,
        isRead: false,
        createdAt: '2024-01-01T00:00:00.000Z' // Old date to stay at bottom
    },
    {
        id: 'mock-profile',
        title: 'Complete your profile',
        message: 'Add your address for faster checkout.',
        type: NotificationType.System,
        isRead: false,
        createdAt: '2024-01-01T00:00:00.000Z' // Old date to stay at bottom
    }
];

export const DashboardLayout: React.FC<DashboardLayoutProps> = ({ children, role }) => {
    const { logout, user } = useAuthStore();
    const navigate = useNavigate();
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);

    // Load read mock IDs from localStorage
    const getReadMockIds = (): string[] => {
        try {
            const stored = localStorage.getItem('readMockNotifications');
            return stored ? JSON.parse(stored) : [];
        } catch {
            return [];
        }
    };

    // Initialize with mocks, checking localStorage for read status
    const [notifications, setNotifications] = useState<NotificationDto[]>(() => {
        const readIds = getReadMockIds();
        return mockNotifications.map(n => ({
            ...n,
            isRead: readIds.includes(n.id!)
        }));
    });

    const [unreadCount, setUnreadCount] = useState(() => {
        const readIds = getReadMockIds();
        return mockNotifications.filter(n => !readIds.includes(n.id!)).length;
    });

    const fetchNotifications = async () => {
        try {
            const data = await NotificationsService.getApiNotifications({ unreadOnly: false });
            const safeData = Array.isArray(data) ? data : [];

            // Get latest read status for mocks
            const readMockIds = getReadMockIds();
            const processedMocks = mockNotifications.map(n => ({
                ...n,
                isRead: readMockIds.includes(n.id!)
            }));

            // Combine real and mock notifications
            const allNotifications = [...safeData, ...processedMocks];

            // Sort by date descending (newest first)
            const sorted = allNotifications.sort((a, b) => {
                const getTime = (dateStr?: string) => {
                    if (!dateStr) return 0;
                    const d = new Date(dateStr);
                    return isNaN(d.getTime()) ? 0 : d.getTime();
                };
                const timeA = getTime(a.createdAt);
                const timeB = getTime(b.createdAt);
                return timeB - timeA;
            });
            // Keep only the latest 20
            const limited = sorted.slice(0, 20);

            setNotifications(limited);
            setUnreadCount(limited.filter(n => !n.isRead).length);
        } catch (error) {
            console.error('Failed to fetch notifications', error);
            // Fallback to mocks
            const readMockIds = getReadMockIds();
            const processedMocks = mockNotifications.map(n => ({
                ...n,
                isRead: readMockIds.includes(n.id!)
            }));
            setNotifications(processedMocks);
            setUnreadCount(processedMocks.filter(n => !n.isRead).length);
        }
    };

    useEffect(() => {
        if (user) {
            fetchNotifications();
            const interval = setInterval(fetchNotifications, 30000); // Poll every 30s
            return () => clearInterval(interval);
        }
    }, [user]);

    const handleNotificationClick = async (notification: NotificationDto) => {
        // Handle Mock Notifications Persistence
        if (notification.id?.startsWith('mock-')) {
            const readIds = getReadMockIds();
            if (!readIds.includes(notification.id)) {
                const newReadIds = [...readIds, notification.id];
                localStorage.setItem('readMockNotifications', JSON.stringify(newReadIds));
            }
        }

        // Only call API for real notifications (not mocks)
        if (!notification.isRead && notification.id && !notification.id.startsWith('mock-')) {
            try {
                await NotificationsService.putApiNotificationsRead({ id: notification.id });
            } catch (error) {
                console.error('Failed to mark notification as read', error);
            }
        }

        // Optimistically update UI for both real and mock
        setNotifications(prev => prev.map(n =>
            n.id === notification.id ? { ...n, isRead: true } : n
        ));
        setUnreadCount(prev => Math.max(0, prev - 1));

        // Handle Redirection
        if (notification.id === 'mock-welcome' || notification.message?.includes('placing your first order')) {
            navigate('/customer/new-order');
        } else if (notification.id === 'mock-profile' || notification.message?.includes('address')) {
            navigate('/customer/settings');
        } else if (notification.type === NotificationType.OrderInProgress ||
            notification.type === NotificationType.OrderCreated ||
            notification.type === NotificationType.OrderConfirmed ||
            notification.type === NotificationType.OrderCompleted ||
            notification.type === NotificationType.OrderCancelled ||
            notification.type === NotificationType.PaymentReceived ||
            notification.type === NotificationType.PaymentFailed ||
            notification.type === NotificationType.OrderAssigned ||
            notification.title?.toLowerCase().includes('order') ||
            notification.message?.toLowerCase().includes('status')) {
            navigate('/customer/orders');
        } else if (notification.message?.toLowerCase().includes('profile')) {
            navigate('/customer/settings');
        }
    };

    const menuItems = {
        Customer: [
            { icon: LayoutDashboard, label: 'Overview', href: '/customer/dashboard' },
            { icon: PlusCircle, label: 'New Order', href: '/customer/new-order' },
            { icon: History, label: 'My Orders', href: '/customer/orders' },
            { icon: CreditCard, label: 'Wallet', href: '/customer/wallet' },
            { icon: Settings, label: 'Settings', href: '/customer/settings' },
        ],
        Driver: [
            { icon: LayoutDashboard, label: 'Dashboard', href: '/driver/dashboard' },
            { icon: History, label: 'Job History', href: '/driver/jobs' },
            { icon: CreditCard, label: 'Earnings', href: '/driver/earnings' },
            { icon: Settings, label: 'Settings', href: '/driver/settings' },
        ],
        Admin: [
            { icon: LayoutDashboard, label: 'Dashboard', href: '/admin/dashboard' },
            { icon: User, label: 'Users', href: '/admin/users' },
            { icon: History, label: 'All Orders', href: '/admin/orders' },
            { icon: Settings, label: 'Settings', href: '/admin/settings' },
        ]
    };

    const currentMenuItems = menuItems[role] || [];

    const getNotificationColor = (type?: number | string, title?: string | null) => {
        // Fallback based on title if type is generic or missing
        if (title) {
            const lowerTitle = title.toLowerCase();
            if (lowerTitle.includes('cancel') || lowerTitle.includes('failed') || lowerTitle.includes('rejected')) {
                return 'bg-red-500';
            }
            if (lowerTitle.includes('success') || lowerTitle.includes('complete') || lowerTitle.includes('received') || lowerTitle.includes('approved')) {
                return 'bg-green-500';
            }
        }

        if (type === undefined || type === null) return 'bg-amber-500';

        let typeValue = type;
        if (typeof type === 'string') {
            // Map string to enum value if possible
            const key = Object.keys(NotificationType).find(k => k === type);
            if (key) {
                typeValue = NotificationType[key as keyof typeof NotificationType];
            }
        }

        switch (typeValue) {
            case NotificationType.OrderCreated:
            case NotificationType.OrderConfirmed:
            case NotificationType.OrderInProgress:
            case NotificationType.OrderCompleted:
            case NotificationType.OrderAssigned:
                return 'bg-cyan-500';
            case NotificationType.PaymentReceived:
            case NotificationType.PaymentFailed:
                return 'bg-green-500';
            case NotificationType.OrderCancelled:
                return 'bg-red-500';
            case NotificationType.System:
                // If system type, rely on title fallback above, otherwise default
                return 'bg-amber-500';
            default:
                return 'bg-amber-500';
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 dark:bg-slate-950 flex">
            {/* Sidebar */}
            <motion.aside
                initial={false}
                animate={{ width: isSidebarOpen ? 280 : 80 }}
                className="bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-800 relative z-20 hidden md:flex flex-col sticky top-0 h-screen"
            >
                {/* Logo Area */}
                <div className="h-20 flex items-center px-6 border-b border-slate-100 dark:border-slate-800">
                    <Link to="/" className="flex items-center gap-3 overflow-hidden group">
                        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-cyan-500 to-teal-500 flex items-center justify-center shrink-0 shadow-lg shadow-cyan-500/20 group-hover:scale-105 transition-transform">
                            <img src={logo} alt="Logo" className="w-6 h-6 object-contain brightness-0 invert" />
                        </div>
                        {isSidebarOpen && (
                            <motion.span
                                initial={{ opacity: 0 }}
                                animate={{ opacity: 1 }}
                                className="font-bold text-xl text-slate-900 dark:text-white tracking-tight"
                            >
                                Spotless
                            </motion.span>
                        )}
                    </Link>
                </div>

                {/* Navigation */}
                <nav className="flex-1 py-6 px-3 space-y-1 overflow-y-auto">
                    {currentMenuItems.map((item, index) => (
                        <Link
                            key={index}
                            to={item.href}
                            className="flex items-center gap-3 px-3 py-3 rounded-xl text-slate-600 dark:text-slate-400 hover:bg-cyan-50 dark:hover:bg-cyan-900/10 hover:text-cyan-600 dark:hover:text-cyan-400 transition-all group"
                        >
                            <item.icon className="w-6 h-6 shrink-0 group-hover:scale-110 transition-transform" />
                            {isSidebarOpen && (
                                <motion.span
                                    initial={{ opacity: 0 }}
                                    animate={{ opacity: 1 }}
                                    className="font-medium"
                                >
                                    {item.label}
                                </motion.span>
                            )}
                        </Link>
                    ))}
                </nav>

                {/* User Profile & Logout */}
                <div className="p-4 border-t border-slate-100 dark:border-slate-800">
                    <div className={`flex items-center gap-3 ${!isSidebarOpen && 'justify-center'}`}>
                        <div className="w-10 h-10 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center shrink-0">
                            <User className="w-5 h-5 text-slate-500" />
                        </div>
                        {isSidebarOpen && (
                            <div className="flex-1 overflow-hidden">
                                <p className="text-sm font-semibold text-slate-900 dark:text-white truncate">
                                    {(user as any)?.name || user?.email || 'User'}
                                </p>
                                <p className="text-xs text-slate-500 truncate">
                                    {role}
                                </p>
                            </div>
                        )}
                        {isSidebarOpen && (
                            <button
                                onClick={() => logout()}
                                className="p-2 rounded-lg hover:bg-red-50 dark:hover:bg-red-900/10 text-slate-400 hover:text-red-500 transition-colors"
                            >
                                <LogOut className="w-5 h-5" />
                            </button>
                        )}
                    </div>
                </div>
            </motion.aside>

            {/* Main Content */}
            <div className="flex-1 flex flex-col min-w-0">
                {/* Header */}
                <header className="h-20 bg-white/80 dark:bg-slate-900/80 backdrop-blur-xl border-b border-slate-200 dark:border-slate-800 flex items-center justify-between px-4 sm:px-8 sticky top-0 z-10">
                    <div className="flex items-center gap-4">
                        <button
                            onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                            className="p-2 rounded-xl hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400 transition-colors"
                        >
                            <Menu className="w-6 h-6" />
                        </button>
                        <h1 className="text-xl font-semibold text-slate-900 dark:text-white hidden sm:block">
                            Dashboard
                        </h1>
                    </div>

                    <div className="flex items-center gap-4">
                        <ThemeToggle />
                        <div className="relative group h-full flex items-center">
                            <button className="p-2 rounded-xl hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400 relative">
                                <Bell className="w-6 h-6" />
                                {unreadCount > 0 && (
                                    <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white dark:border-slate-900"></span>
                                )}
                            </button>
                            {/* Notification Dropdown - Added pt-4 to bridge the gap for hover */}
                            <div className="absolute right-0 top-full pt-4 w-80 hidden group-hover:block z-50">
                                <div className="bg-white dark:bg-slate-900 rounded-xl shadow-xl border border-slate-200 dark:border-slate-800 p-4 animate-fade-in-up">
                                    <div className="flex justify-between items-center mb-3">
                                        <h3 className="font-bold text-slate-900 dark:text-white">Notifications</h3>
                                        {unreadCount > 0 && (
                                            <span className="text-xs text-cyan-600 dark:text-cyan-400 font-medium">
                                                {unreadCount} new
                                            </span>
                                        )}
                                    </div>
                                    <div className="space-y-2 max-h-[300px] overflow-y-auto">
                                        {notifications.length === 0 ? (
                                            <p className="text-sm text-slate-500 text-center py-4">No notifications</p>
                                        ) : (
                                            notifications.map((notification) => (
                                                <div
                                                    key={notification.id}
                                                    onClick={() => handleNotificationClick(notification)}
                                                    className={`flex items-start gap-3 p-2 rounded-lg transition-colors cursor-pointer ${notification.isRead
                                                        ? 'opacity-60 hover:bg-slate-50 dark:hover:bg-slate-800'
                                                        : 'bg-cyan-50/50 dark:bg-cyan-900/10 hover:bg-cyan-50 dark:hover:bg-cyan-900/20'
                                                        }`}
                                                >
                                                    <div className={`w-2 h-2 mt-2 rounded-full shrink-0 ${getNotificationColor(notification.type, notification.title)}`} />
                                                    <div>
                                                        <p className="text-sm text-slate-900 dark:text-white font-medium">{notification.title}</p>
                                                        <p className="text-xs text-slate-500 line-clamp-2">{notification.message}</p>
                                                        <p className="text-[10px] text-slate-400 mt-1">
                                                            {new Date(notification.createdAt || '').toLocaleDateString()}
                                                        </p>
                                                    </div>
                                                </div>
                                            ))
                                        )}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </header>

                {/* Page Content */}
                <main className="flex-1 p-4 sm:p-8 overflow-y-auto relative">
                    <div className="max-w-7xl mx-auto">
                        {children}
                    </div>

                    {/* AI Chat Widget - Only for Customers */}
                    {role === 'Customer' && <AiChatWidget />}
                </main>
            </div>
            <BackToTop />
        </div>
    );
};
