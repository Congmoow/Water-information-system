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

export interface StationFormSections {
  basic: { title: string; description: string }
  status: { title: string; description: string }
  affiliation: { title: string; description: string }
  supplement: { title: string; description: string }
}

export function getStationFormSections(): StationFormSections {
  return {
    basic: { title: '基础信息', description: '定义站点名称与监测类型，作为对象档案的基础标识。' },
    status: { title: '运行状态', description: '设置当前运行状态、最近活跃时间以及告警阈值。' },
    affiliation: { title: '归属关系', description: '维护站点的空间坐标以及归属工程关系。' },
    supplement: { title: '补充说明', description: '补充说明当前站点用途、现场情况或维护备注。' }
  }
}
