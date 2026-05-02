import { describe, expect, it } from 'vitest'
import {
  buildReservoirOverview,
  getReservoirFormSections,
  getReservoirPanelMeta
} from './reservoirPresentation'
import type { ReservoirDetail, ReservoirItem } from '@/types/models'

const rows: ReservoirItem[] = [
  {
    id: 'r1',
    name: '武湖水库',
    location: '武汉市新洲区',
    capacity: 620,
    managementUnit: '市水务局',
    latitude: 30.62,
    longitude: 114.34,
    description: '用于区域防洪调度',
    updatedAt: '2026-04-10T10:00:00'
  },
  {
    id: 'r2',
    name: '青山水库',
    location: '武汉市青山区',
    capacity: 480,
    managementUnit: '区水务局',
    latitude: 30.61,
    longitude: 114.41,
    description: '',
    updatedAt: '2026-04-10T09:00:00'
  }
]

describe('reservoirPresentation', () => {
  it('builds overview metrics from reservoir results', () => {
    expect(buildReservoirOverview(rows)).toMatchObject({
      total: 2,
      totalCapacity: 1100,
      largestReservoirName: '武湖水库'
    })
  })

  it('returns archive-oriented panel meta for reservoir detail', () => {
    const meta = getReservoirPanelMeta(rows[0] as ReservoirDetail)

    expect(meta.heading).toBe('重点资产档案')
    expect(meta.keyFacts[0]).toMatchObject({
      label: '设计容量',
      value: '620 万m³'
    })
  })

  it('keeps reservoir form sections stable', () => {
    expect(getReservoirFormSections()).toEqual({
      basic: { title: '基础信息', description: '定义名称、所在位置、容量和管理单位等核心资产属性。' },
      spatial: { title: '空间信息', description: '维护资产空间坐标，保证对象在地图和档案中的位置一致。' },
      supplement: { title: '补充说明', description: '补充说明资产用途、现场情况或维护备注。' }
    })
  })
})
