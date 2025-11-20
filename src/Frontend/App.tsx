import { Routes, Route, Navigate } from 'react-router-dom';
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
import { AdminDashboard } from './pages/admin/Dashboard';
import { DriverManagementPage } from './pages/admin/DriverManagementPage';
import { OrderManagementPage } from './pages/admin/OrderManagementPage';
import { ServiceManagementPage } from './pages/admin/ServiceManagementPage';
import { ReviewsManagementPage } from './pages/admin/ReviewsManagementPage';
import { RegisterPage } from './pages/customer/RegisterPage';
import { DriverApplicationPage } from './pages/driver/DriverApplicationPage';
import { DriverProfilePage } from './pages/driver/DriverProfilePage';
import { LocationPage } from './pages/driver/LocationPage';
import { ForgotPasswordPage } from './pages/ForgotPasswordPage';
import { ResetPasswordPage } from './pages/ResetPasswordPage';
import { ToastProvider } from './components/ui/Toast';

function App() {
  return (
    <ToastProvider>
      <Routes>
        {/* Public Routes */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/driver/apply" element={<DriverApplicationPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />

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
                <Route path="dashboard" element={<AdminDashboard />} />
                <Route path="drivers" element={<DriverManagementPage />} />
                <Route path="orders" element={<OrderManagementPage />} />
                <Route path="services" element={<ServiceManagementPage />} />
                <Route path="reviews" element={<ReviewsManagementPage />} />
                <Route path="*" element={<Navigate to="dashboard" replace />} />
              </Routes>
            </ProtectedRoute>
          }
        />

        {/* Default Route */}
        <Route path="/" element={<MainPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </ToastProvider>
  );
}

export default App;
