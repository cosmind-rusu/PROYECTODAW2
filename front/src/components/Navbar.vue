<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { RouterLink } from 'vue-router'
import LanguageSwitcher from '@/components/LanguageSwitcher.vue'

const authStore = useAuthStore()
const logout = () => authStore.logout()
</script>

<template>
  <header class="header">
    <div class="header__logo">Finanzas App</div>
    <nav class="header__nav">
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/">Dashboard</RouterLink>
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/about">About</RouterLink>
      <RouterLink v-if="!authStore.isAuthenticated" class="header__nav__link" to="/login">Login</RouterLink>
      <RouterLink v-if="!authStore.isAuthenticated" class="header__nav__link" to="/register">Register</RouterLink>
      <button v-if="authStore.isAuthenticated" class="header__nav__logout" @click="logout">Logout</button>
      <LanguageSwitcher />
    </nav>
  </header>
</template>

<style scoped lang="scss">
@import "@/assets/styles/variables";
@import "@/assets/styles/mixins";

.header {
  @include center-flex;
  justify-content: space-between;
  background-color: $primary-color;
  padding: $spacing-unit;

  &__logo {
    font-size: 1.5rem;
    color: white;
    font-weight: bold;
  }

  &__nav {
    display: flex;
    gap: $spacing-unit;

    &__link {
      color: white;
      text-decoration: none;
      font-weight: bold;
      &:hover {
        color: $secondary-color;
      }
    }

    &__logout {
      background: none;
      border: none;
      color: white;
      cursor: pointer;
      font-size: 1rem;
    }
    & > component[is=LanguageSwitcher] {
      margin-left: $spacing-unit;
    }
  }
}
</style>
