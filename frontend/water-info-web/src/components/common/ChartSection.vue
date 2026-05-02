<template>
  <section class="chart-section panel scale-in">
    <header class="chart-section__header">
      <div class="chart-section__copy">
        <h3>{{ title }}</h3>
        <p v-if="description">{{ description }}</p>
      </div>
      <div v-if="$slots.actions" class="chart-section__actions">
        <slot name="actions" />
      </div>
    </header>

    <div class="chart-section__body">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string
    description?: string
  }>(),
  {
    description: ''
  }
)
</script>

<style scoped lang="scss">
.chart-section {
  padding: var(--wi-space-5, 24px) var(--wi-space-5, 24px);
}

.chart-section__header,
.chart-section__body {
  position: relative;
  z-index: 1;
}

.chart-section__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--wi-space-4, 16px);
  margin-bottom: var(--wi-space-4, 16px);
  padding-bottom: var(--wi-space-3, 12px);
  border-bottom: 1px solid var(--wi-app-border-subtle);
}

.chart-section__copy h3 {
  margin: 0;
  font-size: var(--wi-font-size-section-title, 18px);
  font-weight: var(--wi-font-weight-section-title, 600);
  letter-spacing: -0.01em;
  color: var(--wi-text-primary);
}

.chart-section__copy p {
  margin: var(--wi-space-2, 8px) 0 0;
  color: var(--wi-text-secondary);
  font-size: 13px;
  line-height: 1.65;
}

.chart-section__actions {
  flex-shrink: 0;
}

.chart-section__body {
  min-width: 0;
}

@media (max-width: 960px) {
  .chart-section {
    padding: var(--wi-space-4, 16px);
  }

  .chart-section__header {
    flex-direction: column;
  }
}
</style>
