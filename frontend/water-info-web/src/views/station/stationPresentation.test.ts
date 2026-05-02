import { describe, expect, it } from 'vitest'
import {
  buildStationOverview,
  getStationFormSections,
  getStationStatusInsight,
  stationTypeLabel
} from './stationPresentation'
import type { StationDetail, StationItem } from '@/types/models'

const rows: StationItem[] = [
  {
    id: '1',
    name: '闸口一号站',
    type: 'WaterLevel',
    longitude: 114.12,
    latitude: 30.22,
    status: 'Online',
    warningThreshold: 18.5,
    criticalThreshold: 20,
    description: '',
    parentName: '武湖水库',
    lastActiveAt: '2026-04-10T10:00:00',
    updatedAt: '2026-04-10T10:00:00'
  },
  {
    id: '2',
    name: '闸口二号站',
    type: 'Rainfall',
    longitude: 114.15,
    latitude: 30.28,
    status: 'Warning',
    warningThreshold: 40,
    criticalThreshold: 60,
    description: '',
    parentName: '武湖水库',
    lastActiveAt: '2026-04-10T09:20:00',
    updatedAt: '2026-04-10T09:20:00'
  },
  {
    id: '3',
    name: '闸口三号站',
    type: 'Flow',
    longitude: 114.18,
    latitude: 30.3,
    status: 'Offline',
    warningThreshold: 10,
    criticalThreshold: 12,
    description: '',
    parentName: '长江河段',
    lastActiveAt: '2026-04-09T20:00:00',
    updatedAt: '2026-04-09T20:00:00'
  }
]

describe('stationPresentation', () => {
  it('builds station summary cards from current list results', () => {
    expect(buildStationOverview(rows)).toMatchObject({
      total: 3,
      onlineCount: 1,
      warningCount: 1,
      offlineCount: 1
    })
  })

  it('returns readable status insights for station detail panels', () => {
    const detail = rows[1] as StationDetail
    const offlineDetail = rows[2] as StationDetail

    expect(getStationStatusInsight(detail)).toMatchObject({
      heading: '存在预警风险',
      tone: 'warning'
    })
    expect(getStationStatusInsight(offlineDetail)).toMatchObject({
      heading: '当前离线',
      tone: 'info'
    })
  })

  it('keeps station form grouped for archive and runtime configuration', () => {
    expect(getStationFormSections()).toEqual({
      basic: { title: '基础信息', description: '定义站点名称与监测类型，作为对象档案的基础标识。' },
      status: { title: '运行状态', description: '设置当前运行状态、最近活跃时间以及告警阈值。' },
      affiliation: { title: '归属关系', description: '维护站点的空间坐标以及归属工程关系。' },
      supplement: { title: '补充说明', description: '补充说明当前站点用途、现场情况或维护备注。' }
    })
    expect(stationTypeLabel('WaterLevel')).toBe('水位站')
    expect(stationTypeLabel('Rainfall')).toBe('雨量站')
  })
})
