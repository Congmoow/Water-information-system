export interface AppMenuItem {
  path: string
  name: string
  title: string
  icon: string
}

export const appMenus: AppMenuItem[] = [
  { path: '/dashboard', name: 'dashboard', title: '首页仪表盘', icon: 'DataAnalysis' },
  { path: '/reservoirs', name: 'reservoirs', title: '水库管理', icon: 'Collection' },
  { path: '/rivers', name: 'rivers', title: '河道管理', icon: 'Guide' },
  { path: '/stations', name: 'stations', title: '站点管理', icon: 'Location' },
  { path: '/monitoring', name: 'monitoring', title: '监测数据', icon: 'Histogram' },
  { path: '/alarms', name: 'alarms', title: '告警记录', icon: 'Bell' },
  { path: '/map', name: 'map', title: '地图展示', icon: 'MapLocation' },
  { path: '/user-center', name: 'userCenter', title: '用户中心', icon: 'User' }
]
