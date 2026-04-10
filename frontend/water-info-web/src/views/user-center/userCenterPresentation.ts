import type { UserProfile } from '@/types/auth'

export interface RoleMeta {
  label: string
  description: string
}

export interface UserCenterOverview {
  username: string
  fullName: string
  roleLabel: string
  roleDescription: string
  accountStatus: 'Enabled'
}

export function getRoleMeta(role?: string): RoleMeta {
  if (role === 'Administrator') {
    return {
      label: '系统管理员',
      description: '可管理工程对象、监测记录与告警处置流程。'
    }
  }

  return {
    label: '业务用户',
    description: '可查看监测总览、空间分布与当前授权范围内的数据。'
  }
}

export function buildUserCenterOverview(user: UserProfile | null): UserCenterOverview {
  const roleMeta = getRoleMeta(user?.role)

  return {
    username: user?.username ?? '--',
    fullName: user?.fullName ?? '--',
    roleLabel: roleMeta.label,
    roleDescription: roleMeta.description,
    accountStatus: 'Enabled'
  }
}

export function getUserCenterActionSections() {
  return ['账户资料', '会话操作']
}
