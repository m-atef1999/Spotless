import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import {
    AuthService,
    CustomersService,
    DriversService,
    type LoginCommand,
    type RegisterCustomerCommand,
    type DriverApplicationRequest,
    type CustomerDto,
    type DriverDto
} from '../lib/api';

interface AuthState {
    user: CustomerDto | DriverDto | null;
    token: string | null;
    role: 'Admin' | 'Driver' | 'Customer' | null;
    isLoading: boolean;
    error: string | null;

    login: (cmd: LoginCommand) => Promise<void>;
    loginWithGoogle: (token: string) => Promise<void>;
    registerCustomer: (cmd: RegisterCustomerCommand) => Promise<void>;
    registerDriver: (cmd: DriverApplicationRequest) => Promise<void>;
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
                    const result = await AuthService.postApiAuthLogin({ requestBody: cmd });
                    const token = result.accessToken;

                    // Store token temporarily to allow profile fetch
                    set({ token });

                    // Try to identify user role by fetching profiles
                    try {
                        const customerProfile = await CustomersService.getApiCustomersMe();
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
                        const driverProfile = await DriversService.getApiDriversProfile();
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
                    // For now, let's assume Admin if we have a token but no profile matches
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

            loginWithGoogle: async (googleToken) => {
                set({ isLoading: true, error: null });
                try {
                    const result = await AuthService.postApiAuthExternalGoogle({
                        requestBody: {
                            provider: 'GOOGLE',
                            idToken: googleToken
                        }
                    });

                    const token = result.accessToken;
                    set({ token });

                    // Try to identify user role by fetching profiles
                    try {
                        const customerProfile = await CustomersService.getApiCustomersMe();
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

                    // If not a customer, we assume they need to register or something went wrong, 
                    // but for now let's just set them as authenticated so they can be redirected
                    // In a real app, you might want to check if they are a driver too, or redirect to a "finish registration" page.
                    // But for this specific flow, we'll assume they are a customer or just logged in.
                    set({
                        token: token,
                        role: 'Customer', // Default to customer for Google Login for now
                        isLoading: false
                    });

                } catch (error) {
                    set({ error: (error as Error).message || 'Google Login failed', isLoading: false, token: null, role: null });
                    throw error;
                }
            },

            registerCustomer: async (cmd) => {
                set({ isLoading: true, error: null });
                try {
                    const result = await CustomersService.postApiCustomersRegister({ requestBody: cmd });
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
                    await DriversService.postApiDriversRegister({ requestBody: cmd });
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
