import { defineStore } from 'pinia';
import axios from 'axios';
import http from '@/api/http';
import router from '@/router';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    isAuthenticated: !!localStorage.getItem('token'),
    user: null as any,
    tokenExpiration: null as Date | null,
  }),
  getters: {
    authHeader: (state) => state.token ? { headers: { Authorization: `Bearer ${state.token}` } } : {},
    hasValidToken: (state) => {
      if (!state.token) {
        console.log('Auth: No token found');
        return false;
      }
      if (state.tokenExpiration && new Date() > state.tokenExpiration) {
        console.log('Auth: Token expired');
        return false;
      }
      console.log('Auth: Token is valid');
      return true;
    }
  },
  actions: {
    async login(email: string, password: string) {
      console.log('Auth: Attempting login', { email });
      try {
        // Usar el cliente HTTP personalizado en lugar de axios
        const { data } = await http.post('/api/auth/login', { email, password });
        this.setToken(data.token, data.expiration);
        console.log('Auth: Login successful');
        router.push('/');
        return true;
      } catch (error: any) {
        console.error('Auth: Login failed', error?.response?.data || error?.message || error);
        this.clearAuth();
        throw error;
      }
    },
    setToken(token: string, expiration?: string) {
      console.log('Auth: Setting token', { expiration });
      this.token = token;
      localStorage.setItem('token', token);
      
      // Set token expiration if provided
      if (expiration) {
        this.tokenExpiration = new Date(expiration);
        localStorage.setItem('tokenExpiration', expiration);
      }
      
      // Configurar token en axios global (para cualquier uso directo de axios)
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      
      // Configurar token en cliente HTTP personalizado (redundante, ya que http lo obtiene de localStorage)
      // http.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      
      this.isAuthenticated = true;
      
      console.log('Auth: Token configured in HTTP clients');
      
      // Setup interceptor to log requests with authorization
      this.setupInterceptors();
    },
    logout() {
      console.log('Auth: Logging out');
      this.clearAuth();
      router.push('/login');
    },
    clearAuth() {
      console.log('Auth: Clearing authentication');
      this.token = '';
      this.user = null;
      this.tokenExpiration = null;
      localStorage.removeItem('token');
      localStorage.removeItem('tokenExpiration');
      delete axios.defaults.headers.common['Authorization'];
      this.isAuthenticated = false;
    },
    initialize() {
      console.log('Auth: Initializing authentication state');
      
      const token = localStorage.getItem('token');
      const expiration = localStorage.getItem('tokenExpiration');
      
      if (token) {
        console.log('Auth: Found stored token');
        if (expiration) {
          this.tokenExpiration = new Date(expiration);
          // If token is expired, clear auth
          if (new Date() > this.tokenExpiration) {
            console.log('Auth: Stored token is expired, clearing');
            this.clearAuth();
            return;
          }
        }
        
        // Token valid, set up authentication
        this.setToken(token, expiration || undefined);
        console.log('Auth: Restored authentication from storage');
      } else {
        console.log('Auth: No token in storage, user is not authenticated');
      }
    },
    setupInterceptors() {
      // Add request interceptor
      axios.interceptors.request.use(
        config => {
          // Log requests
          console.log(`Auth: Request [${config.method?.toUpperCase()}] ${config.url}`, 
                     { hasAuth: !!config.headers.Authorization });
          return config;
        },
        error => {
          console.error('Auth: Request error', error);
          return Promise.reject(error);
        }
      );
      
      // Add response interceptor
      axios.interceptors.response.use(
        response => {
          console.log(`Auth: Response [${response.status}] ${response.config.url}`);
          return response;
        },
        error => {
          if (error.response?.status === 401) {
            console.error('Auth: 401 Unauthorized response');
            // Only logout if we were previously authenticated
            if (this.isAuthenticated) {
              console.log('Auth: Logging out due to 401 response');
              this.clearAuth();
              router.push('/login');
            }
          }
          return Promise.reject(error);
        }
      );
    }
  }
});
