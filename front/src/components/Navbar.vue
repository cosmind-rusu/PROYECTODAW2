<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { RouterLink } from 'vue-router'
import LanguageSwitcher from '@/components/LanguageSwitcher.vue'

const authStore = useAuthStore()
const logout = () => authStore.logout()
</script>

<template>
  <header class="header">
    <div class="header__logo">
      <span>VetClinic</span>
    </div>
    <nav class="header__nav">
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/">Dashboard</RouterLink>
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/categories">Animal Species</RouterLink>
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/consultations">Consultations</RouterLink>
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/treatments">Treatments</RouterLink>
      <RouterLink v-if="authStore.isAuthenticated" class="header__nav__link" to="/healthplans">Health Plans</RouterLink>
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
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);

  &__logo {
    display: flex;
    align-items: center;
    font-size: 1.5rem;
    color: white;
    font-weight: bold;
    
    &-img {
      height: 30px;
      margin-right: $spacing-unit / 2;
    }
  }

  &__nav {
    display: flex;
    gap: $spacing-unit;
    align-items: center;
    flex-wrap: wrap;
    
    @media (max-width: 768px) {
      flex-direction: column;
      align-items: flex-end;
    }

    &__link {
      color: white;
      text-decoration: none;
      font-weight: bold;
      transition: color 0.2s ease;
      
      &:hover {
        color: $secondary-color;
      }
      
      &.router-link-active {
        color: $secondary-color;
        border-bottom: 2px solid $secondary-color;
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
