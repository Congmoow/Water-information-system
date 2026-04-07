import { createRouter, createWebHistory } from 'vue-router'
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
        { path: 'reservoirs', name: 'reservoirs', component: () => import('@/views/reservoir/ReservoirView.vue'), meta: { title: '水库管理' } },
        { path: 'rivers', name: 'rivers', component: () => import('@/views/river/RiverView.vue'), meta: { title: '河道管理' } },
        { path: 'stations', name: 'stations', component: () => import('@/views/station/StationView.vue'), meta: { title: '站点管理' } },
        { path: 'monitoring', name: 'monitoring', component: () => import('@/views/monitoring/MonitoringView.vue'), meta: { title: '监测数据' } },
        { path: 'alarms', name: 'alarms', component: () => import('@/views/alarm/AlarmView.vue'), meta: { title: '告警记录' } },
        { path: 'map', name: 'map', component: () => import('@/views/map/MapView.vue'), meta: { title: '地图展示' } },
        { path: 'user-center', name: 'userCenter', component: () => import('@/views/user-center/UserCenterView.vue'), meta: { title: '用户中心' } }
      ]
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

  return true
})

export default router
