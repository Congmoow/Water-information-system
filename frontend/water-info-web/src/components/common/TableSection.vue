<template>
  <section class="table-section panel">
    <header class="table-section__header">
      <div class="table-section__copy">
        <div class="table-section__title-row">
          <h3>{{ title }}</h3>
          <span v-if="typeof total === 'number'" class="table-section__total">共 {{ total }} 条</span>
        </div>
        <p v-if="description">{{ description }}</p>
      </div>
      <div v-if="$slots.actions" class="table-section__actions">
        <slot name="actions" />
      </div>
    </header>

    <div class="table-section__body">
      <slot v-if="loading || hasData" />
      <div v-else class="table-section__empty">
        <p>{{ emptyDescription }}</p>
      </div>
    </div>

    <footer v-if="$slots.pagination" class="table-section__footer">
      <slot name="pagination" />
    </footer>
  </section>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string
    description?: string
    total?: number
    hasData?: boolean
    loading?: boolean
    emptyDescription?: string
  }>(),
  {
    description: '',
    total: undefined,
    hasData: true,
    loading: false,
    emptyDescription: '暂无数据'
  }
)
</script>

<style scoped lang="scss">
.table-section {
  padding: 22px 24px;
}

.table-section__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
  padding-bottom: 16px;
  border-bottom: 1px solid var(--wi-app-border-subtle);
}

.table-section__copy {
  min-width: 0;
}

.table-section__title-row {
  display: flex;
  align-items: center;
  gap: 10px;
}

.table-section__copy h3 {
  margin: 0;
  font-size: 18px;
  color: var(--wi-text-primary);
}

.table-section__copy p {
  margin: 8px 0 0;
  color: var(--wi-text-secondary);
  font-size: 13px;
  line-height: 1.7;
}

.table-section__total {
  display: inline-flex;
  align-items: center;
  min-height: 28px;
  padding: 0 10px;
  border-radius: 999px;
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
  color: var(--wi-text-secondary);
  font-size: 12px;
  font-weight: 600;
}

.table-section__actions {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 12px;
}

.table-section__body {
  min-width: 0;
}

.table-section__empty {
  min-height: 180px;
  display: grid;
  place-items: center;
  border: 1px dashed var(--wi-app-border);
  border-radius: var(--wi-app-radius-md);
  background: var(--wi-app-surface-secondary);

  p {
    margin: 0;
    color: var(--wi-text-tertiary);
    font-size: 14px;
  }
}

.table-section__footer {
  margin-top: 20px;
}

@media (max-width: 960px) {
  .table-section {
    padding: 18px;
  }

  .table-section__header {
    flex-direction: column;
  }

  .table-section__actions {
    width: 100%;
  }
}
</style>
