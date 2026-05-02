import { describe, expect, it } from 'vitest'
import {
  buildRiverOverview,
  getRiverFormSections,
  getRiverPanelMeta
} from './riverPresentation'
import type { RiverDetail, RiverItem } from '@/types/models'

const rows: RiverItem[] = [
  {
    id: 'rv1',
    name: '长江武汉段',
    basin: '长江流域',
    length: 86.5,
    latitude: 30.59,
    longitude: 114.39,
    description: '城市主干河段',
    updatedAt: '2026-04-10T11:00:00'
  },
  {
    id: 'rv2',
    name: '汉江支流',
    basin: '汉江流域',
    length: 42.4,
    latitude: 30.55,
    longitude: 114.22,
    description: '',
    updatedAt: '2026-04-10T09:00:00'
  }
]

describe('riverPresentation', () => {
  it('builds overview metrics from river results', () => {
    expect(buildRiverOverview(rows)).toMatchObject({
      total: 2,
      totalLength: 128.9,
      majorRiverName: '长江武汉段'
    })
  })

  it('returns archive-oriented panel meta for river detail', () => {
    const meta = getRiverPanelMeta(rows[0] as RiverDetail)

    expect(meta.heading).toBe('河道档案概览')
    expect(meta.keyFacts[0]).toMatchObject({
      label: '河道长度',
      value: '86.5 km'
    })
  })

  it('keeps river form sections stable', () => {
    expect(getRiverFormSections()).toEqual({
      basic: { title: '基础信息', description: '定义河道名称、长度及所属流域等基础属性。' },
      spatial: { title: '空间信息', description: '维护河道空间坐标，保证在地图和档案中的位置一致。' },
      supplement: { title: '补充说明', description: '补充说明河道特征、水文情况或维护备注。' }
    })
  })
})
