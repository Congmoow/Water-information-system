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
  padding: var(--wi-space-5, 24px) var(--wi-space-5, 24px);
}

.table-section__header,
.table-section__body,
.table-section__footer {
  position: relative;
  z-index: 1;
}

.table-section__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--wi-space-4, 16px);
  margin-bottom: var(--wi-space-4, 16px);
  padding-bottom: var(--wi-space-4, 16px);
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
  font-size: var(--wi-font-size-section-title, 18px);
  font-weight: var(--wi-font-weight-section-title, 600);
  letter-spacing: -0.02em;
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
  gap: var(--wi-space-3, 12px);
}

.table-section__body {
  min-width: 0;
}

.table-section__empty {
  min-height: 200px;
  display: grid;
  place-items: center;
  border: 1px dashed color-mix(in srgb, var(--wi-app-border) 85%, var(--wi-primary));
  border-radius: var(--wi-app-radius-md);
  background: linear-gradient(165deg, var(--wi-app-surface-secondary) 0%, color-mix(in srgb, var(--wi-primary-soft) 35%, var(--wi-app-surface-secondary)) 100%);

  p {
    margin: 0;
    color: var(--wi-text-tertiary);
    font-size: 14px;
    font-weight: 500;
  }
}

.table-section__footer {
  margin-top: var(--wi-space-5, 24px);
}

@media (max-width: 960px) {
  .table-section {
    padding: var(--wi-space-4, 16px);
  }

  .table-section__header {
    flex-direction: column;
  }

  .table-section__actions {
    width: 100%;
  }
}
</style>
