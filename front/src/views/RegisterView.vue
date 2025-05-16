<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const router = useRouter();
const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const error = ref('');

const register = async () => {
  error.value = '';
  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match';
    return;
  }
  try {
    await axios.post(
      `${import.meta.env.VITE_API_URL}/api/auth/register`,
      { email: email.value, password: password.value }
    );
    router.push('/login');
  } catch (err: any) {
    error.value = err.response?.data || 'Registration failed';
  }
};
</script>

<template>
  <div class="auth-container">
    <h2 class="auth-container__title">Register</h2>
    <form class="auth-container__form" @submit.prevent="register">
      <input class="auth-container__form-input" v-model="email" type="email" placeholder="Email" required />
      <input class="auth-container__form-input" v-model="password" type="password" placeholder="Password" required minlength="6" />
      <input class="auth-container__form-input" v-model="confirmPassword" type="password" placeholder="Confirm Password" required />
      <button type="submit" class="auth-container__form-button">Register</button>
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
