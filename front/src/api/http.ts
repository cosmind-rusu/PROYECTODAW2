import axios from 'axios';

// Crear una instancia de Axios personalizada
const http = axios.create();

// Configuración de interceptores para la instancia personalizada
http.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    console.log('API: Preparing request with token:', !!token);
    
    // Siempre establecer cabeceras como un nuevo objeto para evitar referencias compartidas
    config.headers = config.headers || {};
    
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
      console.log('API: Token added to request headers');
    }
    
    return config;
  },
  error => {
    console.error('API: Request error:', error);
    return Promise.reject(error);
  }
);

// Interceptor de respuesta
http.interceptors.response.use(
  response => {
    console.log(`API: Response success [${response.status}] ${response.config.url}`);
    return response;
  },
  error => {
    console.error(`API: Response error [${error.response?.status || 'Unknown'}] ${error.config?.url || 'Unknown URL'}`, error.response?.data);
    return Promise.reject(error);
  }
);

export default http;
