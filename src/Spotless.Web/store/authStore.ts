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
                    const role = result.role as 'Admin' | 'Driver' | 'Customer';

                    // Store token temporarily to allow profile fetch
                    set({ token, role });

                    // Try to identify user role by fetching profiles
                    try {
                        if (role === 'Customer') {
                            const customerProfile = await CustomersService.getApiCustomersMe();
                            set({
                                user: customerProfile,
                                isLoading: false
                            });
                        } else if (role === 'Driver') {
                            const driverProfile = await DriversService.getApiDriversProfile();
                            set({
                                user: driverProfile,
                                isLoading: false
                            });
                        } else {
                            // Fallback: If role is undefined (old backend) or unknown, try fetching customer profile as default
                            try {
                                const customerProfile = await CustomersService.getApiCustomersMe();
                                set({
                                    user: customerProfile,
                                    role: 'Customer',
                                    isLoading: false
                                });
                            } catch {
                                // If that fails, just stop loading. User is authenticated but unknown.
                                set({ isLoading: false });
                            }
                        }
                    } catch (err) {
                        console.error('Failed to fetch profile', err);
                        set({ isLoading: false });
                    }

                } catch (error: any) {
                    let errorMessage = error.message || 'Login failed';
                    if (error.body && error.body.message) {
                        errorMessage = error.body.message;
                    }
                    set({ error: errorMessage, isLoading: false, token: null, role: null });
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
                    const role = result.role as 'Admin' | 'Driver' | 'Customer';

                    set({ token, role });

                    // Try to identify user role by fetching profiles
                    try {
                        if (role === 'Customer') {
                            const customerProfile = await CustomersService.getApiCustomersMe();
                            set({
                                user: customerProfile,
                                isLoading: false
                            });
                        } else if (role === 'Driver') {
                            const driverProfile = await DriversService.getApiDriversProfile();
                            set({
                                user: driverProfile,
                                isLoading: false
                            });
                        } else {
                            // Fallback: If role is undefined (old backend) or unknown, try fetching customer profile as default
                            try {
                                const customerProfile = await CustomersService.getApiCustomersMe();
                                set({
                                    user: customerProfile,
                                    role: 'Customer',
                                    isLoading: false
                                });
                            } catch {
                                // If that fails, just stop loading. User is authenticated but unknown.
                                set({ isLoading: false });
                            }
                        }
                    } catch (err) {
                        console.error('Failed to fetch profile', err);
                        set({ isLoading: false });
                    }

                } catch (error: any) {
                    let errorMessage = error.message || 'Google Login failed';
                    if (error.body && error.body.message) {
                        errorMessage = error.body.message;
                    }
                    set({ error: errorMessage, isLoading: false, token: null, role: null });
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

            logout: () => {
                window.dispatchEvent(new Event('spotless:logout'));
                set({ user: null, token: null, role: null });
            },

            setRole: (role) => set({ role }),
        }),
        {
            name: 'auth-storage',
            version: 1,
            migrate: (persistedState: any, version: number) => {
                if (version === 0) {
                    // Migration from version 0 (no version) to 1
                    // For safety, we can just clear the state or try to adapt it
                    // Here we'll just return the persisted state as is, but the version bump ensures we track it
                    return persistedState;
                }
                return persistedState;
            },
            partialize: (state) => ({
                user: state.user,
                token: state.token,
                role: state.role
            }),
        }
    )
);
