<template>
  <div class="tratamiento-form">
    <div class="tratamiento-form__header">
      <h2 class="tratamiento-form__title">Nuevo Tratamiento</h2>
      <button @click="cerrar" class="tratamiento-form__close">×</button>
    </div>
    <form @submit.prevent="guardar" class="tratamiento-form__body">
      <div class="tratamiento-form__grid">
        <!-- Campos en grid -->
        <div class="form-group">
          <label for="nombre">Nombre</label>
          <input id="nombre" v-model="dto.nombre" type="text" required placeholder="Ej: Vacunación" />
        </div>
        <div class="form-group">
          <label for="descripcion">Descripción</label>
          <textarea id="descripcion" v-model="dto.descripcion" required placeholder="Detalle del tratamiento" class="form-group--wide"></textarea>
        </div>
        <div class="form-group">
          <label for="costoEstandar">Costo Estándar</label>
          <input id="costoEstandar" v-model.number="dto.costoEstandar" type="number" min="0" required placeholder="0.00" />
        </div>
        <div class="form-group">
          <label for="duracionEstimadaMinutos">Duración (min)</label>
          <input id="duracionEstimadaMinutos" v-model.number="dto.duracionEstimadaMinutos" type="number" min="1" required placeholder="30" />
        </div>
        <div class="form-group">
          <label for="activo">Activo</label>
          <input id="activo" v-model="dto.activo" type="checkbox" />
        </div>
      </div>
      <!-- Acciones -->
      <div class="form-actions">
        <button type="button" @click="cerrar" class="btn btn--secondary">Cancelar</button>
        <button type="submit" :disabled="loading" class="btn btn--primary">{{ loading ? 'Guardando...' : 'Guardar' }}</button>
      </div>
      <p v-if="error" class="error">{{ error }}</p>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useTratamientosStore } from '@/stores/tratamientos';
import type { TratamientoDto } from '@/types/tratamientos';

const emit = defineEmits(['cerrar','guardado']);
const store = useTratamientosStore();

const loading = ref(false);
const error = ref('');

const dto = ref<TratamientoDto>({
  nombre: '',
  descripcion: '',
  costoEstandar: 0,
  duracionEstimadaMinutos: 1,
  activo: true
});

async function guardar() {
  loading.value = true;
  error.value = '';
  try {
    await store.crearTratamiento(dto.value);
    emit('guardado');
    cerrar();
  } catch (err: any) {
    error.value = err.response?.data?.mensaje || 'Error al crear tratamiento';
  } finally {
    loading.value = false;
  }
}

function cerrar() {
  emit('cerrar');
}
</script>

<style scoped>
/* Contenedor */
.tratamiento-form { max-width: 600px; margin: auto; background: #fff; padding: 1.5rem; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
.tratamiento-form__header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
.tratamiento-form__close { background: none; border: none; font-size: 1.5rem; cursor: pointer; }
/* Grid de campos */
.tratamiento-form__grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px,1fr)); gap: 1rem; }
.form-group { display: flex; flex-direction: column; }
.form-group--wide { grid-column: span 2; }
.form-group label { margin-bottom: 0.5rem; font-weight: 500; }
/* Botones */
.btn { padding: 0.5rem 1rem; border: none; border-radius: 4px; font-weight: 500; cursor: pointer; }
.btn--primary { background: #007bff; color: #fff; }
.btn--secondary { background: #6c757d; color: #fff; }
/* Form actions */
.form-actions { display: flex; justify-content: flex-end; gap: 0.5rem; margin-top: 1rem; }
/* Errores */
.error { color: #c00; font-size: 0.875rem; margin-top: 0.25rem; }
@media (max-width: 600px) {
  .form-group--wide { grid-column: span 1; }
}
</style>
