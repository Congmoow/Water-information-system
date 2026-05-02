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
  gap: var(--wi-space-3, 12px);
}

.page-header__breadcrumbs ol {
  display: flex;
  align-items: center;
  gap: var(--wi-space-2, 8px);
  margin: 0;
  padding: 0;
  list-style: none;
}

.page-header__breadcrumbs li {
  display: inline-flex;
  align-items: center;
  gap: var(--wi-space-2, 8px);
  color: var(--wi-text-tertiary);
  font-size: 12px;
  font-weight: 500;

  &:not(:last-child)::after {
    content: '›';
    margin-left: 6px;
    color: var(--wi-text-disabled);
    font-size: 13px;
    font-weight: 500;
    opacity: 0.72;
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
  gap: var(--wi-space-5, 24px);
}

.page-header__copy {
  min-width: 0;

  h1 {
    margin: 0;
    font-size: var(--wi-font-size-page-title, 28px);
    line-height: 1.22;
    color: var(--wi-text-primary);
    font-weight: var(--wi-font-weight-page-title, 600);
    letter-spacing: -0.03em;
  }

  h1::after {
    content: '';
    display: block;
    width: min(56px, 18%);
    height: 3px;
    margin-top: var(--wi-space-3, 12px);
    border-radius: 2px;
    background: linear-gradient(90deg, var(--wi-primary) 0%, color-mix(in srgb, var(--wi-accent) 75%, var(--wi-primary)) 72%, transparent 100%);
  }

  p {
    margin: var(--wi-space-2, 8px) 0 0;
    color: var(--wi-text-secondary);
    font-size: 14px;
    line-height: 1.65;
  }
}

.page-header__actions {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: var(--wi-space-3, 12px);
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
