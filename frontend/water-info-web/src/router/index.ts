import { createRouter, createWebHistory } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/auth/LoginView.vue'),
      meta: { public: true, title: '登录' }
    },
    {
      path: '/',
      component: () => import('@/layout/AppLayout.vue'),
      children: [
        { path: '', redirect: '/dashboard' },
        { path: 'dashboard', name: 'dashboard', component: () => import('@/views/dashboard/DashboardView.vue'), meta: { title: '首页仪表盘' } },
        { path: 'reservoirs', name: 'reservoirs', component: () => import('@/views/reservoir/ReservoirView.vue'), meta: { title: '水库管理', requiresAdmin: true } },
        { path: 'rivers', name: 'rivers', component: () => import('@/views/river/RiverView.vue'), meta: { title: '河道管理', requiresAdmin: true } },
        { path: 'stations', name: 'stations', component: () => import('@/views/station/StationView.vue'), meta: { title: '站点管理', requiresAdmin: true } },
        { path: 'monitoring', name: 'monitoring', component: () => import('@/views/monitoring/MonitoringView.vue'), meta: { title: '监测数据' } },
        { path: 'alarms', name: 'alarms', component: () => import('@/views/alarm/AlarmView.vue'), meta: { title: '告警记录', requiresAdmin: true } },
        { path: 'approvals', name: 'approvals', component: () => import('@/views/approval/ApprovalListView.vue'), meta: { title: '审批管理' } },
        { path: 'approvals/create', name: 'approvalCreate', component: () => import('@/views/approval/ApprovalCreateView.vue'), meta: { title: '新建审批' } },
        { path: 'approvals/:id/review', name: 'reviewResult', component: () => import('@/views/approval/ReviewResultView.vue'), meta: { title: '初审结果' } },
        { path: 'user-center', name: 'userCenter', component: () => import('@/views/user-center/UserCenterView.vue'), meta: { title: '用户中心' } }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/dashboard'
    }
  ]
})

router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (to.path === '/login' && authStore.isAuthenticated) {
    return '/dashboard'
  }

  if (to.meta.public) {
    return true
  }

  if (!authStore.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }

  // M8: 管理员权限路由守卫
  if (to.meta.requiresAdmin && authStore.user?.role !== 'Administrator') {
    ElMessage.warning('该页面需要管理员权限')
    return '/dashboard'
  }

  return true
})

export default router
