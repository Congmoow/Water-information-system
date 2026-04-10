import { describe, expect, it } from 'vitest'
import {
  buildDashboardAlarmSnapshot,
  buildDashboardMetrics,
  buildDashboardSpatialSnapshot,
  getDashboardMeasurementMeta,
  buildTrendSummary
} from './dashboardPresentation'
import type { DashboardOverview } from '@/types/models'

const overview: DashboardOverview = {
  reservoirCount: 12,
  riverCount: 5,
  stationCount: 24,
  onlineStationCount: 18,
  todayAlarmCount: 6,
  waterLevelTrend: [
    { label: '04-08', value: 17.2 },
    { label: '04-09', value: 17.8 }
  ],
  rainfallTrend: [
    { label: '04-08', value: 22 },
    { label: '04-09', value: 18 }
  ],
  alarmLevelStats: [
    { name: 'Info', value: 2 },
    { name: 'Warning', value: 3 },
    { name: 'Critical', value: 1 }
  ],
  stationStatusStats: [],
  recentAlarms: [
    {
      id: 'a1',
      stationName: '一号站',
      level: 'Critical',
      status: 'Pending',
      message: '水位超阈值',
      triggeredAt: '2026-04-10T09:20:00'
    },
    {
      id: 'a2',
      stationName: '二号站',
      level: 'Warning',
      status: 'Resolved',
      message: '雨量偏高',
      triggeredAt: '2026-04-10T08:10:00'
    }
  ]
}

describe('dashboardPresentation', () => {
  it('builds KPI cards with semantic emphasis for risk metrics', () => {
    const metrics = buildDashboardMetrics(overview)

    expect(metrics).toHaveLength(4)
    expect(metrics[0]).toMatchObject({ title: '水库总数', tone: 'info' })
    expect(metrics[2]).toMatchObject({
      title: '在线站点',
      tone: 'success',
      highlight: '75%'
    })
    expect(metrics[3]).toMatchObject({
      title: '今日告警',
      tone: 'warning',
      highlight: '待关注'
    })
  })

  it('builds alarm snapshot for the dashboard summary strip', () => {
    const snapshot = buildDashboardAlarmSnapshot(overview)

    expect(snapshot.criticalCount).toBe(1)
    expect(snapshot.warningCount).toBe(3)
    expect(snapshot.pendingCount).toBe(1)
    expect(snapshot.latestTriggeredAt).toBe('2026-04-10T09:20:00')
  })

  it('builds trend summary with latest value and direction', () => {
    expect(buildTrendSummary(overview.waterLevelTrend)).toMatchObject({
      currentValue: 17.8,
      delta: 0.6,
      direction: 'up'
    })
    expect(buildTrendSummary(overview.rainfallTrend)).toMatchObject({
      currentValue: 18,
      delta: -4,
      direction: 'down'
    })
  })

  it('returns measurement metadata and spatial snapshot for dashboard support panels', () => {
    expect(getDashboardMeasurementMeta('waterLevel')).toMatchObject({
      label: '水位趋势',
      unit: 'm'
    })
    expect(getDashboardMeasurementMeta('rainfall')).toMatchObject({
      label: '雨量统计',
      unit: 'mm'
    })
    expect(buildDashboardSpatialSnapshot(overview)).toMatchObject({
      reservoirCount: 12,
      riverCount: 5,
      stationCount: 24,
      emphasis: '进入空间分布'
    })
  })
})
