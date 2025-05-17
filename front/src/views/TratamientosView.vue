<template>
  <div class="tratamientos">
    <div class="tratamientos__header">
      <div>
        <h1 class="tratamientos__title">Tratamientos Veterinarios</h1>
        <p class="tratamientos__subtitle">Gestiona los tratamientos disponibles</p>
      </div>
      <button @click="mostrarFormulario = true" class="btn btn--primary tratamientos__add">
        + Nuevo Tratamiento
      </button>
    </div>
    
    <TratamientoForm
      v-if="mostrarFormulario"
      @cerrar="mostrarFormulario = false"
      @guardado="onTratamientoGuardado"
    />
    
    <div class="tratamientos__content">
      <div v-if="store.loading" class="loading">Cargando tratamientos...</div>
      <div v-else-if="store.error" class="error">
        <p>{{ store.error }}</p>
        <button @click="cargar" class="btn btn--primary">Reintentar</button>
      </div>
      <div v-else-if="store.tratamientos.length === 0" class="empty">
        <p>No hay tratamientos registrados.</p>
      </div>
      <table v-else class="tratamientos__table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Costo</th>
            <th>Duración</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="t in store.tratamientos" :key="t.id">
            <td>{{ t.nombre }}</td>
            <td class="descripcion">{{ t.descripcion }}</td>
            <td>{{ t.costoEstandar }}</td>
            <td>{{ t.duracionEstimadaMinutos }} min</td>
            <td>{{ t.activo ? 'Sí' : 'No' }}</td>
            <td>
              <button @click="onEliminar(t.id)" class="btn btn--danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useTratamientosStore } from '@/stores/tratamientos';
import TratamientoForm from '@/components/tratamientos/TratamientoForm.vue';

const store = useTratamientosStore();
const mostrarFormulario = ref(false);

const cargar = async () => {
  try { await store.obtenerTratamientos(); }
  catch (e) { console.error('Error al cargar tratamientos:', e); }
};

onMounted(cargar);

const onTratamientoGuardado = () => {
  cargar();
  mostrarFormulario.value = false;
};

// Función para eliminar un tratamiento
async function onEliminar(id: number) {
  if (!confirm('¿Estás seguro de eliminar este tratamiento?')) return;
  try {
    await store.eliminarTratamiento(id);
    cargar();
  } catch (err) {
    console.error('Error al eliminar tratamiento:', err);
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.tratamientos {
  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
  }
  
  &__title {
    font-size: 2rem;
    color: $primary-color;
    margin-bottom: calc($spacing-unit / 4);
  }
  
  &__subtitle {
    font-size: 1.2rem;
    color: $secondary-color;
  }
  
  &__add {
    @include button($bg: $primary-color);
  }
  
  &__content {
    @include card;
    padding: $spacing-unit;
  }
  
  &__table {
    width: 100%;
    border-collapse: collapse;
    
    th, td {
      padding: calc($spacing-unit / 2);
      border-bottom: 1px solid $border-color;
    }
    
    th {
      background-color: rgba($primary-color, 0.05);
      color: $secondary-color;
      font-weight: 600;
    }
    
    .descripcion {
      max-width: 300px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
}

.loading, .error, .empty {
  text-align: center;
  padding: $spacing-unit;
}

.error p { color: $danger-color; }

.btn--primary { @include button($bg: $primary-color); }
</style>
