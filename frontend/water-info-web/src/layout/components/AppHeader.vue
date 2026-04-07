<template>
  <header class="app-header panel">
    <div>
      <p class="app-header__eyebrow">{{ currentDate }}</p>
      <h2>{{ route.meta.title ?? '水利信息管理系统' }}</h2>
    </div>
    <div class="app-header__user">
      <div class="app-header__meta">
        <strong>{{ authStore.user?.fullName ?? '演示用户' }}</strong>
        <span>{{ authStore.user?.role ?? '未登录' }}</span>
      </div>
      <el-dropdown trigger="click" @command="handleCommand">
        <span class="app-header__action">
          <el-avatar size="large" class="app-header__avatar">
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
  </header>
</template>

<script setup lang="ts">
import dayjs from 'dayjs'
import { computed } from 'vue'
import { ElMessage } from 'element-plus'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const currentDate = computed(() => dayjs().format('YYYY年MM月DD日 dddd'))

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
  margin: 18px 24px 0;
  padding: 18px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  background: var(--wi-surface-panel);

  h2 {
    margin: 6px 0 0;
    font-size: 24px;
    color: var(--wi-text-primary);
  }
}

.app-header__eyebrow {
  margin: 0;
  color: var(--wi-text-soft);
  font-size: 13px;
  letter-spacing: 0.06em;
}

.app-header__user {
  display: flex;
  align-items: center;
  gap: 14px;
  text-align: right;

  strong,
  span {
    display: block;
  }

  span {
    margin-top: 6px;
    color: var(--wi-text-soft);
    font-size: 13px;
  }
}

.app-header__meta {
  min-width: 120px;
}

.app-header__action {
  cursor: pointer;
}

.app-header__avatar {
  background: linear-gradient(135deg, var(--wi-primary-hover), var(--wi-primary-active));
  color: var(--wi-primary-contrast);
  box-shadow:
    var(--wi-ring-accent),
    var(--wi-shadow-primary);
}
</style>
