import http from '@/api/http'
import type { ApiResponse, PagedResult } from '@/types/common'
import type { AlarmDetail, AlarmItem } from '@/types/models'

export interface AlarmQuery {
  stationId?: string
  level?: string
  status?: string
  startTime?: string
  endTime?: string
  page?: number
  pageSize?: number
}

export interface AlarmHandleFormModel {
  status: string
  handleRemark?: string
}

export async function fetchAlarms(params: AlarmQuery) {
  const response = await http.get<ApiResponse<PagedResult<AlarmItem>>>('/api/alarm', { params })
  return response.data.data
}

export async function fetchAlarmDetail(id: string) {
  const response = await http.get<ApiResponse<AlarmDetail>>(`/api/alarm/${id}`)
  return response.data.data
}

export async function handleAlarm(id: string, payload: AlarmHandleFormModel) {
  const response = await http.put<ApiResponse<AlarmDetail>>(`/api/alarm/${id}/handle`, payload)
  return response.data.data
}
