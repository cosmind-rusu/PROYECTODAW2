<template>
  <div class="consultas">
    <h1 class="consultas__title">Consultas Veterinarias</h1>
    <p class="consultas__subtitle">Gestiona las consultas y visitas de pacientes</p>
    
    <!-- Componente básico para mostrar/editar consultas -->
    <div class="consultas__content">
      <p>Cargando consultas...</p>
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

.consultas {
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
