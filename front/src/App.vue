<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
// Navbar maneja enlaces y logout
</script>

<template>
  <div class="app-container">
    <Navbar />
    <div class="app-content">
      <RouterView />
    </div>
    <Footer />
  </div>
</template>

<style>
html, body, #app {
  height: 100%;
  margin: 0;
}
.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}
.app-content {
  flex: 1;
}
</style>
