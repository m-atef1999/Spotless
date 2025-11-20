import { Client } from "./apiClient";

const baseUrl =
  import.meta.env.VITE_API_BASE_URL || "https://spotless.runasp.net/";

export const apiClient = new Client(baseUrl, {
  fetch: async (url: RequestInfo, init?: RequestInit) => {
    let token = null;
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
      token = authData.state.token;
    }

    if (token) {
      init = init || {};
      init.headers = {
        ...init.headers,
        Authorization: `Bearer ${token}`,
      };
    }

    let response = await fetch(url, init);

    if (response.status === 401 && !url.toString().includes("/refresh")) {
      // Try to refresh token
      const refreshToken = authData?.state?.refreshToken;
      if (refreshToken) {
        try {
          // Use raw fetch to avoid interceptor loop
          const refreshUrl = `${baseUrl}/api/Auth/refresh`;
          const refreshResponse = await fetch(refreshUrl, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              Accept: "application/json",
            },
            body: JSON.stringify({ refreshToken }),
          });

          if (refreshResponse.ok) {
            const newAuthData = await refreshResponse.json();

            // Update local storage
            const updatedStorage = {
              ...authData,
              state: {
                ...authData.state,
                token: newAuthData.token,
                refreshToken: newAuthData.refreshToken,
                user: newAuthData.user, // Assuming user object is returned or preserved
              },
            };
            localStorage.setItem(
              "auth-storage",
              JSON.stringify(updatedStorage)
            );

            // Retry original request with new token
            if (init) {
              init.headers = {
                ...init.headers,
                Authorization: `Bearer ${newAuthData.token}`,
              };
            }
            response = await fetch(url, init);
          } else {
            // Refresh failed, logout
            localStorage.removeItem("auth-storage");
            window.location.href = "/login";
          }
        } catch (error) {
          console.error("Token refresh failed", error);
          localStorage.removeItem("auth-storage");
          window.location.href = "/login";
        }
      } else {
        // No refresh token, logout
        localStorage.removeItem("auth-storage");
        window.location.href = "/login";
      }
    }

    return response;
  },
});
