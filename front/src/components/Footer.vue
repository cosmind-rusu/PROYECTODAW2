<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const year = new Date().getFullYear()
</script>

<template>
  <footer class="footer">
    <p>{{ t('appName') }} © {{ year }}</p>
  </footer>
</template>

<style scoped lang="scss">
@import "@/assets/styles/variables";
@import "@/assets/styles/mixins";

.footer {
  @include center-flex;
  justify-content: center;
  padding: $spacing-unit;
  background-color: $secondary-color;
  color: white;
  p {
    margin: 0;
    font-size: 0.9rem;
  }
}
</style>
