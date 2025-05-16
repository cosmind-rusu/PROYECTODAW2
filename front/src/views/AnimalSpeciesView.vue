<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

// Definir la interfaz para especies animales (anteriormente Category en el backend)
interface AnimalSpecies {
  id: number
  name: string
  description: string
  isActive: boolean
  commonIssues: string
  careInstructions: string
  consultationsCount?: number // Cantidad de consultas asociadas
  createdDate: string
}

// Referencias reactivas
const animalSpecies = ref<AnimalSpecies[]>([])
const consultations = ref<any[]>([])
const newSpecies = ref({
  name: '',
  description: '',
  commonIssues: '',
  careInstructions: '',
  isActive: true
})

const editingSpecies = ref<AnimalSpecies | null>(null)
const loading = ref({
  species: false,
  consultations: false
})
const showForm = ref(false)

// Cargar especies al montar el componente
onMounted(async () => {
  await loadAnimalSpecies()
  await loadConsultations() // Cargar consultas para calcular estadísticas
})

// Método para cargar especies desde la API
async function loadAnimalSpecies() {
  loading.value.species = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/animalspecies', config)
    animalSpecies.value = data
  } catch (err) {
    console.error('Error loading animal species:', err)
    alert('Error loading animal species')
  } finally {
    loading.value.species = false
  }
}

// Método para cargar consultas (para calcular estadísticas)
async function loadConsultations() {
  loading.value.consultations = true
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.get('/api/consultations', config)
    consultations.value = data
    calculateConsultationStats()
  } catch (err) {
    console.error('Error loading consultations:', err)
  } finally {
    loading.value.consultations = false
  }
}

// Calcular estadísticas por especie
function calculateConsultationStats() {
  animalSpecies.value.forEach(species => {
    const speciesConsultations = consultations.value.filter(c => c.animalSpeciesId === species.id)
    species.consultationsCount = speciesConsultations.length
  })
}

// Crear una nueva especie animal
async function createSpecies() {
  if (!newSpecies.value.name) return
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.post('/api/animalspecies', newSpecies.value, config)
    animalSpecies.value.push({...data, consultationsCount: 0})
    newSpecies.value = { 
      name: '', 
      description: '', 
      commonIssues: '',
      careInstructions: '',
      isActive: true 
    }
    showForm.value = false
  } catch (err) {
    console.error('Error creating animal species:', err)
    alert('Error creating animal species')
  }
}

// Comenzar a editar una especie
function startEdit(species: AnimalSpecies) {
  editingSpecies.value = { ...species }
}

// Cancelar edición
function cancelEdit() {
  editingSpecies.value = null
}

// Guardar cambios en una especie
async function updateSpecies() {
  if (!editingSpecies.value) return
  try {
    const config = useAuthStore().authHeader
    await axios.put(`/api/animalspecies/${editingSpecies.value.id}`, editingSpecies.value, config)
    
    // Actualizar la especie en el array local
    const index = animalSpecies.value.findIndex(s => s.id === editingSpecies.value?.id)
    if (index >= 0) {
      animalSpecies.value[index] = { ...editingSpecies.value }
    }
    editingSpecies.value = null
  } catch (err) {
    console.error('Error updating animal species:', err)
    alert('Error updating animal species')
  }
}

// Eliminar una especie
async function deleteSpecies(id: number) {
  if (!confirm('Delete this animal species? This will NOT delete associated consultations.')) return
  try {
    const config = useAuthStore().authHeader
    await axios.delete(`/api/animalspecies/${id}`, config)
    animalSpecies.value = animalSpecies.value.filter(s => s.id !== id)
  } catch (err) {
    console.error('Error deleting animal species:', err)
    alert('Error deleting animal species')
  }
}

// Especies activas para mostrar
const activeSpecies = computed(() => {
  return animalSpecies.value.filter(s => s.isActive)
})

// Especies inactivas para mostrar
const inactiveSpecies = computed(() => {
  return animalSpecies.value.filter(s => !s.isActive)
})

// Total de especies
const totalSpecies = computed(() => {
  return animalSpecies.value.length
})
</script>

<template>
  <section class="animal-species">
    <h1>Animal Species Manager</h1>
    
    <div class="animal-species__overview">
      <div class="animal-species__summary">
        <h2>Species Overview</h2>
        <p class="species-count">{{ totalSpecies }} registered species</p>
      </div>
      
      <div class="animal-species__actions">
        <button v-if="!showForm" @click="showForm = true" class="btn-primary">
          Add New Species
        </button>
        <button v-else @click="showForm = false" class="btn-secondary">
          Cancel
        </button>
      </div>
    </div>

    <!-- Form for adding new animal species -->
    <form v-if="showForm" @submit.prevent="createSpecies" class="animal-species__form">
      <div class="form-group">
        <label for="name">Species Name</label>
        <input type="text" id="name" v-model="newSpecies.name" required placeholder="e.g. Dog, Cat, Bird">
      </div>
      
      <div class="form-group">
        <label for="description">Description</label>
        <textarea id="description" v-model="newSpecies.description" rows="2" placeholder="General description of the species"></textarea>
      </div>
      
      <div class="form-group">
        <label for="commonIssues">Common Health Issues</label>
        <textarea id="commonIssues" v-model="newSpecies.commonIssues" rows="3" placeholder="Common health problems for this species"></textarea>
      </div>
      
      <div class="form-group">
        <label for="careInstructions">Care Instructions</label>
        <textarea id="careInstructions" v-model="newSpecies.careInstructions" rows="3" placeholder="General care instructions"></textarea>
      </div>
      
      <div class="form-group checkbox">
        <input type="checkbox" id="isActive" v-model="newSpecies.isActive">
        <label for="isActive">Active</label>
      </div>
      
      <div class="form-actions">
        <button type="submit" class="btn-primary">Save</button>
        <button type="button" @click="showForm = false" class="btn-secondary">Cancel</button>
      </div>
    </form>

    <div v-if="loading.species" class="loading">Loading animal species...</div>
    
    <!-- Active species table -->
    <div v-if="activeSpecies.length > 0" class="animal-species__list">
      <h2>Active Species</h2>
      <table>
        <thead>
          <tr>
            <th>Species Name</th>
            <th>Description</th>
            <th>Consultations</th>
            <th>Created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="species in activeSpecies" :key="species.id" 
              :class="{ 'being-edited': editingSpecies && editingSpecies.id === species.id }">
            <template v-if="editingSpecies && editingSpecies.id === species.id">
              <td><input v-model="editingSpecies.name" /></td>
              <td><input v-model="editingSpecies.description" /></td>
              <td>{{ species.consultationsCount || 0 }}</td>
              <td>{{ new Date(species.createdDate).toLocaleDateString() }}</td>
              <td>
                <button @click="updateSpecies" class="btn-small">Save</button>
                <button @click="cancelEdit" class="btn-small btn-secondary">Cancel</button>
              </td>
            </template>
            <template v-else>
              <td>{{ species.name }}</td>
              <td>{{ species.description }}</td>
              <td>{{ species.consultationsCount || 0 }}</td>
              <td>{{ new Date(species.createdDate).toLocaleDateString() }}</td>
              <td>
                <button @click="startEdit(species)" class="btn-small">Edit</button>
                <button @click="deleteSpecies(species.id)" class="btn-small btn-danger">Delete</button>
              </td>
            </template>
          </tr>
        </tbody>
      </table>
    </div>
    
    <!-- Inactive species table (collapsed by default) -->
    <div v-if="inactiveSpecies.length > 0" class="animal-species__list inactive">
      <details>
        <summary>Inactive Species ({{ inactiveSpecies.length }})</summary>
        <table>
          <thead>
            <tr>
              <th>Species Name</th>
              <th>Description</th>
              <th>Consultations</th>
              <th>Created</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="species in inactiveSpecies" :key="species.id"
                :class="{ 'being-edited': editingSpecies && editingSpecies.id === species.id }">
              <template v-if="editingSpecies && editingSpecies.id === species.id">
                <td><input v-model="editingSpecies.name" /></td>
                <td><input v-model="editingSpecies.description" /></td>
                <td>{{ species.consultationsCount || 0 }}</td>
                <td>{{ new Date(species.createdDate).toLocaleDateString() }}</td>
                <td>
                  <button @click="updateSpecies" class="btn-small">Save</button>
                  <button @click="cancelEdit" class="btn-small btn-secondary">Cancel</button>
                </td>
              </template>
              <template v-else>
                <td>{{ species.name }}</td>
                <td>{{ species.description }}</td>
                <td>{{ species.consultationsCount || 0 }}</td>
                <td>{{ new Date(species.createdDate).toLocaleDateString() }}</td>
                <td>
                  <button @click="startEdit(species)" class="btn-small">Edit</button>
                  <button @click="deleteSpecies(species.id)" class="btn-small btn-danger">Delete</button>
                </td>
              </template>
            </tr>
          </tbody>
        </table>
      </details>
    </div>
    
    <!-- Empty state -->
    <div v-if="!loading.species && animalSpecies.length === 0" class="empty-state">
      <p>No animal species registered yet.</p>
      <button @click="showForm = true" class="btn-primary">Add New Species</button>
    </div>
    
    <!-- Additional information -->
    <div v-if="activeSpecies.length > 0" class="species-detail-info">
      <h2>Species Information</h2>
      <p class="info-text">
        Click on 'Edit' to update species details, including care instructions and common health issues.
        Each species can have multiple consultations associated with it.
      </p>
    </div>
  </section>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.animal-species {
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
    .species-count {
      font-size: 1.5rem;
      font-weight: bold;
      color: $primary-color;
    }
  }

  &__actions {
    display: flex;
    gap: $spacing-unit / 2;
  }
  
  &__form {
    background-color: white;
    padding: $spacing-unit;
    border-radius: 8px;
    margin-bottom: $spacing-unit;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    
    .form-group {
      margin-bottom: $spacing-unit / 2;
      
      label {
        display: block;
        margin-bottom: 4px;
        font-weight: 500;
      }
      
      input[type="text"], 
      textarea,
      input:not([type="checkbox"]) {
        width: 100%;
        padding: $spacing-unit / 2;
        border: 1px solid #ddd;
        border-radius: 4px;
      }
      
      &.checkbox {
        display: flex;
        align-items: center;
        
        input {
          margin-right: 8px;
        }
      }
    }
    
    .form-actions {
      display: flex;
      justify-content: flex-start;
      gap: $spacing-unit / 2;
      margin-top: $spacing-unit;
    }
  }
  
  &__list {
    margin-top: $spacing-unit;
    
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
      }
      
      td {
        padding: $spacing-unit / 2;
        border-bottom: 1px solid #eee;
      }
      
      tr:hover:not(.being-edited) {
        background-color: #f9f9f9;
      }
      
      tr.being-edited {
        background-color: #f5f5f5;
        
        td {
          padding: ($spacing-unit / 4);
          
          input {
            width: 100%;
            padding: ($spacing-unit / 4);
            border: 1px solid #ccc;
            border-radius: 4px;
          }
        }
      }
    }
    
    &.inactive {
      opacity: 0.8;
      
      summary {
        cursor: pointer;
        padding: $spacing-unit / 2;
        background-color: white;
        border-radius: 8px;
        box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        font-weight: 500;
        margin-bottom: $spacing-unit / 2;
      }
      
      details[open] summary {
        margin-bottom: $spacing-unit;
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
  
  .species-detail-info {
    margin-top: $spacing-unit * 2;
    padding: $spacing-unit;
    background-color: #f8f9fa;
    border-left: 4px solid $secondary-color;
    border-radius: 4px;
    
    .info-text {
      color: #555;
      line-height: 1.5;
    }
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
