import { describe, expect, it } from 'vitest'
import {
  buildMapOverview,
  getMapPointInsight,
  pointTypeLabel
} from './mapPresentation'
import type { MapPoint } from '@/types/models'

const points: MapPoint[] = [
  {
    id: 'r1',
    name: '武湖水库',
    type: 'Reservoir',
    latitude: 30.62,
    longitude: 114.34,
    description: '重点水库',
    source: 'reservoir'
  },
  {
    id: 'rv1',
    name: '长港河段',
    type: 'River',
    latitude: 30.59,
    longitude: 114.31,
    description: '河道巡检区段',
    source: 'river'
  },
  {
    id: 's1',
    name: '闸口一号站',
    type: 'WaterLevel',
    latitude: 30.63,
    longitude: 114.36,
    description: '水位监测点',
    status: 'Warning',
    source: 'station'
  },
  {
    id: 's2',
    name: '闸口二号站',
    type: 'Rainfall',
    latitude: 30.61,
    longitude: 114.33,
    description: '雨量监测点',
    status: 'Offline',
    source: 'station'
  }
]

describe('mapPresentation', () => {
  it('builds map overview with source and state distribution', () => {
    expect(buildMapOverview(points, points[2])).toMatchObject({
      total: 4,
      engineeringCount: 2,
      stationCount: 2,
      warningCount: 1,
      offlineCount: 1,
      activePointName: '闸口一号站'
    })
  })

  it('returns readable point insight for space detail panels', () => {
    expect(getMapPointInsight(points[2])).toMatchObject({
      heading: '存在预警关注',
      tone: 'warning'
    })
    expect(getMapPointInsight(points[3])).toMatchObject({
      heading: '当前离线',
      tone: 'info'
    })
    expect(getMapPointInsight(points[0])).toMatchObject({
      heading: '工程对象已纳入空间监测',
      tone: 'success'
    })
  })

  it('keeps point labels object-oriented across map sources', () => {
    expect(pointTypeLabel(points[0])).toBe('水库工程')
    expect(pointTypeLabel(points[1])).toBe('河道工程')
    expect(pointTypeLabel(points[2])).toBe('监测站点 / WaterLevel')
  })
})
