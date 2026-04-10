<template>
  <header class="app-header">
    <div class="app-header__inner">
      <div class="app-header__content">
        <div class="app-header__main">
          <PageHeader
            :title="pageTitle"
            :description="pageDescription"
            :breadcrumbs="breadcrumbs"
          >
            <template #actions>
              <div class="app-header__date-card">
              <span>当前日期</span>
              <strong>{{ currentDate }}</strong>
              </div>
            </template>
          </PageHeader>
        </div>
        <div class="app-header__user">
          <div class="app-header__meta">
            <span class="app-header__env">课程演示环境</span>
            <strong>{{ authStore.user?.fullName ?? '演示用户' }}</strong>
            <p>{{ authStore.user?.role ?? '未登录' }}</p>
          </div>
          <el-dropdown trigger="click" @command="handleCommand">
            <span class="app-header__action">
              <el-avatar class="app-header__avatar">
                {{ (authStore.user?.fullName ?? '演示').slice(0, 1) }}
              </el-avatar>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">用户中心</el-dropdown-item>
                <el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import dayjs from 'dayjs'
import { computed } from 'vue'
import { ElMessage } from 'element-plus'
import { useRoute, useRouter } from 'vue-router'
import PageHeader from '@/components/common/PageHeader.vue'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
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
const currentDate = computed(() => dayjs().format('YYYY年MM月DD日 dddd'))
const pageTitle = computed(() => String(route.meta.title ?? '水利信息管理系统'))
const breadcrumbs = computed(() => [
  { label: '系统工作台' },
  { label: pageTitle.value }
])
const pageDescription = computed(() => {
  const name = typeof route.name === 'string' ? route.name : ''
  return pageDescriptionMap[name] ?? '围绕监测、告警、统计与空间展示构建统一工作台。'
})

async function handleCommand(command: string) {
  if (command === 'profile') {
    await router.push('/user-center')
    return
  }

  if (command === 'logout') {
    await authStore.logout()
    ElMessage.success('已退出登录')
    await router.push('/login')
  }
}
</script>

<style scoped lang="scss">
.app-header {
  padding: 18px var(--wi-app-page-padding-inline) 18px;
}

.app-header__inner {
  width: 100%;
  max-width: var(--wi-app-content-max-width);
  min-height: var(--wi-app-header-min-height);
  margin: 0 auto;
  padding: 0 0 18px;
  border-bottom: 1px solid var(--wi-app-border-subtle);
}

.app-header__content {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 24px;
}

.app-header__main {
  flex: 1;
  min-width: 0;
}

.app-header__date-card {
  min-width: 196px;
  padding: 14px 16px;
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
  border-radius: var(--wi-app-radius-md);
  box-shadow: var(--wi-app-shadow-sm);

  span,
  strong {
    display: block;
  }

  span {
    font-size: 12px;
    color: var(--wi-text-tertiary);
    letter-spacing: 0.06em;
    text-transform: uppercase;
  }

  strong {
    margin-top: 8px;
    color: var(--wi-text-primary);
    font-size: 14px;
    font-weight: 600;
    line-height: 1.6;
  }
}

.app-header__user {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 10px 12px;
  background: var(--wi-app-surface);
  border: 1px solid var(--wi-app-border-subtle);
  border-radius: var(--wi-app-radius-md);
  box-shadow: var(--wi-app-shadow-sm);
}

.app-header__meta {
  min-width: 132px;
  text-align: right;

  strong,
  p {
    margin: 0;
    display: block;
  }

  strong {
    margin-top: 6px;
    color: var(--wi-text-primary);
    font-size: 14px;
  }

  p {
    margin-top: 4px;
    color: var(--wi-text-secondary);
    font-size: 12px;
  }
}

.app-header__env {
  display: inline-flex;
  justify-content: flex-end;
  color: var(--wi-text-tertiary);
  font-size: 12px;
}

.app-header__action {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.app-header__avatar {
  width: 40px;
  height: 40px;
  background: var(--wi-app-surface-secondary);
  color: var(--wi-text-primary);
  border: 1px solid var(--wi-app-border);
}

@media (max-width: 1080px) {
  .app-header {
    padding-inline: 18px;
  }

  .app-header__content {
    flex-direction: column;
  }

  .app-header__date-card,
  .app-header__user {
    width: 100%;
  }

  .app-header__meta {
    text-align: left;
  }

  .app-header__env {
    justify-content: flex-start;
  }
}
</style>
