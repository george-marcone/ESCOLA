<template>
  <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
    <span>{{ label }}</span>
    <span class="relative block">
      <input
        :value="modelValue"
        class="min-h-11 rounded-md border border-[#ccd8e5] py-2 pl-3 pr-12 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10 disabled:bg-slate-100 disabled:text-slate-500"
        :type="showPassword ? 'text' : 'password'"
        :autocomplete="autocomplete"
        :required="required"
        :disabled="disabled"
        @input="onInput"
      />
      <button
        class="absolute right-1 top-1/2 inline-flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1] disabled:opacity-50"
        type="button"
        :disabled="disabled"
        :title="showPassword ? 'Ocultar senha' : 'Exibir senha'"
        :aria-label="showPassword ? 'Ocultar senha' : 'Exibir senha'"
        @click="showPassword = !showPassword"
      >
        <EyeOff v-if="showPassword" class="h-5 w-5" aria-hidden="true" />
        <Eye v-else class="h-5 w-5" aria-hidden="true" />
      </button>
    </span>
  </label>
</template>

<script setup lang="ts">
import { Eye, EyeOff } from '@lucide/vue'

defineProps<{
  modelValue: string
  label: string
  autocomplete: string
  required?: boolean
  disabled?: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const showPassword = ref(false)

function onInput(event: Event) {
  emit('update:modelValue', (event.target as HTMLInputElement).value)
}
</script>
