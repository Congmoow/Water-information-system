import type { AlarmDetail, AlarmItem } from '@/types/models'

export interface AlarmSummary {
  total: number
  pendingCount: number
  criticalCount: number
  resolvedCount: number
  latestTriggeredAt?: string
}

export interface AlarmLifecycleMeta {
  heading: string
  description: string
  tone: 'warning' | 'success' | 'danger' | 'info'
}

export function buildAlarmSummary(rows: AlarmItem[], total: number): AlarmSummary {
  const latestTriggeredAt = rows
    .map((item) => item.triggeredAt)
    .sort((left, right) => right.localeCompare(left))[0]

  return {
    total,
    pendingCount: rows.filter((item) => item.status === 'Pending' || item.status === 'Processing').length,
    criticalCount: rows.filter((item) => item.level === 'Critical').length,
    resolvedCount: rows.filter((item) => item.status === 'Resolved').length,
    latestTriggeredAt
  }
}

export function getAlarmLifecycleMeta(detail: AlarmDetail): AlarmLifecycleMeta {
  if (detail.status === 'Resolved') {
    return {
      heading: '已完成处置',
      description: '该告警已形成处理结果，可用于复盘本次处置过程。',
      tone: 'success'
    }
  }

  if (detail.status === 'Processing') {
    return {
      heading: '处理中',
      description: '当前事件已进入处理流程，需继续跟踪结果和备注更新。',
      tone: 'warning'
    }
  }

  return {
    heading: '仍需人工介入',
    description: detail.level === 'Critical'
      ? '该事件仍处于待处理状态，且等级较高，应优先确认现场风险。'
      : '该事件尚未形成处理结论，建议尽快确认处置动作和记录。',
    tone: 'warning'
  }
}
