<template>
  <article class="metric-card panel hover-lift metric-card-animated" :class="`metric-card--${tone}`">
    <div class="metric-card__label-row">
      <span class="metric-card__label">{{ label }}</span>
      <div class="metric-card__header-right">
        <slot name="badge" />
        <span v-if="highlight" class="metric-card__highlight">{{ highlight }}</span>
      </div>
    </div>
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
  padding: var(--wi-space-5, 24px) var(--wi-space-5, 24px);
}

.metric-card__label-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--wi-space-3, 12px);
}

.metric-card__header-right {
  display: flex;
  align-items: center;
  gap: var(--wi-space-2, 8px);
}

.metric-card__label {
  color: var(--wi-text-tertiary);
  font-size: 13px;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

.metric-card__value {
  display: block;
  margin-top: var(--wi-space-3, 12px);
  font-size: clamp(26px, 2.4vw, 32px);
  line-height: 1.05;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  letter-spacing: -0.03em;
  color: var(--wi-text-primary);
}

.metric-card__highlight {
  display: inline-flex;
  align-items: center;
  min-height: 26px;
  padding: 0 10px;
  border-radius: 999px;
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
  color: var(--wi-text-secondary);
  font-size: 12px;
  font-weight: 600;
}

.metric-card__description {
  margin: var(--wi-space-3, 12px) 0 0;
  color: var(--wi-text-secondary);
  font-size: 14px;
  line-height: 1.65;
}

.metric-card__footer {
  margin-top: var(--wi-space-3, 12px);
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
