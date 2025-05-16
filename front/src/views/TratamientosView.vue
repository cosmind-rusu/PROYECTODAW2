<template>
  <div class="tratamientos">
    <h1 class="tratamientos__title">Tratamientos Veterinarios</h1>
    <p class="tratamientos__subtitle">Gestiona los tratamientos disponibles en la clínica</p>
    
    <!-- Componente básico para mostrar/editar tratamientos -->
    <div class="tratamientos__content">
      <p>Cargando tratamientos...</p>
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

.tratamientos {
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
