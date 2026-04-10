export type StatusTagType = '' | 'success' | 'warning' | 'danger' | 'info' | 'primary'

export type StatusTagCategory =
  | 'alarmLevel'
  | 'alarmStatus'
  | 'stationStatus'
  | 'enabledStatus'
  | 'riskStatus'

export interface StatusTagMeta {
  label: string
  type: StatusTagType
}

const statusTagRegistry: Record<StatusTagCategory, Record<string, StatusTagMeta>> = {
  alarmLevel: {
    Info: { label: '提示', type: 'info' },
    Warning: { label: '预警', type: 'warning' },
    Critical: { label: '严重', type: 'danger' }
  },
  alarmStatus: {
    Pending: { label: '待处理', type: 'warning' },
    Processing: { label: '处理中', type: 'warning' },
    Resolved: { label: '已解决', type: 'success' }
  },
  stationStatus: {
    Online: { label: '在线', type: 'success' },
    Offline: { label: '离线', type: 'info' },
    Warning: { label: '预警', type: 'warning' }
  },
  enabledStatus: {
    Enabled: { label: '启用', type: 'success' },
    Disabled: { label: '停用', type: 'info' }
  },
  riskStatus: {
    Normal: { label: '正常', type: 'success' },
    Warning: { label: '预警', type: 'warning' },
    Critical: { label: '严重', type: 'danger' }
  }
}

export function resolveStatusTagMeta(category: StatusTagCategory, value?: string, fallbackLabel?: string): StatusTagMeta {
  if (!value) {
    return {
      label: fallbackLabel ?? '--',
      type: 'info'
    }
  }

  return statusTagRegistry[category][value] ?? {
    label: fallbackLabel ?? value,
    type: 'info'
  }
}
