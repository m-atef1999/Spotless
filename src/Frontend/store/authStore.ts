import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { LoginCommand, RegisterCustomerCommand, SubmitDriverApplicationDto, CustomerDto, DriverDto } from '../lib/apiClient';
import { apiClient } from '../lib/api';

interface AuthState {
    user: CustomerDto | DriverDto | null;
    token: string | null;
    role: 'Admin' | 'Driver' | 'Customer' | null;
    isLoading: boolean;
    error: string | null;

    login: (cmd: LoginCommand) => Promise<void>;
    registerCustomer: (cmd: RegisterCustomerCommand) => Promise<void>;
    registerDriver: (cmd: SubmitDriverApplicationDto) => Promise<void>;
    logout: () => void;
    setRole: (role: 'Admin' | 'Driver' | 'Customer') => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            user: null,
            token: null,
            role: null,
            isLoading: false,
            error: null,

            login: async (cmd) => {
                set({ isLoading: true, error: null });
                try {
                    const result = await apiClient.login(cmd);
                    const token = result.accessToken;

                    // Store token temporarily to allow profile fetch
                    set({ token });

                    // Try to identify user role by fetching profiles
                    try {
                        const customerProfile = await apiClient.profileGET();
                        set({
                            user: customerProfile,
                            role: 'Customer',
                            token: token,
                            isLoading: false
                        });
                        return;
                    } catch {
                        // Not a customer
                    }

                    try {
                        const driverProfile = await apiClient.profileGET2();
                        set({
                            user: driverProfile,
                            role: 'Driver',
                            token: token,
                            isLoading: false
                        });
                        return;
                    } catch {
                        // Not a driver
                    }

                    // If both fail, might be Admin or just authenticated without profile
                    // For now, let's assume Admin if we have a token but no profile matches (or handle differently)
                    // TODO: Add specific Admin profile check if available
                    set({
                        token: token,
                        role: 'Admin', // Fallback for now
                        isLoading: false
                    });

                } catch (error) {
                    set({ error: (error as Error).message || 'Login failed', isLoading: false, token: null, role: null });
                    throw error;
                }
            },

            registerCustomer: async (cmd) => {
                set({ isLoading: true, error: null });
                try {
                    const result = await apiClient.register(cmd);
                    set({
                        token: result.accessToken,
                        role: 'Customer',
                        isLoading: false
                    });
                } catch (error) {
                    set({ error: (error as Error).message || 'Registration failed', isLoading: false });
                    throw error;
                }
            },

            registerDriver: async (cmd) => {
                set({ isLoading: true, error: null });
                try {
                    await apiClient.register2(cmd);
                    set({ isLoading: false });
                } catch (error) {
                    set({ error: (error as Error).message || 'Driver application failed', isLoading: false });
                    throw error;
                }
            },

            logout: () => set({ user: null, token: null, role: null }),

            setRole: (role) => set({ role }),
        }),
        {
            name: 'auth-storage',
        }
    )
);
