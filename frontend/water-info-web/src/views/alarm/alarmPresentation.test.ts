import { describe, expect, it } from 'vitest'
import {
  buildAlarmSummary,
  getAlarmLifecycleMeta
} from './alarmPresentation'
import type { AlarmDetail, AlarmItem } from '@/types/models'

const rows: AlarmItem[] = [
  {
    id: 'a1',
    stationId: 's1',
    stationName: '闸口站',
    alarmType: 'Threshold',
    level: 'Critical',
    status: 'Pending',
    message: '水位超过严重阈值',
    triggeredAt: '2026-04-10T11:00:00'
  },
  {
    id: 'a2',
    stationId: 's2',
    stationName: '雨量站',
    alarmType: 'Threshold',
    level: 'Warning',
    status: 'Processing',
    message: '雨量持续升高',
    triggeredAt: '2026-04-10T10:00:00'
  },
  {
    id: 'a3',
    stationId: 's3',
    stationName: '河道站',
    alarmType: 'Threshold',
    level: 'Info',
    status: 'Resolved',
    message: '阈值恢复正常',
    triggeredAt: '2026-04-10T09:00:00',
    handledAt: '2026-04-10T09:30:00'
  }
]

describe('alarmPresentation', () => {
  it('builds alarm summary cards for the current filtered result set', () => {
    const summary = buildAlarmSummary(rows, 18)

    expect(summary.total).toBe(18)
    expect(summary.pendingCount).toBe(2)
    expect(summary.criticalCount).toBe(1)
    expect(summary.resolvedCount).toBe(1)
    expect(summary.latestTriggeredAt).toBe('2026-04-10T11:00:00')
  })

  it('builds lifecycle meta for pending and resolved detail panels', () => {
    const pendingDetail = rows[0] as AlarmDetail
    const resolvedDetail = rows[2] as AlarmDetail

    expect(getAlarmLifecycleMeta(pendingDetail)).toMatchObject({
      heading: '仍需人工介入',
      tone: 'warning'
    })
    expect(getAlarmLifecycleMeta(resolvedDetail)).toMatchObject({
      heading: '已完成处置',
      tone: 'success'
    })
  })
})
