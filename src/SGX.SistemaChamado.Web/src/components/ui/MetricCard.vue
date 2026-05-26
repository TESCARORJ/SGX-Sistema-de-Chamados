<script setup lang="ts">
import { computed } from 'vue'

type Tone = 'primary' | 'info' | 'warning' | 'negative' | 'positive' | 'purple'

const props = defineProps<{
  title?: string
  value?: string | number
  caption?: string
  trend?: string
  tone?: Tone
  loading?: boolean
  icon?: string
  titulo?: string
  valor?: string | number
  subtitulo?: string
  color?: string
}>()

const tituloExibido = computed(() => props.title || props.titulo || '')
const valorExibido = computed(() => props.value ?? props.valor ?? 0)
const legendaExibida = computed(() => props.caption || props.subtitulo || '')

const toneMap: Record<Tone, { avatar: string; icon: string }> = {
  primary: { avatar: 'bg-blue-1 text-primary', icon: 'text-primary' },
  info: { avatar: 'bg-cyan-1 text-info', icon: 'text-info' },
  warning: { avatar: 'bg-orange-1 text-warning', icon: 'text-warning' },
  negative: { avatar: 'bg-red-1 text-negative', icon: 'text-negative' },
  positive: { avatar: 'bg-green-1 text-positive', icon: 'text-positive' },
  purple: { avatar: 'bg-purple-1 text-purple-8', icon: 'text-purple-8' },
}

const toneComputado = computed<Tone>(() => {
  if (props.tone) return props.tone
  const valor = (props.color || '').toLowerCase()
  if (valor.includes('negative') || valor.includes('red')) return 'negative'
  if (valor.includes('warning') || valor.includes('orange') || valor.includes('yellow')) return 'warning'
  if (valor.includes('positive') || valor.includes('green')) return 'positive'
  if (valor.includes('purple')) return 'purple'
  if (valor.includes('info') || valor.includes('cyan') || valor.includes('teal')) return 'info'
  return 'primary'
})

const classesTom = computed(() => toneMap[toneComputado.value])
</script>

<template>
  <q-card flat bordered class="sgx-card metric-card">
    <q-card-section class="metric-card__content">
      <div class="metric-card__info">
        <div class="text-caption text-weight-medium sgx-muted">{{ tituloExibido }}</div>

        <q-skeleton v-if="loading" type="text" width="62%" class="q-mt-sm" />
        <div v-else class="metric-card__value">{{ valorExibido }}</div>

        <q-skeleton v-if="loading" type="text" width="74%" class="q-mt-sm" />
        <div v-else-if="legendaExibida" class="text-caption sgx-muted q-mt-sm">{{ legendaExibida }}</div>

        <div v-if="trend && !loading" class="metric-card__trend q-mt-sm">
          <q-icon name="trending_up" size="16px" />
          <span>{{ trend }}</span>
        </div>
      </div>

      <q-avatar v-if="icon" size="42px" :class="classesTom.avatar">
        <q-icon :name="icon" :class="classesTom.icon" />
      </q-avatar>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.metric-card {
  height: 100%;
  border-radius: var(--sgx-radius-md);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.metric-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--sgx-shadow-md);
}

.metric-card__content {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
}

.metric-card__info {
  min-width: 0;
  flex: 1;
}

.metric-card__value {
  margin-top: 6px;
  font-size: 2rem;
  line-height: 1.08;
  font-weight: 800;
  color: var(--sgx-text);
  overflow-wrap: anywhere;
}

.metric-card__trend {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  color: var(--sgx-info);
  font-size: 0.8rem;
  font-weight: 700;
}

@media (max-width: 768px) {
  .metric-card__value {
    font-size: 1.74rem;
  }
}
</style>
