import { defineStore } from 'pinia';
import apiService from '@/api/services';

export const useDashboardStore = defineStore('dashboard', {
  state: () => ({
    consultations: [],
    animalSpecies: [],
    treatments: [],
    healthPlans: [],
    loading: false,
    error: null as string | null
  }),
  
  actions: {
    async fetchDashboardData() {
      console.log('Dashboard: Fetching dashboard data');
      this.loading = true;
      this.error = null;
      
      try {
        // Obtener datos de forma paralela
        const [consultations, animalSpecies, treatments, healthPlans] = await Promise.all([
          apiService.getConsultations(),
          apiService.getAnimalSpecies(),
          apiService.getTreatments(),
          apiService.getHealthPlans()
        ]);
        
        // Actualizar el estado
        this.consultations = consultations;
        this.animalSpecies = animalSpecies;
        this.treatments = treatments;
        this.healthPlans = healthPlans;
        
        console.log('Dashboard: All data fetched successfully');
        return true;
      } catch (error: any) {
        console.error('Dashboard: Error fetching data', error);
        this.error = error?.message || 'Error al cargar los datos del dashboard';
        return false;
      } finally {
        this.loading = false;
      }
    }
  }
});
