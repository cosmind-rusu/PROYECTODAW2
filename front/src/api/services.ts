import http from './http';

// Servicio para manejar las operaciones del API
export const apiService = {
  // ===== GET OPERATIONS =====
  
  // Consultas
  async getConsultations() {
    console.log('Service: Fetching consultations');
    try {
      const response = await http.get('/api/consultations');
      console.log('Service: Consultations fetched successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error fetching consultations', error);
      throw error;
    }
  },

  // Especies de animales
  async getAnimalSpecies() {
    console.log('Service: Fetching animal species');
    try {
      const response = await http.get('/api/animalspecies');
      console.log('Service: Animal species fetched successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error fetching animal species', error);
      throw error;
    }
  },

  // Tratamientos
  async getTreatments() {
    console.log('Service: Fetching treatments');
    try {
      const response = await http.get('/api/treatments');
      console.log('Service: Treatments fetched successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error fetching treatments', error);
      throw error;
    }
  },

  // Planes de salud
  async getHealthPlans() {
    console.log('Service: Fetching health plans');
    try {
      const response = await http.get('/api/healthplans');
      console.log('Service: Health plans fetched successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error fetching health plans', error);
      throw error;
    }
  },
  
  // ===== CREATE OPERATIONS =====
  
  // Crear especie animal
  async createAnimalSpecies(speciesData) {
    console.log('Service: Creating animal species', speciesData);
    try {
      const response = await http.post('/api/animalspecies', speciesData);
      console.log('Service: Animal species created successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error creating animal species', error);
      throw error;
    }
  },
  
  // Crear consulta veterinaria
  async createConsultation(consultationData) {
    console.log('Service: Creating consultation', consultationData);
    try {
      const response = await http.post('/api/consultations', consultationData);
      console.log('Service: Consultation created successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error creating consultation', error);
      throw error;
    }
  },
  
  // Crear tratamiento
  async createTreatment(treatmentData) {
    console.log('Service: Creating treatment', treatmentData);
    try {
      const response = await http.post('/api/treatments', treatmentData);
      console.log('Service: Treatment created successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error creating treatment', error);
      throw error;
    }
  },
  
  // Crear plan de salud
  async createHealthPlan(healthPlanData) {
    console.log('Service: Creating health plan', healthPlanData);
    try {
      const response = await http.post('/api/healthplans', healthPlanData);
      console.log('Service: Health plan created successfully', response.data);
      return response.data;
    } catch (error) {
      console.error('Service: Error creating health plan', error);
      throw error;
    }
  }
};

export default apiService;
