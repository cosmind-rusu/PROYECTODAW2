<script setup lang="ts">
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

// Crear categoría con prompt usando authHeader del store
const createCategory = async () => {
  const name = prompt('Name?')
  if (!name) return
  try {
    const config = useAuthStore().authHeader
    const { data } = await axios.post('/api/categories', { name, description: '', isActive: true }, config)
    alert(`Category created: ${data.name}`)
  } catch (err) {
    console.error(err)
    alert('Error creating category')
  }
}
</script>

<template>
  <main class="dashboard">
    <h2>Dashboard</h2>
    <button class="dashboard__btn" @click="createCategory">Create Category</button>
  </main>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.dashboard {
  padding: $spacing-unit;
  &__btn {
    margin-top: $spacing-unit;
    padding: ($spacing-unit / 2) $spacing-unit;
    background-color: $primary-color;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    &:hover {
      background-color: $secondary-color;
    }
  }
}
</style>
