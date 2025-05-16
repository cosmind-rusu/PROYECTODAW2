<template>
  <div class="especies">
    <h1 class="especies__title">Especies de Animales</h1>
    <p class="especies__subtitle">Gestiona las diferentes especies registradas en la clínica</p>
    
    <!-- Componente básico para mostrar/editar especies -->
    <div class="especies__content">
      <p>Cargando especies...</p>
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

.especies {
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
