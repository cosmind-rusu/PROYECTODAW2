import axios from 'axios';

// Crear una instancia personalizada de Axios con la URL base de las variables de entorno
const apiBase = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api';

console.log('API Base configurada en:', apiBase);

const api = axios.create({
  baseURL: apiBase,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Configurar interceptores para añadir el token de autenticación
api.interceptors.request.use(
  config => {
    // Agregar token de autenticación si existe
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    console.error('Error en interceptor de solicitud:', error);
    return Promise.reject(error);
  }
);

// Interceptor para manejar respuestas y errores
api.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    console.error('Error en respuesta API:', error.response?.data || error.message);
    
    // Si el error es 401 (no autorizado), podemos manejar el cierre de sesión aquí
    if (error.response?.status === 401) {
      console.log('Sesión caducada o no autorizada');
      // Podríamos emitir un evento o llamar a una función para cerrar sesión
    }
    
    return Promise.reject(error);
  }
);

export default api;
