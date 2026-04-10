import { describe, expect, it } from 'vitest'
import { resolveStatusTagMeta } from '@/components/common/statusTag'

describe('resolveStatusTagMeta', () => {
  it('uses warning semantics for pending alarm workflow states', () => {
    const pending = resolveStatusTagMeta('alarmStatus', 'Pending')
    const processing = resolveStatusTagMeta('alarmStatus', 'Processing')

    expect(pending.type).toBe('warning')
    expect(pending.label).toBe('待处理')
    expect(processing.type).toBe('warning')
    expect(processing.label).toBe('处理中')
  })

  it('maps alarm levels and station states into stable semantic colors', () => {
    expect(resolveStatusTagMeta('alarmLevel', 'Critical')).toMatchObject({
      type: 'danger',
      label: '严重'
    })
    expect(resolveStatusTagMeta('alarmLevel', 'Warning')).toMatchObject({
      type: 'warning',
      label: '预警'
    })
    expect(resolveStatusTagMeta('stationStatus', 'Offline')).toMatchObject({
      type: 'info',
      label: '离线'
    })
    expect(resolveStatusTagMeta('stationStatus', 'Online')).toMatchObject({
      type: 'success',
      label: '在线'
    })
  })

  it('supports normal warning critical triads without reusing accent as warning', () => {
    expect(resolveStatusTagMeta('riskStatus', 'Normal')).toMatchObject({
      type: 'success',
      label: '正常'
    })
    expect(resolveStatusTagMeta('riskStatus', 'Warning')).toMatchObject({
      type: 'warning',
      label: '预警'
    })
    expect(resolveStatusTagMeta('riskStatus', 'Critical')).toMatchObject({
      type: 'danger',
      label: '严重'
    })
  })
})
