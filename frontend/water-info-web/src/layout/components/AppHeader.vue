<template>
  <header class="app-header">
    <div class="app-header__inner">
      <PageHeader
        :title="pageTitle"
        :description="pageDescription"
        :breadcrumbs="breadcrumbs"
      />
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import PageHeader from '@/components/common/PageHeader.vue'

const route = useRoute()

const pageDescriptionMap: Record<string, string> = {
  dashboard: '查看工程总览、站点状态、近期告警与核心趋势。',
  reservoirs: '统一维护水库基础信息、空间位置与工程资料。',
  rivers: '集中管理河道档案、流域信息与空间属性。',
  stations: '维护监测站点状态、阈值和归属工程关系。',
  monitoring: '按站点与时间维度查看监测数据和历史变化。',
  alarms: '聚焦预警分级、处置进度和告警闭环。',
  map: '从空间视角查看工程、站点和异常状态分布。',
  userCenter: '查看当前账号资料、权限身份说明与会话入口。'
}

const pageTitle = computed(() => String(route.meta.title ?? '水利信息管理系统'))
const breadcrumbs = computed(() => [
  { label: '系统工作台' },
  { label: pageTitle.value }
])
const pageDescription = computed(() => {
  const name = typeof route.name === 'string' ? route.name : ''
  return pageDescriptionMap[name] ?? '围绕监测、告警、统计与空间展示构建统一工作台。'
})
</script>

<style scoped lang="scss">
.app-header {
  flex-shrink: 0;
  padding: var(--wi-space-5, 24px) var(--wi-app-page-padding-inline) var(--wi-space-3, 12px);
  background: color-mix(in srgb, var(--wi-app-bg) 82%, transparent);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
}

.app-header__inner {
  width: 100%;
  max-width: var(--wi-app-content-max-width);
  min-height: var(--wi-app-header-min-height);
  margin: 0 auto;
  padding: 0 0 var(--wi-space-4, 16px);
  border-bottom: 1px solid var(--wi-app-border-subtle);
  box-shadow: 0 1px 0 rgba(255, 255, 255, 0.65);
}

@media (max-width: 1080px) {
  .app-header {
    padding-inline: 18px;
  }
}
</style>
