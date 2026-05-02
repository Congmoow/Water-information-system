<template>
  <section class="page-card panel hover-glow">
    <header v-if="title || subtitle" class="page-card__header">
      <div>
        <h3>{{ title }}</h3>
        <p v-if="subtitle">{{ subtitle }}</p>
      </div>
      <div v-if="$slots.extra" class="page-card__extra">
        <slot name="extra" />
      </div>
    </header>
    <div class="page-card__body">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
defineProps<{
  title?: string
  subtitle?: string
}>()
</script>

<style scoped lang="scss">
.page-card {
  padding: var(--wi-space-5, 24px) var(--wi-space-5, 24px);
  background: var(--wi-app-surface, var(--wi-surface-panel));
}

.page-card__header,
.page-card__body {
  position: relative;
  z-index: 1;
}

.page-card__header {
  display: flex;
  justify-content: space-between;
  gap: var(--wi-space-4, 16px);
  align-items: flex-start;
  margin-bottom: var(--wi-space-4, 16px);
  padding-bottom: var(--wi-space-4, 16px);
  border-bottom: 1px solid var(--wi-app-border-subtle, var(--wi-border-subtle));

  h3 {
    margin: 0;
    font-size: var(--wi-font-size-card-title, 16px);
    font-weight: var(--wi-font-weight-card-title, 600);
    color: var(--wi-text-primary);
    letter-spacing: -0.02em;
  }

  p {
    margin: var(--wi-space-2, 8px) 0 0;
    color: var(--wi-text-secondary);
    font-size: 13px;
    line-height: 1.65;
  }
}
</style>
