import React from 'react'
import { GoogleOAuthProvider } from '@react-oauth/google'
import ReactDOM from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import App from './App'
import './index.css'


import { OpenAPI } from './lib/api';

const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
const apiUrl = import.meta.env.VITE_API_URL || 'https://spotless.runasp.net';

if (apiUrl) {
  OpenAPI.BASE = apiUrl;
}

// Initialize OpenAPI Token from localStorage
try {
  const authStorage = localStorage.getItem('auth-storage');
  if (authStorage) {
    const { state } = JSON.parse(authStorage);
    if (state && state.token) {
      OpenAPI.TOKEN = state.token;
    }
  }
} catch (e) {
  console.error("Failed to restore auth token", e);
}

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <GoogleOAuthProvider clientId={clientId || ''}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </GoogleOAuthProvider>
  </React.StrictMode>,
)
