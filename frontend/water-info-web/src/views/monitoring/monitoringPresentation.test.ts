import { describe, expect, it } from 'vitest'
import {
  buildMonitoringOverview,
  getMonitoringDialogSections,
  getMonitoringMetricMeta
} from './monitoringPresentation'
import type { MonitoringItem, TrendPoint } from '@/types/models'

const rows: MonitoringItem[] = [
  {
    id: 'm1',
    stationId: 's1',
    stationName: '闸口一号站',
    dataType: 'WaterLevel',
    value: 18.6,
    collectedAt: '2026-04-10T09:30:00',
    triggeredAlarm: false,
    remark: '运行正常'
  },
  {
    id: 'm2',
    stationId: 's1',
    stationName: '闸口一号站',
    dataType: 'WaterLevel',
    value: 19.2,
    collectedAt: '2026-04-10T10:00:00',
    triggeredAlarm: true,
    remark: '接近预警阈值'
  }
]

const history: TrendPoint[] = [
  { label: '09:00', value: 18.3 },
  { label: '09:30', value: 18.6 },
  { label: '10:00', value: 19.2 }
]

describe('monitoringPresentation', () => {
  it('builds monitoring overview for summary and main trend modules', () => {
    expect(buildMonitoringOverview(rows, history, 18, 'WaterLevel')).toMatchObject({
      total: 18,
      latestValue: 19.2,
      latestCollectedAt: '2026-04-10T10:00:00',
      triggeredCount: 1,
      delta: 0.6,
      direction: 'up',
      metricLabel: '水位监测'
    })
  })

  it('returns metric meta for current monitoring type', () => {
    expect(getMonitoringMetricMeta('Rainfall')).toMatchObject({
      label: '雨量监测',
      seriesType: 'rainfall',
      unit: 'mm'
    })
    expect(getMonitoringMetricMeta('Flow')).toMatchObject({
      label: '流量监测',
      seriesType: 'trendSecondary',
      unit: 'm³/s'
    })
  })

  it('keeps record dialogs grouped for collection context and remarks', () => {
    expect(getMonitoringDialogSections()).toEqual(['采集信息', '监测数值', '补充说明'])
  })
})
