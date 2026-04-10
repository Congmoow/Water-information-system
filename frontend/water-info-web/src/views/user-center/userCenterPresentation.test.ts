import { describe, expect, it } from 'vitest'
import {
  buildUserCenterOverview,
  getRoleMeta,
  getUserCenterActionSections
} from './userCenterPresentation'
import type { UserProfile } from '@/types/auth'

const adminUser: UserProfile = {
  id: 'u1',
  username: 'admin',
  fullName: '系统管理员',
  role: 'Administrator',
  createdAt: '2026-04-01T09:00:00'
}

describe('userCenterPresentation', () => {
  it('builds account overview from current authenticated user', () => {
    expect(buildUserCenterOverview(adminUser)).toMatchObject({
      username: 'admin',
      fullName: '系统管理员',
      accountStatus: 'Enabled',
      roleLabel: '系统管理员'
    })
  })

  it('returns readable role meta for permission copy and summaries', () => {
    expect(getRoleMeta('Administrator')).toMatchObject({
      label: '系统管理员',
      description: '可管理工程对象、监测记录与告警处置流程。'
    })
    expect(getRoleMeta('Operator')).toMatchObject({
      label: '业务用户'
    })
  })

  it('keeps account action sections grouped for profile and session management', () => {
    expect(getUserCenterActionSections()).toEqual(['账户资料', '会话操作'])
  })
})
