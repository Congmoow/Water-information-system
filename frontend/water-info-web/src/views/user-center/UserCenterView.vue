<template>
  <div class="page-shell">
    <div class="user-center-summary-grid">
      <MetricCard
        label="账户标识"
        :value="overview.username"
        description="当前登录账户的系统用户名，用于标识当前会话身份。"
        highlight="当前账号"
        tone="info"
      />
      <MetricCard
        label="姓名"
        :value="overview.fullName"
        description="当前资料中记录的账户显示名称。"
        highlight="账户资料"
        tone="info"
      />
      <MetricCard
        label="权限身份"
        :value="overview.roleLabel"
        :description="overview.roleDescription"
        highlight="角色说明"
        tone="info"
      />
      <MetricCard
        label="接入时间"
        :value="createdAtLabel"
        description="当前账户资料首次写入系统的时间。"
        highlight="资料时间"
        tone="info"
      />
    </div>

    <div class="user-center-layout">
      <SideInfoPanel title="账户身份面板" subtitle="账户中心">
        <template #status>
          <StatusTag category="enabledStatus" :value="overview.accountStatus" />
        </template>
        <template #meta>
          <div class="entity-state-note entity-state-note--info">
            <span class="entity-state-note__eyebrow">身份摘要</span>
            <strong>{{ overview.roleLabel }}</strong>
            <p>{{ overview.roleDescription }}</p>
          </div>
          <div class="entity-facts-grid entity-facts-grid--2 user-center-meta">
            <div class="entity-facts-grid__item">
              <span>用户名</span>
              <strong>{{ overview.username }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>姓名</span>
              <strong>{{ overview.fullName }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>角色</span>
              <strong>{{ overview.roleLabel }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>账户状态</span>
              <strong>已启用</strong>
            </div>
          </div>
        </template>
        <dl class="entity-detail-list">
          <div>
            <dt>账户创建时间</dt>
            <dd>{{ createdAtLabel }}</dd>
          </div>
          <div>
            <dt>权限说明</dt>
            <dd>{{ overview.roleDescription }}</dd>
          </div>
          <div>
            <dt>当前页面定位</dt>
            <dd>本页用于查看当前账号资料、权限身份和会话相关入口，不承担业务数据管理职责。</dd>
          </div>
        </dl>
      </SideInfoPanel>

      <SideInfoPanel title="账户操作区" subtitle="资料与会话">
        <template #status>
          <span class="entity-pill">{{ overview.roleLabel }}</span>
        </template>
        <template #meta>
          <div class="entity-facts-grid entity-facts-grid--2 user-center-meta">
            <div class="entity-facts-grid__item">
              <span>{{ actionSections[0] }}</span>
              <strong>查看当前资料并手动刷新</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>{{ actionSections[1] }}</span>
              <strong>退出当前登录会话</strong>
            </div>
          </div>
        </template>
        <div class="entity-form">
          <section class="entity-form__section">
            <div class="entity-form__section-head">
              <h4>{{ actionSections[0] }}</h4>
              <p>当前页面展示的资料来自已登录账户；若资料发生更新，可刷新后重新同步到本地会话。</p>
            </div>
            <div class="user-center-actions">
              <el-button type="primary" :loading="refreshing" @click="refreshProfile">刷新资料</el-button>
            </div>
          </section>

          <section class="entity-form__section">
            <div class="entity-form__section-head">
              <h4>{{ actionSections[1] }}</h4>
              <p>当需要切换账户或结束当前工作台会话时，可从这里退出登录。</p>
            </div>
            <div class="user-center-actions">
              <el-button :loading="loggingOut" @click="logout">退出登录</el-button>
            </div>
          </section>
        </div>
        <template #footer>
          <div class="entity-panel-footer">
            <div>
              <span>当前角色</span>
              <strong>{{ overview.roleLabel }}</strong>
            </div>
            <div>
              <span>资料状态</span>
              <strong>已同步到当前会话</strong>
            </div>
          </div>
        </template>
      </SideInfoPanel>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import dayjs from 'dayjs'
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router'
import MetricCard from '@/components/common/MetricCard.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import { useAuthStore } from '@/stores/auth'
import { buildUserCenterOverview, getUserCenterActionSections } from './userCenterPresentation'

const router = useRouter()
const authStore = useAuthStore()
const refreshing = ref(false)
const loggingOut = ref(false)

const overview = computed(() => buildUserCenterOverview(authStore.user))
const createdAtLabel = computed(() => {
  const value = authStore.user?.createdAt
  return value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '--'
})
const actionSections = getUserCenterActionSections()

async function refreshProfile() {
  refreshing.value = true
  try {
    await authStore.refreshProfile()
    ElMessage.success('账户资料已刷新')
  } finally {
    refreshing.value = false
  }
}

async function logout() {
  loggingOut.value = true
  try {
    await authStore.logout()
    ElMessage.success('已退出登录')
    await router.push('/login')
  } finally {
    loggingOut.value = false
  }
}
</script>

<style scoped lang="scss">
.user-center-summary-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 24px;
}

.user-center-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(320px, 0.9fr);
  gap: 24px;
}

.user-center-meta {
  gap: 12px;
}

.user-center-actions {
  display: flex;
  gap: 12px;
}

@media (max-width: 1200px) {
  .user-center-summary-grid,
  .user-center-layout {
    grid-template-columns: 1fr;
  }
}
</style>
