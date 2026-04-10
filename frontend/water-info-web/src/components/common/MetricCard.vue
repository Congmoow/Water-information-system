<template>
  <article class="metric-card panel" :class="`metric-card--${tone}`">
    <div class="metric-card__label-row">
      <span class="metric-card__label">{{ label }}</span>
      <slot name="badge" />
    </div>
    <span v-if="highlight" class="metric-card__highlight">{{ highlight }}</span>
    <strong class="metric-card__value">{{ value }}</strong>
    <p v-if="description" class="metric-card__description">{{ description }}</p>
    <div v-if="$slots.footer" class="metric-card__footer">
      <slot name="footer" />
    </div>
  </article>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    label: string
    value: string | number
    description?: string
    highlight?: string
    tone?: 'neutral' | 'info' | 'success' | 'warning' | 'danger'
  }>(),
  {
    description: '',
    highlight: '',
    tone: 'neutral'
  }
)
</script>

<style scoped lang="scss">
.metric-card {
  padding: 20px 22px;
}

.metric-card__label-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.metric-card__label {
  color: var(--wi-text-tertiary);
  font-size: 13px;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

.metric-card__value {
  display: block;
  margin-top: 10px;
  font-size: 36px;
  line-height: 1;
  color: var(--wi-text-primary);
}

.metric-card__highlight {
  display: inline-flex;
  align-items: center;
  min-height: 26px;
  margin-top: 16px;
  padding: 0 10px;
  border-radius: 999px;
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
  color: var(--wi-text-secondary);
  font-size: 12px;
  font-weight: 600;
}

.metric-card__description {
  margin: 14px 0 0;
  color: var(--wi-text-secondary);
  line-height: 1.7;
}

.metric-card__footer {
  margin-top: 14px;
}

.metric-card--success .metric-card__highlight {
  background: var(--wi-success-soft);
  border-color: color-mix(in srgb, var(--wi-success) 18%, transparent);
  color: var(--wi-success);
}

.metric-card--warning .metric-card__highlight {
  background: var(--wi-warning-soft);
  border-color: color-mix(in srgb, var(--wi-warning) 18%, transparent);
  color: var(--wi-warning);
}

.metric-card--danger .metric-card__highlight {
  background: var(--wi-danger-soft);
  border-color: color-mix(in srgb, var(--wi-danger) 18%, transparent);
  color: var(--wi-danger);
}

.metric-card--info .metric-card__highlight {
  background: var(--wi-info-soft);
  border-color: color-mix(in srgb, var(--wi-info) 18%, transparent);
  color: var(--wi-info);
}
</style>
