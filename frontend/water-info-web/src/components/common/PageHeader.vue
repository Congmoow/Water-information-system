<template>
  <section class="page-header">
    <nav v-if="breadcrumbs.length > 0" class="page-header__breadcrumbs" aria-label="页面路径">
      <ol>
        <li v-for="(item, index) in breadcrumbs" :key="`${item.label}-${index}`">
          <span>{{ item.label }}</span>
        </li>
      </ol>
    </nav>

    <div class="page-header__main">
      <div class="page-header__copy">
        <h1>{{ title }}</h1>
        <p v-if="description">{{ description }}</p>
      </div>

      <div v-if="$slots.actions" class="page-header__actions">
        <slot name="actions" />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
export interface PageHeaderBreadcrumbItem {
  label: string
}

withDefaults(
  defineProps<{
    title: string
    description?: string
    breadcrumbs?: PageHeaderBreadcrumbItem[]
  }>(),
  {
    description: '',
    breadcrumbs: () => []
  }
)
</script>

<style scoped lang="scss">
.page-header {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.page-header__breadcrumbs ol {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0;
  padding: 0;
  list-style: none;
}

.page-header__breadcrumbs li {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: var(--wi-text-tertiary);
  font-size: 12px;

  &:not(:last-child)::after {
    content: '/';
    color: var(--wi-text-disabled);
  }

  &:last-child {
    color: var(--wi-text-primary);
    font-weight: 600;
  }
}

.page-header__main {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 20px;
}

.page-header__copy {
  min-width: 0;

  h1 {
    margin: 0;
    font-size: 28px;
    line-height: 1.2;
    color: var(--wi-text-primary);
    font-weight: 600;
  }

  p {
    margin: 8px 0 0;
    color: var(--wi-text-secondary);
    font-size: 14px;
    line-height: 1.7;
  }
}

.page-header__actions {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 12px;
}

@media (max-width: 960px) {
  .page-header__main {
    flex-direction: column;
  }

  .page-header__actions {
    width: 100%;
  }
}
</style>
