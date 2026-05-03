export interface AppMenuItem {
  path: string
  name: string
  title: string
  icon: string
}

export interface SettingsMenuItem {
  name: string
  title: string
  icon: string
  path?: string
  action?: 'logout'
}

export interface SettingsMenuGroup {
  id: string
  items: SettingsMenuItem[]
}

export const appMenus: AppMenuItem[] = [
  { path: '/dashboard', name: 'dashboard', title: '首页仪表盘', icon: 'DataAnalysis' },
  { path: '/reservoirs', name: 'reservoirs', title: '水库管理', icon: 'Collection' },
  { path: '/rivers', name: 'rivers', title: '河道管理', icon: 'Guide' },
  { path: '/stations', name: 'stations', title: '站点管理', icon: 'Location' },
  { path: '/monitoring', name: 'monitoring', title: '监测数据', icon: 'Histogram' },
  { path: '/alarms', name: 'alarms', title: '告警记录', icon: 'Bell' },
  { path: '/approvals', name: 'approvals', title: '审批管理', icon: 'Document' }
]

export const settingsMenuGroups: SettingsMenuGroup[] = [
  {
    id: 'profile',
    items: [{ path: '/user-center', name: 'userCenter', title: '个人中心', icon: 'User' }]
  },
  {
    id: 'session',
    items: [{ name: 'logout', title: '退出登录', icon: 'SwitchButton', action: 'logout' }]
  }
]
