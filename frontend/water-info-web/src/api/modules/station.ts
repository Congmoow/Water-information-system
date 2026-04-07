import http from '@/api/http'
import type { ApiResponse, PagedResult } from '@/types/common'
import type { StationDetail, StationItem } from '@/types/models'

export interface StationQuery {
  page?: number
  pageSize?: number
  keyword?: string
  type?: string
  status?: string
}

export interface StationFormModel {
  name: string
  type: string
  longitude: number
  latitude: number
  status: string
  warningThreshold: number
  criticalThreshold: number
  description: string
  lastActiveAt?: string
  reservoirId?: string
  riverId?: string
}

export async function fetchStations(params: StationQuery) {
  const response = await http.get<ApiResponse<PagedResult<StationItem>>>('/api/station', { params })
  return response.data.data
}

export async function fetchStationDetail(id: string) {
  const response = await http.get<ApiResponse<StationDetail>>(`/api/station/${id}`)
  return response.data.data
}

export async function createStation(payload: StationFormModel) {
  const response = await http.post<ApiResponse<StationDetail>>('/api/station', payload)
  return response.data.data
}

export async function updateStation(id: string, payload: StationFormModel) {
  const response = await http.put<ApiResponse<StationDetail>>(`/api/station/${id}`, payload)
  return response.data.data
}

export async function deleteStation(id: string) {
  await http.delete(`/api/station/${id}`)
}
