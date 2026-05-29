<template>
  <div class="grid min-w-0 gap-2 text-sm font-extrabold text-[#071d3b]">
    <label v-if="label" :for="inputId">{{ label }}</label>

    <div class="flex min-w-0 overflow-hidden rounded-md border border-[#ccd8e5] bg-white focus-within:border-[#147f72] focus-within:ring-4 focus-within:ring-[#147f72]/10">
      <input
        :id="inputId"
        v-model="inputValue"
        class="min-h-11 min-w-0 flex-1 border-0 px-3 text-[#071d3b] outline-none focus:ring-0 disabled:bg-[#f3f7fb]"
        type="text"
        inputmode="numeric"
        autocomplete="off"
        :disabled="disabled"
        :required="required"
        :placeholder="DATE_BR_PLACEHOLDER"
        maxlength="10"
        @input="atualizarDigitacao"
        @focus="abrir"
        @keydown.esc.prevent="fechar"
        @keydown.enter.prevent="confirmarDigitacao"
      />
      <button
        class="inline-flex min-h-11 w-11 shrink-0 items-center justify-center border-0 border-l border-[#d4dee9] bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1] disabled:cursor-not-allowed disabled:opacity-60"
        type="button"
        title="Selecionar data"
        aria-label="Selecionar data"
        :disabled="disabled"
        @click="alternar"
      >
        <CalendarDays class="h-5 w-5" aria-hidden="true" />
      </button>
    </div>

    <div
      v-if="aberto && !disabled"
      class="grid gap-3 rounded-lg border border-[#d4dee9] bg-white p-3 shadow-[0_14px_34px_rgba(14,30,53,0.12)]"
    >
      <div class="flex items-center justify-between gap-2">
        <button
          class="inline-flex h-9 w-9 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
          type="button"
          title="Mes anterior"
          aria-label="Mes anterior"
          @click="mudarMes(-1)"
        >
          <ChevronLeft class="h-5 w-5" aria-hidden="true" />
        </button>
        <strong class="text-center text-sm text-[#071d3b]">{{ mesAtualLabel }}</strong>
        <button
          class="inline-flex h-9 w-9 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
          type="button"
          title="Proximo mes"
          aria-label="Proximo mes"
          @click="mudarMes(1)"
        >
          <ChevronRight class="h-5 w-5" aria-hidden="true" />
        </button>
      </div>

      <div class="grid grid-cols-7 gap-1 text-center">
        <span
          v-for="dia in diasSemana"
          :key="dia"
          class="py-1 text-xs font-extrabold uppercase text-[#62728a]"
        >
          {{ dia }}
        </span>
        <button
          v-for="dia in diasCalendario"
          :key="dia.iso"
          class="h-9 rounded-md text-sm font-extrabold transition"
          :class="diaClasses(dia)"
          type="button"
          @click="selecionarDia(dia.date)"
        >
          {{ dia.date.getDate() }}
        </button>
      </div>

      <div class="grid grid-cols-2 gap-2 sm:grid-cols-3">
        <button class="btn btn-secondary btn-small" type="button" @click="selecionarHoje">
          Hoje
        </button>
        <button class="btn btn-secondary btn-small" type="button" @click="limpar">
          Limpar
        </button>
        <button class="btn btn-primary btn-small col-span-2 sm:col-span-1" type="button" @click="fechar">
          OK
        </button>
      </div>
    </div>

    <span v-if="hint" class="text-xs font-extrabold text-[#62728a]">{{ hint }}</span>
  </div>
</template>

<script setup lang="ts">
import { CalendarDays, ChevronLeft, ChevronRight } from '@lucide/vue'
import {
  addMonths,
  DATE_BR_PLACEHOLDER,
  formatDateToIso,
  formatIsoDateToBr,
  maskBrDateInput,
  parseBrDateToIso,
  parseIsoDate
} from '~/utils/date-utils'

const props = withDefaults(defineProps<{
  modelValue?: string | null
  label?: string
  hint?: string
  disabled?: boolean
  required?: boolean
  id?: string
}>(), {
  modelValue: '',
  label: '',
  hint: '',
  disabled: false,
  required: false,
  id: ''
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

interface DiaCalendario {
  date: Date
  iso: string
  currentMonth: boolean
}

const inputValue = ref(formatIsoDateToBr(props.modelValue))
const aberto = ref(false)
const hoje = new Date()
const mesAtual = ref(parseIsoDate(props.modelValue) ?? new Date(hoje.getFullYear(), hoje.getMonth(), 1))
const generatedId = useId()
const inputId = computed(() => props.id || `date-picker-${generatedId}`)
const diasSemana = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab']
const mesAtualLabel = computed(() =>
  new Intl.DateTimeFormat('pt-BR', {
    month: 'long',
    year: 'numeric'
  }).format(mesAtual.value)
)
const diasCalendario = computed<DiaCalendario[]>(() => {
  const firstDay = new Date(mesAtual.value.getFullYear(), mesAtual.value.getMonth(), 1)
  const start = new Date(firstDay)
  start.setDate(firstDay.getDate() - firstDay.getDay())

  return Array.from({ length: 42 }, (_, index) => {
    const date = new Date(start)
    date.setDate(start.getDate() + index)

    return {
      date,
      iso: formatDateToIso(date),
      currentMonth: date.getMonth() === mesAtual.value.getMonth()
    }
  })
})

watch(() => props.modelValue, (value) => {
  inputValue.value = formatIsoDateToBr(value)
  const parsed = parseIsoDate(value)
  if (parsed) {
    mesAtual.value = new Date(parsed.getFullYear(), parsed.getMonth(), 1)
  }
})

function atualizarDigitacao() {
  inputValue.value = maskBrDateInput(inputValue.value)

  if (!inputValue.value) {
    emit('update:modelValue', '')
    return
  }

  const iso = parseBrDateToIso(inputValue.value)
  if (iso) {
    emit('update:modelValue', iso)
    const parsed = parseIsoDate(iso)
    if (parsed) mesAtual.value = new Date(parsed.getFullYear(), parsed.getMonth(), 1)
  }
}

function confirmarDigitacao() {
  const iso = parseBrDateToIso(inputValue.value)
  if (iso) {
    emit('update:modelValue', iso)
    fechar()
  }
}

function selecionarDia(date: Date) {
  const iso = formatDateToIso(date)
  emit('update:modelValue', iso)
  inputValue.value = formatIsoDateToBr(iso)
  mesAtual.value = new Date(date.getFullYear(), date.getMonth(), 1)
  fechar()
}

function selecionarHoje() {
  selecionarDia(new Date())
}

function limpar() {
  inputValue.value = ''
  emit('update:modelValue', '')
}

function mudarMes(amount: number) {
  mesAtual.value = addMonths(mesAtual.value, amount)
}

function abrir() {
  aberto.value = true
}

function fechar() {
  aberto.value = false
}

function alternar() {
  aberto.value = !aberto.value
}

function diaClasses(dia: DiaCalendario) {
  const selected = props.modelValue === dia.iso
  const isToday = formatDateToIso(hoje) === dia.iso

  if (selected) return 'bg-[#147f72] text-white hover:bg-[#0f6c61]'
  if (isToday) return 'bg-[#eaf4f1] text-[#006b61] ring-2 ring-[#147f72]/25'
  if (!dia.currentMonth) return 'text-[#a0aabc] hover:bg-[#f5f8fb]'

  return 'text-[#071d3b] hover:bg-[#edf3f8]'
}
</script>
