import dayjs from 'dayjs'

/**
 * 将 ISO 日期字符串格式化为 'YYYY-MM-DD HH:mm' 显示格式
 */
export function formatDateTime(value?: string): string {
  if (!value) return '--'
  return dayjs(value).format('YYYY-MM-DD HH:mm')
}
