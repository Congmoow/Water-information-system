import http from '@/api/http'
import type { ApiResponse, PagedResult } from '@/types/common'
import type { MonitoringCreateResult, MonitoringItem, TrendPoint } from '@/types/models'

export interface MonitoringQuery {
  stationId?: string
  dataType?: string
  startTime?: string
  endTime?: string
  page?: number
  pageSize?: number
}

export interface MonitoringFormModel {
  stationId: string
  dataType: string
  value: number
  collectedAt: string
  remark?: string
}

export async function fetchMonitoringRecords(params: MonitoringQuery) {
  const response = await http.get<ApiResponse<PagedResult<MonitoringItem>>>('/api/monitoring', { params })
  return response.data.data
}

export async function fetchMonitoringHistory(params: MonitoringQuery) {
  const response = await http.get<ApiResponse<TrendPoint[]>>('/api/monitoring/history', { params })
  return response.data.data
}

export async function createMonitoringRecord(payload: MonitoringFormModel) {
  const response = await http.post<ApiResponse<MonitoringCreateResult>>('/api/monitoring', payload)
  return response.data.data
}
