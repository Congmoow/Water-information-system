<template>
  <section class="page-card panel">
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
  padding: 22px 24px;
}

.page-card__header {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  align-items: flex-start;
  margin-bottom: 18px;

  h3 {
    margin: 0;
    font-size: 18px;
  }

  p {
    margin: 8px 0 0;
    color: var(--wi-text-soft);
    font-size: 13px;
  }
}
</style>
