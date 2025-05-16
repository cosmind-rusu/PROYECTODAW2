<script setup lang="ts">
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted } from 'vue'

// Estado para el dashboard
const stats = ref({
  totalPets: 0,
  todayAppointments: 0,
  pendingAppointments: 0,
  treatments: 0
})

const recentConsultations = ref([])

// Cargar datos del dashboard
onMounted(async () => {
  await loadDashboardData()
})

// Cargar datos del dashboard
async function loadDashboardData() {
  try {
    const config = useAuthStore().authHeader
    
    // Cargar consultas recientes (últimas 5)
    const { data: consultations } = await axios.get('/api/consultations', config)
    recentConsultations.value = consultations.slice(0, 5)
    
    // Simulamos estadísticas básicas
    stats.value = {
      totalPets: consultations.length,
      todayAppointments: consultations.filter(c => {
        const today = new Date().toISOString().split('T')[0]
        return c.consultationDate.startsWith(today)
      }).length,
      pendingAppointments: consultations.filter(c => 
        c.followUpDate && new Date(c.followUpDate) >= new Date()
      ).length,
      treatments: [...new Set(consultations.map(c => c.treatmentId))].length
    }
  } catch (err) {
    console.error('Error loading dashboard data:', err)
  }
}

// Registrar nueva especie animal
const createAnimalSpecies = async () => {
  const name = prompt('Animal Species Name? (e.g. Dog, Cat, Bird)')
  if (!name) return
  
  const description = prompt('Description of the species?') || ''
  
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.post('/api/animalspecies', { 
      name, 
      description, 
      isActive: true,
      commonIssues: '',
      careInstructions: ''
    }, config)
    
    alert(`Animal Species registered: ${data.name}`)
  } catch (err) {
    console.error(err)
    alert('Error registering animal species')
  }
}

// Registrar nueva consulta veterinaria
const createConsultation = async () => {
  const speciesId = prompt('Animal Species ID?')
  if (!speciesId) return
  
  const petName = prompt('Pet Name?')
  if (!petName) return
  
  const ownerName = prompt('Owner Name?')
  if (!ownerName) return
  
  const contactInfo = prompt('Contact Information?')
  if (!contactInfo) return
  
  const treatmentId = prompt('Treatment ID?')
  if (!treatmentId) return
  
  const cost = prompt('Cost of consultation?')
  if (!cost) return
  
  const description = prompt('Diagnosis/Description?') || ''
  const date = prompt('Date (YYYY-MM-DD)?', new Date().toISOString().split('T')[0])
  
  try {
    const config = useAuthStore().authHeader
    const payload = { 
      animalSpeciesId: Number(speciesId),
      treatmentId: Number(treatmentId),
      petName,
      ownerName,
      contactInfo,
      cost: Number(cost),
      description,
      consultationDate: date,
      treatmentNotes: '',
      prescription: '',
      isExpense: true // Para mantener compatibilidad
    }
    
    const { data } = await axios.post('/api/consultations', payload, config)
    alert(`Consultation registered with ID: ${data.id}`)
    await loadDashboardData() // Refrescar datos
  } catch (err) {
    console.error(err)
    alert('Error registering consultation')
  }
}

// Registrar un nuevo tratamiento
const createTreatment = async () => {
  const name = prompt('Treatment Name?')
  if (!name) return
  
  const description = prompt('Treatment Description?')
  if (!description) return
  
  const cost = prompt('Default Cost?', '0')
  
  try {
    const config = useAuthStore().authHeader
    const payload = { 
      name, 
      description,
      defaultCost: Number(cost),
      isActive: true,
      isExpense: true // Para mantener compatibilidad
    }
    
    const { data } = await axios.post('/api/treatments', payload, config)
    alert(`Treatment registered: ${data.name}`)
  } catch (err) {
    console.error(err)
    alert('Error registering treatment')
  }
}
</script>

<template>
  <main class="dashboard">
    <h1>Veterinary Clinic Dashboard</h1>
    
    <!-- Estadísticas generales -->
    <div class="stats-container">
      <div class="stat-card">
        <div class="stat-number">{{ stats.totalPets }}</div>
        <div class="stat-label">Total Pets</div>
      </div>
      
      <div class="stat-card">
        <div class="stat-number">{{ stats.todayAppointments }}</div>
        <div class="stat-label">Today's Appointments</div>
      </div>
      
      <div class="stat-card">
        <div class="stat-number">{{ stats.pendingAppointments }}</div>
        <div class="stat-label">Pending Follow-ups</div>
      </div>
      
      <div class="stat-card">
        <div class="stat-number">{{ stats.treatments }}</div>
        <div class="stat-label">Available Treatments</div>
      </div>
    </div>
    
    <div class="dashboard-content">
      <!-- Sección de navegación rápida -->
      <div class="quick-links">
        <h2>Clinic Management</h2>
        <router-link class="dashboard__btn" to="/categories">Animal Species</router-link>
        <router-link class="dashboard__btn" to="/consultations">Consultations</router-link>
        <router-link class="dashboard__btn" to="/treatments">Treatments</router-link>
        <router-link class="dashboard__btn" to="/healthplans">Health Plans</router-link>
      </div>
      
      <!-- Sección de acciones rápidas -->
      <div class="quick-actions">
        <h2>Quick Actions</h2>
        <button class="dashboard__btn primary" @click="createConsultation">New Consultation</button>
        <button class="dashboard__btn" @click="createAnimalSpecies">New Animal Species</button>
        <button class="dashboard__btn" @click="createTreatment">New Treatment</button>
      </div>
    </div>
    
    <!-- Lista de consultas recientes -->
    <div class="recent-appointments">
      <h2>Recent Consultations</h2>
      <div v-if="recentConsultations.length === 0" class="empty-state">
        No consultations recorded yet
      </div>
      <table v-else>
        <thead>
          <tr>
            <th>Date</th>
            <th>Pet</th>
            <th>Owner</th>
            <th>Diagnosis</th>
            <th>Treatment</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="consultation in recentConsultations" :key="consultation.id">
            <td>{{ new Date(consultation.consultationDate).toLocaleDateString() }}</td>
            <td>{{ consultation.petName }}</td>
            <td>{{ consultation.ownerName }}</td>
            <td>{{ consultation.description }}</td>
            <td>{{ consultation.treatmentName || 'Unknown' }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </main>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.dashboard {
  padding: $spacing-unit * 1.5;
  max-width: 1200px;
  margin: 0 auto;
  
  h1 {
    color: $primary-color;
    margin-bottom: $spacing-unit * 1.5;
    font-size: 2rem;
  }
  
  h2 {
    color: $secondary-color;
    margin-bottom: $spacing-unit;
    font-size: 1.3rem;
  }
  
  // Contenedor de estadísticas
  .stats-container {
    display: flex;
    justify-content: space-between;
    gap: $spacing-unit;
    margin-bottom: $spacing-unit * 2;
    
    .stat-card {
      flex: 1;
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      padding: $spacing-unit;
      text-align: center;
      
      .stat-number {
        font-size: 2rem;
        font-weight: bold;
        color: $primary-color;
      }
      
      .stat-label {
        font-size: 0.9rem;
        color: #666;
        margin-top: $spacing-unit / 2;
      }
    }
  }
  
  // Layout de contenido
  .dashboard-content {
    display: flex;
    gap: $spacing-unit * 2;
    margin-bottom: $spacing-unit * 2;
    
    .quick-links,
    .quick-actions {
      flex: 1;
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      padding: $spacing-unit;
    }
  }
  
  // Lista de consultas recientes
  .recent-appointments {
    background-color: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    padding: $spacing-unit;
    
    table {
      width: 100%;
      border-collapse: collapse;
      
      th, td {
        padding: $spacing-unit / 2;
        text-align: left;
        border-bottom: 1px solid #eee;
      }
      
      th {
        color: $secondary-color;
        font-weight: bold;
      }
      
      tr:hover td {
        background-color: #f9f9f9;
      }
    }
    
    .empty-state {
      padding: $spacing-unit;
      color: #666;
      font-style: italic;
      text-align: center;
    }
  }
  
  // Botones
  &__btn {
    display: block;
    width: 100%;
    margin-bottom: $spacing-unit / 2;
    padding: ($spacing-unit / 2) $spacing-unit;
    background-color: $primary-color;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    text-align: center;
    text-decoration: none;
    transition: background-color 0.2s ease;
    
    &:hover {
      background-color: darken($primary-color, 5%);
    }
    
    &.primary {
      background-color: $secondary-color;
      font-weight: bold;
      
      &:hover {
        background-color: darken($secondary-color, 5%);
      }
    }
  }
}
</style>
