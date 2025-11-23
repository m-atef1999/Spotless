import { Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'react-hot-toast';
import { LoginPage } from './pages/LoginPage';
import { MainPage } from './pages/MainPage';
import { ProtectedRoute } from './components/ProtectedRoute';
import { CustomerDashboard } from './pages/customer/Dashboard';
import { NewOrderPage } from './pages/customer/NewOrderPage';
import { MyOrdersPage } from './pages/customer/MyOrdersPage';
import { OrderDetailsPage } from './pages/customer/OrderDetailsPage';
import { ProfilePage } from './pages/customer/ProfilePage';
import { WalletPage } from './pages/customer/WalletPage';
import { ServicesPage } from './pages/customer/ServicesPage';
import { DriverDashboard } from './pages/driver/Dashboard';
import { AdminDashboardPage } from './pages/admin/AdminDashboardPage';
import { DriverManagementPage } from './pages/admin/DriverManagementPage';
import { OrderManagementPage } from './pages/admin/OrderManagementPage';
import { ServiceManagementPage } from './pages/admin/ServiceManagementPage';
import { ReviewsManagementPage } from './pages/admin/ReviewsManagementPage';
import { RegisterPage } from './pages/customer/RegisterPage';
import { DriverApplicationPage } from './pages/driver/DriverApplicationPage';
import { DriverProfilePage } from './pages/driver/DriverProfilePage';
import { LocationPage } from './pages/driver/LocationPage';
import { ForgotPasswordPage } from './pages/ForgotPasswordPage';
import { PublicServicesPage } from './pages/PublicServicesPage';
import { ResetPasswordPage } from './pages/ResetPasswordPage';
import { ToastProvider } from './components/ui/Toast';
import { CustomerManagementPage } from './pages/admin/CustomerManagementPage';
import { CategoryManagementPage } from './pages/admin/CategoryManagementPage';

import { PaymentMethodsPage } from './pages/customer/PaymentMethodsPage';
import { CartPage } from './pages/customer/CartPage';
import { EarningsPage } from './pages/driver/EarningsPage';
import { AvailableOrdersPage } from './pages/driver/AvailableOrdersPage';
import { OrderHistoryPage } from './pages/driver/OrderHistoryPage';
import AnalyticsPage from './pages/admin/AnalyticsPage';
import SystemSettingsPage from './pages/admin/SystemSettingsPage';
import AuditLogsPage from './pages/admin/AuditLogsPage';
import { CreateReviewPage } from './pages/customer/CreateReviewPage';
import { ServiceDetailsPage } from './pages/customer/ServiceDetailsPage';
import { DriverStatusPage } from './pages/driver/DriverStatusPage';
import { AdminUsersPage } from './pages/admin/AdminUsersPage';
import { DriverApplicationsPage } from './pages/admin/DriverApplicationsPage';
import { DebugPanel } from './components/dev/DebugPanel';
import { NotificationProvider } from './contexts/NotificationContext';

import { useEffect } from 'react';
import { useAuthStore } from './store/authStore';
import { OpenAPI } from './lib/api';

const queryClient = new QueryClient();

function App() {
  const { token, fetchProfile } = useAuthStore();

  useEffect(() => {
    if (token) {
      OpenAPI.TOKEN = token;
      fetchProfile();
    }
  }, [token]);

  return (
    <QueryClientProvider client={queryClient}>
      <ToastProvider>
        <NotificationProvider>
          <Routes>
            {/* Public Routes */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/driver/apply" element={<DriverApplicationPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />
            <Route path="/services" element={<PublicServicesPage />} />

            {/* Protected Customer Routes */}
            <Route
              path="/customer/*"
              element={
                <ProtectedRoute allowedRoles={['Customer']}>
                  <Routes>
                    <Route path="dashboard" element={<CustomerDashboard />} />
                    <Route path="new-order" element={<NewOrderPage />} />
                    <Route path="orders" element={<MyOrdersPage />} />
                    <Route path="orders/:id" element={<OrderDetailsPage />} />
                    <Route path="profile" element={<ProfilePage />} />
                    <Route path="wallet" element={<WalletPage />} />
                    <Route path="services" element={<ServicesPage />} />

                    <Route path="payment-methods" element={<PaymentMethodsPage />} />
                    <Route path="cart" element={<CartPage />} />
                    <Route path="services/:id" element={<ServiceDetailsPage />} />
                    <Route path="create-review" element={<CreateReviewPage />} />
                    <Route path="settings" element={<ProfilePage />} />
                    <Route path="*" element={<Navigate to="dashboard" replace />} />
                  </Routes>
                </ProtectedRoute>
              }
            />

            {/* Protected Driver Routes */}
            <Route
              path="/driver/*"
              element={
                <ProtectedRoute allowedRoles={['Driver']}>
                  <Routes>
                    <Route path="dashboard" element={<DriverDashboard />} />
                    <Route path="profile" element={<DriverProfilePage />} />
                    <Route path="location" element={<LocationPage />} />
                    <Route path="earnings" element={<EarningsPage />} />
                    <Route path="available-orders" element={<AvailableOrdersPage />} />
                    <Route path="order-history" element={<OrderHistoryPage />} />
                    <Route path="status" element={<DriverStatusPage />} />
                    <Route path="*" element={<Navigate to="dashboard" replace />} />
                  </Routes>
                </ProtectedRoute>
              }
            />

            {/* Protected Admin Routes */}
            <Route
              path="/admin/*"
              element={
                <ProtectedRoute allowedRoles={['Admin']}>
                  <Routes>
                    <Route path="dashboard" element={<AdminDashboardPage />} />
                    <Route path="drivers" element={<DriverManagementPage />} />
                    <Route path="orders" element={<OrderManagementPage />} />
                    <Route path="services" element={<ServiceManagementPage />} />
                    <Route path="reviews" element={<ReviewsManagementPage />} />
                    <Route path="customers" element={<CustomerManagementPage />} />
                    <Route path="categories" element={<CategoryManagementPage />} />
                    <Route path="analytics" element={<AnalyticsPage />} />
                    <Route path="settings" element={<SystemSettingsPage />} />
                    <Route path="audit-logs" element={<AuditLogsPage />} />
                    <Route path="users" element={<AdminUsersPage />} />
                    <Route path="driver-applications" element={<DriverApplicationsPage />} />
                    <Route path="*" element={<Navigate to="dashboard" replace />} />
                  </Routes>
                </ProtectedRoute>
              }
            />

            {/* Default Route */}
            <Route path="/" element={<MainPage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </NotificationProvider>
      </ToastProvider>
      <Toaster position="top-right" />
      <DebugPanel />
    </QueryClientProvider>
  );
}

export default App;
