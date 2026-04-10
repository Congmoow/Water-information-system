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
    expect(getStationFormSections()).toEqual(['基础信息', '运行状态', '归属关系', '补充说明'])
    expect(stationTypeLabel('WaterLevel')).toBe('水位站')
    expect(stationTypeLabel('Rainfall')).toBe('雨量站')
  })
})
