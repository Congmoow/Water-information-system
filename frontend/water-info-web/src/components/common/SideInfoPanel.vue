<template>
  <aside class="side-info-panel panel slide-in-right">
    <header class="side-info-panel__header">
      <div class="side-info-panel__copy">
        <p v-if="subtitle" class="side-info-panel__subtitle">{{ subtitle }}</p>
        <h3>{{ title }}</h3>
      </div>
      <div v-if="$slots.status" class="side-info-panel__status">
        <slot name="status" />
      </div>
    </header>

    <section v-if="$slots.meta" class="side-info-panel__meta">
      <slot name="meta" />
    </section>

    <section class="side-info-panel__body">
      <slot />
    </section>

    <footer v-if="$slots.footer" class="side-info-panel__footer">
      <slot name="footer" />
    </footer>
  </aside>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string
    subtitle?: string
  }>(),
  {
    subtitle: ''
  }
)
</script>

<style scoped lang="scss">
.side-info-panel {
  padding: var(--wi-space-5, 24px) var(--wi-space-5, 24px);
}

.side-info-panel__header,
.side-info-panel__meta,
.side-info-panel__body,
.side-info-panel__footer {
  position: relative;
  z-index: 1;
}

.side-info-panel__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--wi-space-3, 12px);
}

.side-info-panel__copy h3 {
  margin: var(--wi-space-2, 8px) 0 0;
  color: var(--wi-text-primary);
  font-size: var(--wi-font-size-section-title, 18px);
  font-weight: var(--wi-font-weight-section-title, 600);
  letter-spacing: -0.01em;
}

.side-info-panel__subtitle {
  margin: 0;
  color: var(--wi-text-tertiary);
  font-size: 12px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.side-info-panel__status {
  flex-shrink: 0;
}

.side-info-panel__meta {
  margin-top: var(--wi-space-4, 16px);
  padding: var(--wi-space-4, 16px);
  border-radius: var(--wi-app-radius-md);
  background: linear-gradient(180deg, var(--wi-app-surface-secondary) 0%, var(--wi-app-surface-tertiary) 100%);
  border: 1px solid var(--wi-app-border-subtle);
  box-shadow: var(--wi-app-shadow-xs);
}

.side-info-panel__body {
  margin-top: var(--wi-space-4, 16px);
}

.side-info-panel__footer {
  margin-top: var(--wi-space-4, 16px);
  padding-top: var(--wi-space-4, 16px);
  border-top: 1px solid var(--wi-app-border-subtle);
}
</style>
