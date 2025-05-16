<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { WritableComputedRef } from 'vue'

const i18n = useI18n({ useScope: 'global' })
const localeRef = i18n.locale as WritableComputedRef<'en' | 'es'>

const switchLanguage = () => {
  localeRef.value = localeRef.value === 'en' ? 'es' : 'en'
}
</script>

<template>
  <button class="lang-switcher" @click="switchLanguage">
    Switch to {{ localeRef === 'en' ? 'Spanish' : 'English' }}
  </button>
</template>

<style scoped lang="scss">
@use "@/assets/styles/variables" as *;
@use "@/assets/styles/mixins" as *;

.lang-switcher {
  padding: 0.5rem 1rem;
  background-color: $primary-color;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;

  &:hover {
    background-color: $secondary-color;
  }
}
</style>
