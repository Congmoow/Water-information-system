<template>
  <section class="chart-section panel">
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
  padding: 22px 24px;
}

.chart-section__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 16px;
}

.chart-section__copy h3 {
  margin: 0;
  font-size: 18px;
  color: var(--wi-text-primary);
}

.chart-section__copy p {
  margin: 8px 0 0;
  color: var(--wi-text-secondary);
  font-size: 13px;
  line-height: 1.7;
}

.chart-section__actions {
  flex-shrink: 0;
}

.chart-section__body {
  min-width: 0;
}

@media (max-width: 960px) {
  .chart-section {
    padding: 18px;
  }

  .chart-section__header {
    flex-direction: column;
  }
}
</style>
