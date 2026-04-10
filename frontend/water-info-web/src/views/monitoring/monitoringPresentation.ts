import type { LineChartSeriesType } from '@/theme/tokens'
import type { MonitoringItem, TrendPoint } from '@/types/models'

export interface MonitoringMetricMeta {
  label: string
  description: string
  unit: string
  seriesType: LineChartSeriesType
}

export interface MonitoringOverview {
  total: number
  latestValue: number
  latestCollectedAt?: string
  triggeredCount: number
  delta: number
  direction: 'up' | 'down' | 'flat'
  metricLabel: string
}

export function getMonitoringMetricMeta(dataType?: string): MonitoringMetricMeta {
  if (dataType === 'Rainfall') {
    return {
      label: '雨量监测',
      description: '用于观察站点降雨变化和短时异常波动。',
      unit: 'mm',
      seriesType: 'rainfall'
    }
  }

  if (dataType === 'Flow') {
    return {
      label: '流量监测',
      description: '用于观察流量变化趋势，辅助判断来水与泄流压力。',
      unit: 'm³/s',
      seriesType: 'trendSecondary'
    }
  }

  return {
    label: '水位监测',
    description: '用于观察当前水位变化与阈值风险趋势。',
    unit: 'm',
    seriesType: 'waterLevel'
  }
}

export function buildMonitoringOverview(
  rows: MonitoringItem[],
  historyPoints: TrendPoint[],
  total: number,
  dataType?: string
): MonitoringOverview {
  const metricMeta = getMonitoringMetricMeta(dataType)
  const latestRow = [...rows].sort((left, right) => right.collectedAt.localeCompare(left.collectedAt))[0]
  const latestValue = historyPoints.at(-1)?.value ?? latestRow?.value ?? 0
  const previousValue = historyPoints.at(-2)?.value ?? latestValue
  const delta = Number((latestValue - previousValue).toFixed(2))

  return {
    total,
    latestValue,
    latestCollectedAt: latestRow?.collectedAt,
    triggeredCount: rows.filter((item) => item.triggeredAlarm).length,
    delta,
    direction: delta > 0 ? 'up' : delta < 0 ? 'down' : 'flat',
    metricLabel: metricMeta.label
  }
}

export function getMonitoringDialogSections() {
  return ['采集信息', '监测数值', '补充说明']
}
