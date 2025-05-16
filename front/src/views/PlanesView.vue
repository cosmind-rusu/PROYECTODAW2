<template>
  <div class="planes">
    <h1 class="planes__title">Planes de Salud</h1>
    <p class="planes__subtitle">Gestiona los planes de salud disponibles para mascotas</p>
    
    <!-- Componente básico para mostrar/editar planes -->
    <div class="planes__content">
      <p>Cargando planes de salud...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
// Implementación básica para pasar la construcción
// Se ampliará en el futuro con funcionalidad completa
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.planes {
  &__title {
    font-size: 2rem;
    color: $primary-color;
    margin-bottom: calc($spacing-unit / 2);
  }
  
  &__subtitle {
    font-size: 1.2rem;
    color: $secondary-color;
    margin-bottom: $spacing-unit;
  }
  
  &__content {
    @include card;
    padding: $spacing-unit;
  }
}
</style>
