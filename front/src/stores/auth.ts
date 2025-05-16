import { defineStore } from 'pinia';
import axios from 'axios';
import router from '@/router';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || '',
    isAuthenticated: !!localStorage.getItem('token'),
  }),
  getters: {
    authHeader: (state) => state.token ? { headers: { Authorization: `Bearer ${state.token}` } } : {}
  },
  actions: {
    async login(email: string, password: string) {
      const { data } = await axios.post('/api/auth/login', { email, password });
      this.token = data.token;
      localStorage.setItem('token', this.token);
      axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
      this.isAuthenticated = true;
      router.push('/');
    },
    logout() {
      this.token = '';
      localStorage.removeItem('token');
      delete axios.defaults.headers.common['Authorization'];
      this.isAuthenticated = false;
      router.push('/login');
    },
    initialize() {
      if (this.token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
        this.isAuthenticated = true;
      }
    }
  }
});
