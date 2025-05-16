<template>
  <div class="login-page">
    <div class="login-container">
      <h1 class="login-title">Iniciar Sesión</h1>
      
      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="email">Correo Electrónico</label>
          <input 
            type="email" 
            id="email" 
            v-model="email" 
            class="form-control" 
            required
            placeholder="ejemplo@correo.com"
            :class="{ 'form-control--error': hasError }"
          />
        </div>
        
        <div class="form-group">
          <label for="password">Contraseña</label>
          <input 
            type="password" 
            id="password" 
            v-model="password" 
            class="form-control" 
            required
            :class="{ 'form-control--error': hasError }"
          />
        </div>
        
        <div v-if="error" class="alert alert--error">
          {{ error }}
        </div>
        
        <div class="form-actions">
          <button 
            type="submit" 
            class="btn btn--primary btn--block" 
            :disabled="loading"
          >
            {{ loading ? 'Cargando...' : 'Iniciar Sesión' }}
          </button>
        </div>
        
        <div class="login-links">
          <p>¿No tienes cuenta? <router-link to="/registro" class="login-link">Regístrate</router-link></p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const error = ref('');
const loading = ref(false);
const hasError = ref(false);

const handleLogin = async () => {
  error.value = '';
  hasError.value = false;
  loading.value = true;
  
  try {
    await authStore.login(email.value, password.value);
    router.push('/');
  } catch (err: any) {
    console.error('Error en inicio de sesión:', err);
    error.value = err.response?.data?.mensaje || 'Error al iniciar sesión. Verifica tus credenciales.';
    hasError.value = true;
  } finally {
    loading.value = false;
  }
};
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.login-page {
  @include center-flex;
  min-height: calc(100vh - 130px); // Ajustar por altura de navbar y footer
  background-color: $background-color;
  padding: $spacing-unit;
}

.login-container {
  @include card($padding: $spacing-unit * 2, $bg: $light-color);
  width: 100%;
  max-width: 400px;
}

.login-title {
  font-size: 1.8rem;
  color: $primary-color;
  margin-bottom: $spacing-unit * 2;
  text-align: center;
}

.login-form {
  @include flex-column;
}

.form-group {
  margin-bottom: $spacing-unit;
  
  label {
    display: block;
    margin-bottom: $spacing-unit / 2;
    font-weight: 500;
    color: $text-color;
  }
}

.form-control {
  @include form-control;
  
  &--error {
    border-color: $danger-color;
  }
}

.form-actions {
  margin-top: $spacing-unit;
}

.btn {
  @include button;
  
  &--primary {
    @include button($bg: $primary-color);
  }
  
  &--block {
    display: block;
    width: 100%;
  }
}

.alert {
  padding: $spacing-unit;
  margin: $spacing-unit 0;
  border-radius: $border-radius;
  
  &--error {
    background-color: rgba($danger-color, 0.1);
    color: $danger-color;
    border: 1px solid rgba($danger-color, 0.2);
  }
}

.login-links {
  margin-top: $spacing-unit;
  text-align: center;
  font-size: $font-size-small;
}

.login-link {
  color: $primary-color;
  font-weight: 500;
  text-decoration: none;
  
  &:hover {
    text-decoration: underline;
  }
}
</style>
