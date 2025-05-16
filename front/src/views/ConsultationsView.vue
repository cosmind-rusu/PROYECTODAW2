<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

// Definir la interfaz para consultas veterinarias
interface Consultation {
  id: number
  petName: string
  ownerName: string
  contactInfo: string
  animalSpeciesId: number
  treatmentId: number
  cost: number
  description: string
  treatmentNotes: string
  prescription: string
  consultationDate: string
  followUpDate?: string
  animalSpeciesName?: string
  treatmentName?: string
}

// Referencias reactivas
const consultations = ref<Consultation[]>([])
const animalSpecies = ref<any[]>([])
const treatments = ref<any[]>([])

const newConsultation = ref({
  animalSpeciesId: null as number | null,
  treatmentId: null as number | null,
  petName: '',
  ownerName: '',
  contactInfo: '',
  cost: null as number | null,
  description: '',
  treatmentNotes: '',
  prescription: '',
  consultationDate: new Date().toISOString().split('T')[0],
  followUpDate: null as string | null,
  isExpense: true // Para mantener compatibilidad
})

const editingConsultation = ref<Consultation | null>(null)
const loading = ref({
  consultations: false,
  species: false,
  treatments: false
})
const showForm = ref(false)
const showFilters = ref(false)
const filterDate = ref<string>('')
const filterSpecies = ref<number | null>(null)
const filterPetName = ref<string>('')

// Cargar datos al montar el componente
onMounted(async () => {
  await Promise.all([
    loadConsultations(),
    loadAnimalSpecies(),
    loadTreatments()
  ])
})

// Cargar consultas desde la API
async function loadConsultations() {
  loading.value.consultations = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/consultations', config)
    consultations.value = data.map((c: any) => ({
      ...c,
      // Convertir fecha de ISO a formato local para mostrar
      consultationDate: c.consultationDate.split('T')[0],
      followUpDate: c.followUpDate ? c.followUpDate.split('T')[0] : null
    }))
  } catch (err) {
    console.error('Error loading consultations:', err)
    alert('Error loading consultations')
  } finally {
    loading.value.consultations = false
  }
}

// Cargar especies animales
async function loadAnimalSpecies() {
  loading.value.species = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/animalspecies', config)
    animalSpecies.value = data
  } catch (err) {
    console.error('Error loading animal species:', err)
  } finally {
    loading.value.species = false
  }
}

// Cargar tratamientos
async function loadTreatments() {
  loading.value.treatments = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/treatments', config)
    treatments.value = data
  } catch (err) {
    console.error('Error loading treatments:', err)
  } finally {
    loading.value.treatments = false
  }
}

// Crear una nueva consulta veterinaria
async function createConsultation() {
  if (!newConsultation.value.animalSpeciesId || 
      !newConsultation.value.treatmentId || 
      !newConsultation.value.petName ||
      !newConsultation.value.ownerName ||
      !newConsultation.value.cost) return
  
  try {
    const config = useAuthStore().authHeader
    const payload = {
      ...newConsultation.value,
      cost: Number(newConsultation.value.cost)
    }
    
    const { data } = await axios.post('/api/consultations', payload, config)
    
    // Agregar nombres para UI
    const species = animalSpecies.value.find(s => s.id === data.animalSpeciesId)
    const treatment = treatments.value.find(t => t.id === data.treatmentId)
    
    consultations.value.unshift({
      ...data,
      animalSpeciesName: species?.name || 'Unknown',
      treatmentName: treatment?.name || 'Unknown',
      consultationDate: data.consultationDate.split('T')[0],
      followUpDate: data.followUpDate ? data.followUpDate.split('T')[0] : null
    })
    
    resetForm()
  } catch (err) {
    console.error('Error creating consultation:', err)
    alert('Error creating consultation')
  }
}

// Resetear el formulario
function resetForm() {
  newConsultation.value = {
    animalSpeciesId: null,
    treatmentId: null,
    petName: '',
    ownerName: '',
    contactInfo: '',
    cost: null,
    description: '',
    treatmentNotes: '',
    prescription: '',
    consultationDate: new Date().toISOString().split('T')[0],
    followUpDate: null,
    isExpense: true
  }
  showForm.value = false
}

// Comenzar a editar una consulta
function startEdit(consultation: Consultation) {
  editingConsultation.value = { ...consultation }
}

// Cancelar edición
function cancelEdit() {
  editingConsultation.value = null
}

// Guardar cambios en una consulta
async function updateConsultation() {
  if (!editingConsultation.value) return
  try {
    const config = useAuthStore().authHeader
    const payload = {
      ...editingConsultation.value,
      cost: Number(editingConsultation.value.cost)
    }
    
    await axios.put(`/api/consultations/${editingConsultation.value.id}`, payload, config)
    
    // Actualizar la consulta en el array local
    const index = consultations.value.findIndex(c => c.id === editingConsultation.value?.id)
    if (index >= 0) {
      // Mantener/actualizar los nombres para UI
      const species = animalSpecies.value.find(s => s.id === editingConsultation.value?.animalSpeciesId)
      const treatment = treatments.value.find(t => t.id === editingConsultation.value?.treatmentId)
      
      consultations.value[index] = {
        ...editingConsultation.value,
        animalSpeciesName: species?.name || 'Unknown',
        treatmentName: treatment?.name || 'Unknown'
      }
    }
    editingConsultation.value = null
  } catch (err) {
    console.error('Error updating consultation:', err)
    alert('Error updating consultation')
  }
}

// Eliminar una consulta
async function deleteConsultation(id: number) {
  if (!confirm('Delete this consultation?')) return
  try {
    const config = useAuthStore().authHeader
    await axios.delete(`/api/consultations/${id}`, config)
    consultations.value = consultations.value.filter(c => c.id !== id)
  } catch (err) {
    console.error('Error deleting consultation:', err)
    alert('Error deleting consultation')
  }
}

// Consultas filtradas
const filteredConsultations = computed(() => {
  let result = [...consultations.value]
  
  // Filtrar por fecha
  if (filterDate.value) {
    result = result.filter(c => c.consultationDate === filterDate.value)
  }
  
  // Filtrar por especie
  if (filterSpecies.value) {
    result = result.filter(c => c.animalSpeciesId === filterSpecies.value)
  }
  
  // Filtrar por nombre de mascota
  if (filterPetName.value) {
    const search = filterPetName.value.toLowerCase()
    result = result.filter(c => c.petName.toLowerCase().includes(search))
  }
  
  return result.sort((a, b) => {
    // Ordenar por fecha descendente (más reciente primero)
    return new Date(b.consultationDate).getTime() - new Date(a.consultationDate).getTime()
  })
})

// Consultas por especie para estadísticas
const consultationsBySpecies = computed(() => {
  const counts: Record<string, number> = {}
  
  consultations.value.forEach(c => {
    const speciesName = c.animalSpeciesName || 'Unknown'
    counts[speciesName] = (counts[speciesName] || 0) + 1
  })
  
  return Object.entries(counts)
    .map(([species, count]) => ({ species, count }))
    .sort((a, b) => b.count - a.count)
})

// Limpiar filtros
function clearFilters() {
  filterDate.value = ''
  filterSpecies.value = null
  filterPetName.value = ''
}

// Obtener el nombre de una especie
function getSpeciesName(id: number) {
  const species = animalSpecies.value.find(s => s.id === id)
  return species?.name || 'Unknown'
}

// Obtener el nombre de un tratamiento
function getTreatmentName(id: number) {
  const treatment = treatments.value.find(t => t.id === id)
  return treatment?.name || 'Unknown'
}
</script>

<template>
  <section class="consultations">
    <h1>Veterinary Consultations</h1>
    
    <div class="consultations__overview">
      <div class="consultations__summary">
        <h2>Consultations Overview</h2>
        <p class="stat-number">{{ consultations.length }} total consultations</p>
      </div>
      
      <div class="consultations__actions">
        <button v-if="!showForm" @click="showForm = true" class="btn-primary">
          New Consultation
        </button>
        <button v-else @click="showForm = false" class="btn-secondary">
          Cancel
        </button>
        
        <button @click="showFilters = !showFilters" class="btn-outline">
          {{ showFilters ? 'Hide Filters' : 'Show Filters' }}
        </button>
      </div>
    </div>
    
    <!-- Filtros -->
    <div v-if="showFilters" class="consultations__filters">
      <h3>Filter Consultations</h3>
      <div class="filters-grid">
        <div class="form-group">
          <label for="filterDate">Date</label>
          <input type="date" id="filterDate" v-model="filterDate">
        </div>
        
        <div class="form-group">
          <label for="filterSpecies">Animal Species</label>
          <select id="filterSpecies" v-model="filterSpecies">
            <option :value="null">All Species</option>
            <option v-for="species in animalSpecies" :key="species.id" :value="species.id">
              {{ species.name }}
            </option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="filterPetName">Pet Name</label>
          <input type="text" id="filterPetName" v-model="filterPetName" placeholder="Search by pet name">
        </div>
        
        <div class="form-group">
          <button @click="clearFilters" class="btn-secondary">Clear Filters</button>
        </div>
      </div>
    </div>

    <!-- Form for adding new consultations -->
    <form v-if="showForm" @submit.prevent="createConsultation" class="consultations__form">
      <div class="form-grid">
        <div class="form-group">
          <label for="animalSpeciesId">Animal Species*</label>
          <select id="animalSpeciesId" v-model="newConsultation.animalSpeciesId" required>
            <option :value="null" disabled>Select Species</option>
            <option v-for="species in animalSpecies" :key="species.id" :value="species.id">
              {{ species.name }}
            </option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="treatmentId">Treatment*</label>
          <select id="treatmentId" v-model="newConsultation.treatmentId" required>
            <option :value="null" disabled>Select Treatment</option>
            <option v-for="treatment in treatments" :key="treatment.id" :value="treatment.id">
              {{ treatment.name }}
            </option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="petName">Pet Name*</label>
          <input type="text" id="petName" v-model="newConsultation.petName" required>
        </div>
        
        <div class="form-group">
          <label for="ownerName">Owner Name*</label>
          <input type="text" id="ownerName" v-model="newConsultation.ownerName" required>
        </div>
        
        <div class="form-group">
          <label for="contactInfo">Contact Info</label>
          <input type="text" id="contactInfo" v-model="newConsultation.contactInfo" placeholder="Phone or email">
        </div>
        
        <div class="form-group">
          <label for="cost">Cost*</label>
          <input type="number" id="cost" v-model="newConsultation.cost" step="0.01" min="0" required>
        </div>
        
        <div class="form-group full-width">
          <label for="description">Diagnosis/Description*</label>
          <textarea id="description" v-model="newConsultation.description" rows="2" required></textarea>
        </div>
        
        <div class="form-group full-width">
          <label for="treatmentNotes">Treatment Notes</label>
          <textarea id="treatmentNotes" v-model="newConsultation.treatmentNotes" rows="2"></textarea>
        </div>
        
        <div class="form-group full-width">
          <label for="prescription">Prescription</label>
          <textarea id="prescription" v-model="newConsultation.prescription" rows="2"></textarea>
        </div>
        
        <div class="form-group">
          <label for="consultationDate">Consultation Date*</label>
          <input type="date" id="consultationDate" v-model="newConsultation.consultationDate" required>
        </div>
        
        <div class="form-group">
          <label for="followUpDate">Follow-up Date</label>
          <input type="date" id="followUpDate" v-model="newConsultation.followUpDate">
        </div>
      </div>
      
      <div class="form-actions">
        <button type="submit" class="btn-primary">Save Consultation</button>
        <button type="button" @click="resetForm" class="btn-secondary">Cancel</button>
      </div>
    </form>

    <div v-if="loading.consultations || loading.species || loading.treatments" class="loading">
      Loading data...
    </div>
    
    <!-- Consultations list -->
    <div v-else class="consultations__content">
      <div class="consultations__stats">
        <h3>Consultations by Species</h3>
        <div class="stat-bars">
          <div v-for="(item, index) in consultationsBySpecies" :key="index" class="stat-bar-item">
            <div class="stat-label">{{ item.species }}</div>
            <div class="stat-bar-container">
              <div class="stat-bar" :style="{ width: `${(item.count / consultations.length) * 100}%` }"></div>
              <span class="stat-value">{{ item.count }}</span>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Table of consultations -->
      <div v-if="filteredConsultations.length > 0" class="consultations__table">
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Pet</th>
              <th>Owner</th>
              <th>Species</th>
              <th>Treatment</th>
              <th>Diagnosis</th>
              <th>Cost</th>
              <th>Follow-up</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="consultation in filteredConsultations" :key="consultation.id"
                :class="{ 'being-edited': editingConsultation && editingConsultation.id === consultation.id }">
              <template v-if="editingConsultation && editingConsultation.id === consultation.id">
                <td>
                  <input type="date" v-model="editingConsultation.consultationDate">
                </td>
                <td>
                  <input type="text" v-model="editingConsultation.petName">
                </td>
                <td>
                  <input type="text" v-model="editingConsultation.ownerName">
                </td>
                <td>
                  <select v-model="editingConsultation.animalSpeciesId">
                    <option v-for="species in animalSpecies" :key="species.id" :value="species.id">
                      {{ species.name }}
                    </option>
                  </select>
                </td>
                <td>
                  <select v-model="editingConsultation.treatmentId">
                    <option v-for="treatment in treatments" :key="treatment.id" :value="treatment.id">
                      {{ treatment.name }}
                    </option>
                  </select>
                </td>
                <td>
                  <input type="text" v-model="editingConsultation.description">
                </td>
                <td>
                  <input type="number" v-model="editingConsultation.cost" step="0.01" min="0">
                </td>
                <td>
                  <input type="date" v-model="editingConsultation.followUpDate">
                </td>
                <td>
                  <button @click="updateConsultation" class="btn-small">Save</button>
                  <button @click="cancelEdit" class="btn-small btn-secondary">Cancel</button>
                </td>
              </template>
              <template v-else>
                <td>{{ new Date(consultation.consultationDate).toLocaleDateString() }}</td>
                <td>{{ consultation.petName }}</td>
                <td>{{ consultation.ownerName }}</td>
                <td>{{ consultation.animalSpeciesName || getSpeciesName(consultation.animalSpeciesId) }}</td>
                <td>{{ consultation.treatmentName || getTreatmentName(consultation.treatmentId) }}</td>
                <td>{{ consultation.description }}</td>
                <td>{{ consultation.cost.toFixed(2) }}€</td>
                <td>
                  <span v-if="consultation.followUpDate">
                    {{ new Date(consultation.followUpDate).toLocaleDateString() }}
                  </span>
                  <span v-else>-</span>
                </td>
                <td>
                  <button @click="startEdit(consultation)" class="btn-small">Edit</button>
                  <button @click="deleteConsultation(consultation.id)" class="btn-small btn-danger">Delete</button>
                </td>
              </template>
            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Empty state -->
      <div v-else-if="consultations.length === 0" class="empty-state">
        <p>No consultations recorded yet.</p>
        <button @click="showForm = true" class="btn-primary">Record First Consultation</button>
      </div>
      
      <!-- No results for filters -->
      <div v-else class="empty-state">
        <p>No consultations match your filter criteria.</p>
        <button @click="clearFilters" class="btn-secondary">Clear Filters</button>
      </div>
    </div>
  </section>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.consultations {
  padding: $spacing-unit * 1.5;
  max-width: 1200px;
  margin: 0 auto;
  
  h1 {
    color: $primary-color;
    margin-bottom: $spacing-unit * 1.5;
    font-size: 2rem;
  }
  
  h2, h3 {
    color: $secondary-color;
    margin-bottom: $spacing-unit;
  }

  &__overview {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
    padding: $spacing-unit;
    background-color: white;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  }

  &__summary {
    .stat-number {
      font-size: 1.5rem;
      font-weight: bold;
      color: $primary-color;
    }
  }

  &__actions {
    display: flex;
    gap: $spacing-unit / 2;
  }
  
  &__filters {
    background-color: white;
    padding: $spacing-unit;
    border-radius: 8px;
    margin-bottom: $spacing-unit;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    
    .filters-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: $spacing-unit;
    }
  }
  
  &__form {
    background-color: white;
    padding: $spacing-unit;
    border-radius: 8px;
    margin-bottom: $spacing-unit;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    
    .form-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: $spacing-unit;
      
      .full-width {
        grid-column: 1 / -1;
      }
    }
    
    .form-group {
      margin-bottom: $spacing-unit / 2;
      
      label {
        display: block;
        margin-bottom: 4px;
        font-weight: 500;
      }
      
      input[type="text"], 
      input[type="number"],
      input[type="date"],
      select,
      textarea {
        width: 100%;
        padding: $spacing-unit / 2;
        border: 1px solid #ddd;
        border-radius: 4px;
      }
    }
    
    .form-actions {
      display: flex;
      justify-content: flex-start;
      gap: $spacing-unit / 2;
      margin-top: $spacing-unit;
    }
  }
  
  &__content {
    margin-top: $spacing-unit;
  }
  
  &__stats {
    background-color: white;
    padding: $spacing-unit;
    margin-bottom: $spacing-unit;
    border-radius: 8px;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    
    .stat-bars {
      .stat-bar-item {
        display: flex;
        align-items: center;
        margin-bottom: 8px;
        
        .stat-label {
          width: 100px;
          font-weight: 500;
        }
        
        .stat-bar-container {
          flex: 1;
          height: 20px;
          background-color: #eee;
          border-radius: 4px;
          position: relative;
          overflow: hidden;
          
          .stat-bar {
            height: 100%;
            background-color: $primary-color;
            border-radius: 4px;
          }
          
          .stat-value {
            position: absolute;
            right: 8px;
            top: 0;
            font-size: 0.8rem;
            line-height: 20px;
            color: #333;
          }
        }
      }
    }
  }
  
  &__table {
    overflow-x: auto;
    
    table {
      width: 100%;
      border-collapse: collapse;
      margin-bottom: $spacing-unit;
      background-color: white;
      box-shadow: 0 2px 6px rgba(0,0,0,0.1);
      border-radius: 8px;
      overflow: hidden;
      
      th {
        text-align: left;
        padding: $spacing-unit / 2;
        background-color: $primary-color;
        color: white;
        white-space: nowrap;
      }
      
      td {
        padding: $spacing-unit / 2;
        border-bottom: 1px solid #eee;
        
        input, select {
          width: 100%;
          padding: 4px;
          border: 1px solid #ccc;
          border-radius: 4px;
        }
      }
      
      tr:hover:not(.being-edited) {
        background-color: #f9f9f9;
      }
      
      tr.being-edited {
        background-color: #f5f5f5;
      }
    }
  }
  
  .empty-state {
    text-align: center;
    padding: $spacing-unit * 2;
    background-color: white;
    border-radius: 8px;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    
    p {
      margin-bottom: $spacing-unit;
      font-style: italic;
      color: #666;
    }
  }
  
  .loading {
    text-align: center;
    padding: $spacing-unit;
    color: $secondary-color;
    font-style: italic;
  }
}

// Estilos de botones
.btn-primary {
  background-color: $primary-color;
  color: white;
  border: none;
  padding: ($spacing-unit / 2) $spacing-unit;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  
  &:hover {
    background-color: darken($primary-color, 5%);
  }
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
  border: none;
  padding: ($spacing-unit / 2) $spacing-unit;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  
  &:hover {
    background-color: darken(#6c757d, 5%);
  }
}

.btn-outline {
  background-color: transparent;
  color: $primary-color;
  border: 1px solid $primary-color;
  padding: ($spacing-unit / 2) $spacing-unit;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  
  &:hover {
    background-color: rgba($primary-color, 0.1);
  }
}

.btn-danger {
  background-color: #dc3545;
  color: white;
  
  &:hover {
    background-color: darken(#dc3545, 5%);
  }
}

.btn-small {
  padding: ($spacing-unit / 4) ($spacing-unit / 2);
  font-size: 0.9rem;
}
</style>
