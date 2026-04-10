<template>
  <aside class="side-info-panel panel">
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
  padding: 20px 22px;
}

.side-info-panel__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 14px;
}

.side-info-panel__copy h3 {
  margin: 6px 0 0;
  color: var(--wi-text-primary);
  font-size: 20px;
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
  margin-top: 18px;
  padding: 16px;
  border-radius: var(--wi-app-radius-md);
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
}

.side-info-panel__body {
  margin-top: 18px;
}

.side-info-panel__footer {
  margin-top: 18px;
  padding-top: 16px;
  border-top: 1px solid var(--wi-app-border-subtle);
}
</style>
