import type { CategoryCount, DashboardOverview, TrendPoint } from '@/types/models'

export type DashboardMetricTone = 'info' | 'success' | 'warning'

export interface DashboardMetricItem {
  title: string
  value: number
  description: string
  highlight: string
  tone: DashboardMetricTone
}

export interface DashboardAlarmSnapshot {
  criticalCount: number
  warningCount: number
  pendingCount: number
  latestTriggeredAt?: string
}

export interface TrendSummary {
  currentValue: number
  previousValue: number
  delta: number
  direction: 'up' | 'down' | 'flat'
}

export interface DashboardMeasurementMeta {
  label: string
  unit: string
}

export interface DashboardSpatialSnapshot {
  reservoirCount: number
  riverCount: number
  stationCount: number
  emphasis: string
}

function getCategoryValue(items: CategoryCount[], keys: string[]) {
  const normalizedKeys = keys.map((item) => item.toLowerCase())
  return items.reduce((total, item) => {
    const currentName = item.name.toLowerCase()
    if (normalizedKeys.includes(currentName)) {
      return total + item.value
    }

    return total
  }, 0)
}

export function buildDashboardMetrics(overview: DashboardOverview): DashboardMetricItem[] {
  const onlineRate = overview.stationCount > 0
    ? `${Math.round((overview.onlineStationCount / overview.stationCount) * 100)}%`
    : '0%'

  return [
    {
      title: '水库总数',
      value: overview.reservoirCount,
      description: '纳入系统统一维护的水库工程',
      highlight: '工程档案',
      tone: 'info'
    },
    {
      title: '河道总数',
      value: overview.riverCount,
      description: '纳入系统展示的主要河道',
      highlight: '流域总览',
      tone: 'info'
    },
    {
      title: '在线站点',
      value: overview.onlineStationCount,
      description: `当前在线 ${overview.onlineStationCount} / 全部 ${overview.stationCount}`,
      highlight: onlineRate,
      tone: 'success'
    },
    {
      title: '今日告警',
      value: overview.todayAlarmCount,
      description: '今日自动触发的告警记录',
      highlight: overview.todayAlarmCount > 0 ? '待关注' : '运行稳定',
      tone: overview.todayAlarmCount > 0 ? 'warning' : 'success'
    }
  ]
}

export function buildDashboardAlarmSnapshot(overview: DashboardOverview): DashboardAlarmSnapshot {
  const latestTriggeredAt = overview.recentAlarms
    .map((item) => item.triggeredAt)
    .sort((left, right) => right.localeCompare(left))[0]

  return {
    criticalCount: getCategoryValue(overview.alarmLevelStats, ['Critical', '严重']),
    warningCount: getCategoryValue(overview.alarmLevelStats, ['Warning', '预警']),
    pendingCount: overview.recentAlarms.filter((item) => item.status !== 'Resolved').length,
    latestTriggeredAt
  }
}

export function buildTrendSummary(points: TrendPoint[]): TrendSummary {
  const current = points.at(-1)?.value ?? 0
  const previous = points.at(-2)?.value ?? current
  const delta = Number((current - previous).toFixed(2))

  return {
    currentValue: current,
    previousValue: previous,
    delta,
    direction: delta > 0 ? 'up' : delta < 0 ? 'down' : 'flat'
  }
}

export function getDashboardMeasurementMeta(metric: 'waterLevel' | 'rainfall'): DashboardMeasurementMeta {
  if (metric === 'rainfall') {
    return {
      label: '雨量统计',
      unit: 'mm'
    }
  }

  return {
    label: '水位趋势',
    unit: 'm'
  }
}

export function buildDashboardSpatialSnapshot(overview: DashboardOverview): DashboardSpatialSnapshot {
  return {
    reservoirCount: overview.reservoirCount,
    riverCount: overview.riverCount,
    stationCount: overview.stationCount,
    emphasis: '进入空间分布'
  }
}
