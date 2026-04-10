import type { MapPoint } from '@/types/models'

export interface MapOverview {
  total: number
  engineeringCount: number
  stationCount: number
  reservoirCount: number
  riverCount: number
  warningCount: number
  offlineCount: number
  activePointName: string
}

export interface MapPointInsight {
  heading: string
  description: string
  tone: 'success' | 'warning' | 'info'
}

export type MapPointStatusMeta =
  | {
      kind: 'status'
      category: 'riskStatus' | 'stationStatus'
      value: 'Warning' | 'Offline' | 'Normal'
    }
  | {
      kind: 'identity'
      label: string
    }

export function pointTypeLabel(point: MapPoint) {
  if (point.source === 'reservoir') return '水库工程'
  if (point.source === 'river') return '河道工程'
  return `监测站点 / ${point.type}`
}

export function buildMapOverview(points: MapPoint[], activePoint: MapPoint | null): MapOverview {
  return {
    total: points.length,
    engineeringCount: points.filter((item) => item.source !== 'station').length,
    stationCount: points.filter((item) => item.source === 'station').length,
    reservoirCount: points.filter((item) => item.source === 'reservoir').length,
    riverCount: points.filter((item) => item.source === 'river').length,
    warningCount: points.filter((item) => item.status === 'Warning').length,
    offlineCount: points.filter((item) => item.status === 'Offline').length,
    activePointName: activePoint?.name ?? '--'
  }
}

export function getMapPointInsight(point: MapPoint): MapPointInsight {
  if (point.status === 'Warning') {
    return {
      heading: '存在预警关注',
      description: '该点位当前处于预警状态，建议优先查看对象详情和关联监测上下文。',
      tone: 'warning'
    }
  }

  if (point.status === 'Offline') {
    return {
      heading: '当前离线',
      description: '该站点当前未在线，建议确认通信状态和最近一次上报时间。',
      tone: 'info'
    }
  }

  return {
    heading: point.source === 'station' ? '运行状态稳定' : '工程对象已纳入空间监测',
    description: point.source === 'station'
      ? '该监测站点当前未显示离线或预警状态，可继续结合地图与详情面板查看。'
      : '该空间对象已纳入统一地图视图，可在侧栏继续查看对象属性与说明信息。',
    tone: 'success'
  }
}

export function getMapPointStatusMeta(point: MapPoint): MapPointStatusMeta {
  if (point.source !== 'station') {
    return {
      kind: 'identity',
      label: pointTypeLabel(point)
    }
  }

  if (point.status === 'Offline') {
    return {
      kind: 'status',
      category: 'stationStatus',
      value: 'Offline'
    }
  }

  if (point.status === 'Warning') {
    return {
      kind: 'status',
      category: 'riskStatus',
      value: 'Warning'
    }
  }

  return {
    kind: 'status',
    category: 'riskStatus',
    value: 'Normal'
  }
}
