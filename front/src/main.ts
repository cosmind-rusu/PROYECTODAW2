import axios from 'axios'
axios.defaults.baseURL = import.meta.env.VITE_API_URL

import './assets/styles/main.scss'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { useAuthStore } from './stores/auth'

import App from './App.vue'
import router from './router'

const app = createApp(App)

const pinia = createPinia()
app.use(pinia)
app.use(router)

// Initialize auth store
const authStore = useAuthStore(pinia)
authStore.initialize()

app.mount('#app')
