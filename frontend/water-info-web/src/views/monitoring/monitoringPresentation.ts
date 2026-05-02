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

export interface MonitoringDialogSections {
  collect: { title: string; description: string }
  value: { title: string; description: string }
  supplement: { title: string; description: string }
}

export function getMonitoringDialogSections(): MonitoringDialogSections {
  return {
    collect: { title: '采集信息', description: '定义采样归属的站点、监测指标与采集时间。' },
    value: { title: '监测数值', description: '录入本次采样的监测值，系统会沿用当前阈值规则进行风险判断。' },
    supplement: { title: '补充说明', description: '补充记录采样背景、现场情况或需要在历史记录中保留的说明。' }
  }
}
