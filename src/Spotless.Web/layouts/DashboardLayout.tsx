import React from 'react';
import { motion } from 'framer-motion';
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


interface DashboardLayoutProps {
    children: React.ReactNode;
    role: 'Customer' | 'Driver' | 'Admin';
}

export const DashboardLayout: React.FC<DashboardLayoutProps> = ({ children, role }) => {
    const { logout, user } = useAuthStore();
    const [isSidebarOpen, setIsSidebarOpen] = React.useState(true);

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

    return (
        <div className="min-h-screen bg-slate-50 dark:bg-slate-950 flex">
            {/* Sidebar */}
            <motion.aside
                initial={false}
                animate={{ width: isSidebarOpen ? 280 : 80 }}
                className="bg-white dark:bg-slate-900 border-r border-slate-200 dark:border-slate-800 relative z-20 hidden md:flex flex-col"
            >
                {/* Logo Area */}
                <div className="h-20 flex items-center px-6 border-b border-slate-100 dark:border-slate-800">
                    <div className="flex items-center gap-3 overflow-hidden">
                        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-cyan-500 to-teal-500 flex items-center justify-center shrink-0 shadow-lg shadow-cyan-500/20">
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
                    </div>
                </div>

                {/* Navigation */}
                <nav className="flex-1 py-6 px-3 space-y-1 overflow-y-auto">
                    {currentMenuItems.map((item, index) => (
                        <a
                            key={index}
                            href={item.href}
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
                        </a>
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
                                    {user?.email || 'User'}
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
                        <button className="p-2 rounded-xl hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-600 dark:text-slate-400 relative">
                            <Bell className="w-6 h-6" />
                            <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white dark:border-slate-900"></span>
                        </button>
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
        </div>
    );
};
