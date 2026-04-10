import type { StationDetail, StationItem } from '@/types/models'

export function stationTypeLabel(value: string) {
  if (value === 'WaterLevel') return '水位站'
  if (value === 'Rainfall') return '雨量站'
  if (value === 'Flow') return '流量站'
  return value
}

export function buildStationOverview(rows: StationItem[]) {
  return {
    total: rows.length,
    onlineCount: rows.filter((item) => item.status === 'Online').length,
    warningCount: rows.filter((item) => item.status === 'Warning').length,
    offlineCount: rows.filter((item) => item.status === 'Offline').length
  }
}

export function getStationStatusInsight(detail: StationDetail) {
  if (detail.status === 'Warning') {
    return {
      heading: '存在预警风险',
      description: '当前站点处于预警状态，应优先查看阈值和最近活跃情况。',
      tone: 'warning' as const
    }
  }

  if (detail.status === 'Offline') {
    return {
      heading: '当前离线',
      description: '该站点当前未在线，建议先确认最近活跃时间和现场通讯状态。',
      tone: 'info' as const
    }
  }

  return {
    heading: '运行正常',
    description: '该站点当前在线且未处于预警状态，可作为正常运行节点继续监测。',
    tone: 'success' as const
  }
}

export function getStationFormSections() {
  return ['基础信息', '运行状态', '归属关系', '补充说明']
}
