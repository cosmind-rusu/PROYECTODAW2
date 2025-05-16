<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();
const email = ref('');
const password = ref('');
const error = ref('');

const login = async () => {
  error.value = '';
  try {
    await authStore.login(email.value, password.value);
    router.push('/');
  } catch (err: any) {
    error.value = err.response?.data || 'Login failed';
  }
};
</script>

<template>
  <div class="auth-container">
    <h2 class="auth-container__title">Login</h2>
    <form class="auth-container__form" @submit.prevent="login">
      <input class="auth-container__form-input" v-model="email" type="email" placeholder="Email" required />
      <input class="auth-container__form-input" v-model="password" type="password" placeholder="Password" required />
      <button type="submit" class="auth-container__form-button">Login</button>
    </form>
    <p v-if="error" class="auth-container__error">{{ error }}</p>
  </div>
</template>

<style scoped lang="scss">
.auth-container {
  max-width: 400px;
  margin: auto;
  padding: 2rem;
  form {
    display: flex;
    flex-direction: column;
    input {
      margin-bottom: 1rem;
      padding: 0.5rem;
    }
    button {
      padding: 0.75rem;
      background: #42b983;
      color: white;
      border: none;
      cursor: pointer;
    }
  }
  .error {
    color: red;
    margin-top: 1rem;
  }
}
</style>
