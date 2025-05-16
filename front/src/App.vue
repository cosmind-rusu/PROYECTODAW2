<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
import { useAuthStore } from './stores/auth'
import { onMounted, onBeforeMount, provide } from 'vue'
import axios from 'axios'

console.log('App: Component initialization');
const authStore = useAuthStore();

// Provide auth store to all components
provide('authStore', authStore);

// Setup authentication before mounting
onBeforeMount(() => {
  console.log('App: Setting up authentication before mount');
  
  // Configurar interceptores globales una sola vez
  if (!window.interceptorsInitialized) {
    console.log('App: Initializing global interceptors');
    
    // Interceptor de solicitud global
    axios.interceptors.request.use(
      config => {
        console.log(`App: Request to ${config.url}`);
        return config;
      },
      error => {
        console.error('App: Request error:', error);
        return Promise.reject(error);
      }
    );
    
    window.interceptorsInitialized = true;
  }
});

// Initialize auth on mount
onMounted(() => {
  console.log('App: Component mounted, initializing auth');
  authStore.initialize();
  
  // Log auth status
  console.log(`App: Authentication status: ${authStore.isAuthenticated ? 'Authenticated' : 'Not authenticated'}`);
})
</script>

<template>
  <div class="app-container">
    <Navbar />
    <div class="app-content">
      <RouterView />
    </div>
    <Footer />
  </div>
</template>

<style>
html, body, #app {
  height: 100%;
  margin: 0;
}
.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}
.app-content {
  flex: 1;
}
</style>
