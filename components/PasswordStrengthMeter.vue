<template>
  <div class="password-strength" :class="strength.className">
    <div class="strength-header">
      <span>Forca da senha</span>
      <strong>{{ strength.label }}</strong>
    </div>

    <div class="strength-track" aria-hidden="true">
      <div class="strength-fill" :style="{ width: `${strength.percent}%` }" />
    </div>

    <ul v-if="strength.hints.length" class="strength-hints">
      <li v-for="hint in strength.hints" :key="hint">{{ hint }}</li>
    </ul>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  password: string
}>()

const strength = computed(() => getPasswordStrength(props.password))
</script>
