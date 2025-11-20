import { Client } from "./apiClient";
import axios from "axios";

const baseUrl =
  import.meta.env.VITE_API_BASE_URL || "https://spotless.runasp.net";

// Create axios instance with interceptors
const axiosInstance = axios.create({
  baseURL: baseUrl,
});

// Request interceptor to add auth token
axiosInstance.interceptors.request.use(
  (config) => {
    const getStoredAuth = () => {
      try {
        const stored = localStorage.getItem("auth-storage");
        if (stored) {
          return JSON.parse(stored);
        }
      } catch (e) {
        console.error("Failed to retrieve token", e);
      }
      return null;
    };

    const authData = getStoredAuth();
    if (authData?.state?.token) {
      config.headers.Authorization = `Bearer ${authData.state.token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle 401 and refresh token
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // If 401 and not already retrying and not a refresh request
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes("/refresh")
    ) {
      originalRequest._retry = true;

      const getStoredAuth = () => {
        try {
          const stored = localStorage.getItem("auth-storage");
          if (stored) {
            return JSON.parse(stored);
          }
        } catch (e) {
          console.error("Failed to retrieve auth", e);
        }
        return null;
      };

      const authData = getStoredAuth();
      const refreshToken = authData?.state?.refreshToken;

      if (refreshToken) {
        try {
          // Call refresh endpoint
          const refreshResponse = await axios.post(
            `${baseUrl}/api/Auth/refresh`,
            { refreshToken },
            {
              headers: {
                "Content-Type": "application/json",
                Accept: "application/json",
              },
            }
          );

          if (refreshResponse.status === 200) {
            const newAuthData = refreshResponse.data;

            // Update local storage
            const updatedStorage = {
              ...authData,
              state: {
                ...authData.state,
                token: newAuthData.token,
                refreshToken: newAuthData.refreshToken,
                user: newAuthData.user,
              },
            };
            localStorage.setItem("auth-storage", JSON.stringify(updatedStorage));

            // Update the failed request with new token
            originalRequest.headers.Authorization = `Bearer ${newAuthData.token}`;

            // Retry the original request
            return axiosInstance(originalRequest);
          }
        } catch (refreshError) {
          console.error("Token refresh failed", refreshError);
          localStorage.removeItem("auth-storage");
          window.location.href = "/login";
          return Promise.reject(refreshError);
        }
      }

      // No refresh token, logout
      localStorage.removeItem("auth-storage");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);

// Create and export API client with axios instance
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const apiClient = new Client(baseUrl, axiosInstance as any);
