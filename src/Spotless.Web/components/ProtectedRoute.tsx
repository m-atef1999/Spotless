import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';

interface ProtectedRouteProps {
    children: React.ReactNode;
    allowedRoles?: string[];
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, allowedRoles }) => {
    const { token, role } = useAuthStore();
    const location = useLocation();
    const isAuthenticated = !!token;

    if (!isAuthenticated) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    if (allowedRoles && role && !allowedRoles.includes(role)) {
        // Redirect to appropriate dashboard based on role if trying to access unauthorized area
        if (role === 'Customer') return <Navigate to="/customer/dashboard" replace />;
        if (role === 'Driver') return <Navigate to="/driver/dashboard" replace />;
        if (role === 'Admin') return <Navigate to="/admin/dashboard" replace />;
        return <Navigate to="/" replace />;
    }

    return <>{children}</>;
};
