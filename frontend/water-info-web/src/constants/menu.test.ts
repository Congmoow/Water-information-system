import { describe, expect, it } from 'vitest'
import { appMenus, settingsMenuGroups } from './menu'

describe('menu constants', () => {
  it('keeps user center out of top-level business menus', () => {
    expect(appMenus.map((item) => item.name)).not.toContain('userCenter')
  })

  it('groups profile and logout under settings navigation', () => {
    expect(
      settingsMenuGroups.map((group) => ({
        id: group.id,
        items: group.items.map((item) => ({
          name: item.name,
          title: item.title
        }))
      }))
    ).toEqual([
      {
        id: 'profile',
        items: [{ name: 'userCenter', title: '个人中心' }]
      },
      {
        id: 'session',
        items: [{ name: 'logout', title: '退出登录' }]
      }
    ])
  })
})
