<template>
  <div class="consulta-form">
    <div class="consulta-form__header">
      <h2>Nuevo Consulta</h2>
      <button class="consulta-form__close" @click="cerrar">×</button>
    </div>
    <form @submit.prevent="guardar" class="consulta-form__body">
      <div class="consulta-form__grid">
        <!-- Grilla de campos -->
        <div class="form-group">
          <label for="especie">Especie</label>
          <select id="especie" v-model="dto.especieAnimalId" required>
            <option value="">Seleccione una especie</option>
            <option v-for="e in especies" :key="e.id" :value="e.id">{{ e.nombre }}</option>
          </select>
          <p v-if="submitted && !dto.especieAnimalId" class="error">Seleccione una especie</p>
        </div>
        <div class="form-group">
          <label for="tratamiento">Tratamiento</label>
          <select id="tratamiento" v-model="dto.tratamientoId" required>
            <option value="">Seleccione un tratamiento</option>
            <option v-for="t in tratamientos" :key="t.id" :value="t.id">{{ t.nombre }}</option>
          </select>
          <p v-if="submitted && !dto.tratamientoId" class="error">Seleccione un tratamiento</p>
        </div>
        <div class="form-group">
          <label for="nombreMascota">Nombre Mascota</label>
          <input id="nombreMascota" v-model="dto.nombreMascota" type="text" required placeholder="Ingrese el nombre de la mascota" />
          <p v-if="submitted && !dto.nombreMascota.trim()" class="error">El nombre de la mascota es obligatorio</p>
        </div>
        <div class="form-group">
          <label for="nombrePropietario">Propietario</label>
          <input id="nombrePropietario" v-model="dto.nombrePropietario" type="text" required placeholder="Ingrese el nombre del propietario" />
          <p v-if="submitted && !dto.nombrePropietario.trim()" class="error">El nombre del propietario es obligatorio</p>
        </div>
        <div class="form-group">
          <label for="informacionContacto">Contacto</label>
          <input id="informacionContacto" v-model="dto.informacionContacto" type="text" required placeholder="Ingrese la información de contacto" />
          <p v-if="submitted && !dto.informacionContacto.trim()" class="error">La información de contacto es obligatoria</p>
        </div>
        <div class="form-group">
          <label for="costo">Costo</label>
          <input id="costo" v-model.number="dto.costo" type="number" min="0" required placeholder="0" />
        </div>
        <div class="form-group form-group--wide">
          <label for="descripcion">Descripción</label>
          <textarea id="descripcion" v-model="dto.descripcion" required placeholder="Ingrese la descripción"></textarea>
          <p v-if="submitted && !dto.descripcion.trim()" class="error">La descripción es obligatoria</p>
        </div>
        <div class="form-group">
          <label for="fechaConsulta">Fecha Consulta</label>
          <input id="fechaConsulta" v-model="dto.fechaConsulta" type="date" required />
          <p v-if="submitted && !dto.fechaConsulta" class="error">La fecha de consulta es obligatoria</p>
        </div>
        <div class="form-group">
          <label for="fechaSeguimiento">Fecha Seguimiento</label>
          <input id="fechaSeguimiento" v-model="dto.fechaSeguimiento" type="date" />
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
import { ref, onMounted } from 'vue';
import apiService from '@/services/api-service';
import { useConsultasStore } from '@/stores/consultas';
import type { ConsultaVeterinariaDto } from '@/types/consultas';
import type { EspecieAnimal } from '@/types/especies';
import type { Tratamiento } from '@/types/tratamientos';

const emit = defineEmits(['cerrar','guardado']);
const store = useConsultasStore();

const loading = ref(false);
const submitted = ref(false);
const error = ref('');
const especies = ref<EspecieAnimal[]>([]);
const tratamientos = ref<Tratamiento[]>([]);
const dto = ref<ConsultaVeterinariaDto>({
  especieAnimalId: 0,
  tratamientoId: 0,
  nombreMascota: '',
  nombrePropietario: '',
  informacionContacto: '',
  costo: 0,
  descripcion: '',
  fechaConsulta: new Date().toISOString().substr(0,10),
  fechaSeguimiento: undefined
});

async function cargarListas() {
  try {
    especies.value = await apiService.getEspecies();
    tratamientos.value = await apiService.getTratamientos();
  } catch (e) {
    console.error('Error al cargar listas en formulario:', e);
  }
}

onMounted(cargarListas);

async function guardar() {
  submitted.value = true;
  loading.value = true;
  error.value = '';
  // Validaciones
  const errorsList: string[] = [];
  if (!dto.value.especieAnimalId) errorsList.push('Seleccione una especie');
  if (!dto.value.tratamientoId) errorsList.push('Seleccione un tratamiento');
  if (!dto.value.nombreMascota.trim()) errorsList.push('El nombre de la mascota es obligatorio');
  if (!dto.value.nombrePropietario.trim()) errorsList.push('El nombre del propietario es obligatorio');
  if (!dto.value.informacionContacto.trim()) errorsList.push('La información de contacto es obligatoria');
  if (!dto.value.descripcion.trim()) errorsList.push('La descripción es obligatoria');
  if (!dto.value.fechaConsulta) errorsList.push('La fecha de consulta es obligatoria');
  if (errorsList.length) { error.value = errorsList.join(', '); loading.value = false; return; }
  try {
    await store.crearConsulta(dto.value);
    emit('guardado');
    cerrar();
  } catch (e: any) {
    error.value = e.response?.data?.mensaje || 'Error al crear consulta';
  } finally {
    loading.value = false;
  }
}

function cerrar() {
  emit('cerrar');
}
</script>

<style scoped>
.consulta-form { background: #fff; padding: 1.5rem; border-radius: 8px; max-width: 700px; margin: auto; box-shadow: 0 2px 12px rgba(0,0,0,0.1); }
.consulta-form__header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
.consulta-form__close { background: none; border: none; font-size: 1.5rem; cursor: pointer; }
.consulta-form__grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px,1fr)); gap: 1rem; }
.form-group { display: flex; flex-direction: column; }
.form-group--wide { grid-column: span 2; }
.form-group label { margin-bottom: 0.5rem; font-weight: 500; }
.form-actions { display: flex; justify-content: flex-end; gap: 0.5rem; margin-top: 1rem; }
.error { color: #c00; font-size: 0.875rem; margin-top: 0.25rem; }
.btn { padding: 0.5rem 1rem; border: none; border-radius: 4px; font-weight: 500; cursor: pointer; }
.btn--primary { background: #007bff; color: #fff; }
.btn--secondary { background: #6c757d; color: #fff; }
@media (max-width: 600px) {
  .form-group--wide { grid-column: span 1; }
}
</style>
