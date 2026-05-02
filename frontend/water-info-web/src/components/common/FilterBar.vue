<template>
  <section class="filter-bar slide-in-left">
    <div class="filter-bar__fields">
      <slot />
    </div>
    <div v-if="$slots.actions" class="filter-bar__actions">
      <slot name="actions" />
    </div>
  </section>
</template>

<style scoped lang="scss">
.filter-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
}

.filter-bar__fields {
  display: flex;
  align-items: center;
  gap: 16px;
  flex: 1;
  flex-wrap: wrap;
}

.filter-bar__fields :deep(> *) {
  flex: 1 1 220px;
  min-width: min(100%, 220px);
}

.filter-bar__actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  flex-wrap: wrap;
}

@media (max-width: 960px) {
  .filter-bar,
  .filter-bar__actions {
    width: 100%;
  }

  .filter-bar__actions {
    justify-content: flex-start;
  }
}
</style>
