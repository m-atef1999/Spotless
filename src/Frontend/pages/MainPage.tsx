
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import {
    User,
    ShoppingBag,
    MapPin,
    LayoutDashboard,
    LogIn,
    UserPlus,
    CheckCircle2,
    ShieldCheck,
    Clock,
    ArrowRight
} from 'lucide-react';

export const MainPage = () => {
    const { user, role } = useAuthStore();
    const navigate = useNavigate();

    const services = [
        { name: 'Dry Cleaning', price: '$15.00', description: 'Professional care for delicate fabrics' },
        { name: 'Wash & Fold', price: '$2.50/lb', description: 'Everyday laundry, perfectly folded' },
        { name: 'Ironing', price: '$5.00', description: 'Crisp and wrinkle-free finish' },
        { name: 'Stain Removal', price: '$10.00', description: 'Expert treatment for tough stains' },
        { name: 'Leather Care', price: '$45.00', description: 'Specialized cleaning for leather items' },
        { name: 'Wedding Dress', price: '$150.00', description: 'Preservation and cleaning' },
    ];

    const features = [
        { icon: <Clock className="w-6 h-6 text-blue-600" />, title: 'Fast Turnaround', description: 'Same-day service available for urgent needs' },
        { icon: <ShieldCheck className="w-6 h-6 text-blue-600" />, title: 'Quality Guarantee', description: 'We promise the best care for your clothes' },
        { icon: <CheckCircle2 className="w-6 h-6 text-blue-600" />, title: 'Eco-Friendly', description: 'Using sustainable and safe cleaning products' },
    ];

    const renderShortcuts = () => {
        if (!user) {
            return (
                <div className="flex flex-col sm:flex-row gap-4 justify-center mt-8">
                    <Link
                        to="/login"
                        className="flex items-center justify-center gap-2 px-8 py-3 bg-blue-600 text-white rounded-full hover:bg-blue-700 transition-colors font-semibold shadow-lg hover:shadow-xl"
                    >
                        <LogIn className="w-5 h-5" />
                        Login
                    </Link>
                    <Link
                        to="/register"
                        className="flex items-center justify-center gap-2 px-8 py-3 bg-white text-blue-600 border-2 border-blue-600 rounded-full hover:bg-blue-50 transition-colors font-semibold shadow-lg hover:shadow-xl"
                    >
                        <UserPlus className="w-5 h-5" />
                        Register
                    </Link>
                </div>
            );
        }

        const shortcuts = [];

        if (role === 'Customer') {
            shortcuts.push(
                { label: 'Dashboard', icon: <LayoutDashboard />, path: '/customer/dashboard', color: 'bg-indigo-500' },
                { label: 'New Order', icon: <ShoppingBag />, path: '/customer/new-order', color: 'bg-green-500' },
                { label: 'My Orders', icon: <Clock />, path: '/customer/orders', color: 'bg-blue-500' },
                { label: 'Profile', icon: <User />, path: '/customer/profile', color: 'bg-purple-500' }
            );
        } else if (role === 'Driver') {
            shortcuts.push(
                { label: 'Dashboard', icon: <LayoutDashboard />, path: '/driver/dashboard', color: 'bg-indigo-500' },
                { label: 'My Location', icon: <MapPin />, path: '/driver/location', color: 'bg-green-500' },
                { label: 'Profile', icon: <User />, path: '/driver/profile', color: 'bg-purple-500' }
            );
        } else if (role === 'Admin') {
            shortcuts.push(
                { label: 'Dashboard', icon: <LayoutDashboard />, path: '/admin/dashboard', color: 'bg-indigo-500' },
                { label: 'Drivers', icon: <User />, path: '/admin/drivers', color: 'bg-blue-500' },
                { label: 'Orders', icon: <ShoppingBag />, path: '/admin/orders', color: 'bg-green-500' }
            );
        }

        return (
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-8 max-w-3xl mx-auto px-4">
                {shortcuts.map((shortcut, index) => (
                    <button
                        key={index}
                        onClick={() => navigate(shortcut.path)}
                        className={`${shortcut.color} text-white p-6 rounded-xl shadow-lg hover:shadow-xl transform hover:-translate-y-1 transition-all duration-200 flex flex-col items-center gap-3`}
                    >
                        <div className="p-3 bg-white/20 rounded-full [&>svg]:w-6 [&>svg]:h-6">
                            {shortcut.icon}
                        </div>
                        <span className="font-semibold">{shortcut.label}</span>
                    </button>
                ))}
            </div>
        );
    };

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Hero Section */}
            <div className="bg-white relative overflow-hidden">
                <div className="absolute inset-0 bg-gradient-to-br from-blue-50 to-indigo-50 opacity-50" />
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-20 pb-16 relative">
                    <div className="text-center">
                        <h1 className="text-4xl md:text-6xl font-bold text-gray-900 mb-6 tracking-tight">
                            Premium Laundry Service <br />
                            <span className="text-blue-600">Delivered to Your Door</span>
                        </h1>
                        <p className="text-xl text-gray-600 mb-8 max-w-2xl mx-auto">
                            Experience the most convenient and professional laundry service.
                            We pick up, clean, and deliver your clothes with care.
                        </p>

                        {renderShortcuts()}
                    </div>
                </div>
            </div>

            {/* Features Section */}
            <div className="py-16 bg-white">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="grid md:grid-cols-3 gap-8">
                        {features.map((feature, index) => (
                            <div key={index} className="p-6 bg-gray-50 rounded-2xl border border-gray-100 text-center hover:shadow-md transition-shadow">
                                <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                                    {feature.icon}
                                </div>
                                <h3 className="text-lg font-semibold text-gray-900 mb-2">{feature.title}</h3>
                                <p className="text-gray-600">{feature.description}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            {/* Services & Pricing */}
            <div className="py-16 bg-gray-50">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="text-center mb-12">
                        <h2 className="text-3xl font-bold text-gray-900 mb-4">Our Services & Pricing</h2>
                        <p className="text-gray-600">Transparent pricing with no hidden fees</p>
                    </div>
                    <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {services.map((service, index) => (
                            <div key={index} className="bg-white p-6 rounded-xl shadow-sm border border-gray-100 hover:border-blue-200 transition-colors">
                                <div className="flex justify-between items-start mb-4">
                                    <h3 className="text-lg font-semibold text-gray-900">{service.name}</h3>
                                    <span className="px-3 py-1 bg-blue-50 text-blue-700 rounded-full text-sm font-medium">
                                        {service.price}
                                    </span>
                                </div>
                                <p className="text-gray-600 text-sm mb-4">{service.description}</p>
                                <button className="w-full py-2 text-blue-600 font-medium text-sm hover:bg-blue-50 rounded-lg transition-colors flex items-center justify-center gap-2 group">
                                    Learn More
                                    <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
                                </button>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            {/* Footer */}
            <footer className="bg-gray-900 text-white py-12">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="grid md:grid-cols-4 gap-8">
                        <div className="col-span-1 md:col-span-2">
                            <h3 className="text-xl font-bold mb-4">Spotless</h3>
                            <p className="text-gray-400 max-w-xs">
                                Your trusted partner for all your laundry and dry cleaning needs.
                                Quality service, delivered.
                            </p>
                        </div>
                        <div>
                            <h4 className="font-semibold mb-4">Quick Links</h4>
                            <ul className="space-y-2 text-gray-400">
                                <li><Link to="/login" className="hover:text-white transition-colors">Login</Link></li>
                                <li><Link to="/register" className="hover:text-white transition-colors">Register</Link></li>
                                <li><Link to="/driver/apply" className="hover:text-white transition-colors">Become a Driver</Link></li>
                            </ul>
                        </div>
                        <div>
                            <h4 className="font-semibold mb-4">Contact</h4>
                            <ul className="space-y-2 text-gray-400">
                                <li>support@spotless.com</li>
                                <li>1-800-SPOTLESS</li>
                            </ul>
                        </div>
                    </div>
                    <div className="border-t border-gray-800 mt-12 pt-8 text-center text-gray-500 text-sm">
                        © {new Date().getFullYear()} Spotless. All rights reserved.
                    </div>
                </div>
            </footer>
        </div>
    );
};
