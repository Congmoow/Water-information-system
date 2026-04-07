import http from '@/api/http'
import type { ApiResponse, PagedResult } from '@/types/common'
import type { ReservoirDetail, ReservoirItem } from '@/types/models'

export interface ReservoirQuery {
  page?: number
  pageSize?: number
  keyword?: string
}

export interface ReservoirFormModel {
  name: string
  location: string
  capacity: number
  managementUnit: string
  latitude: number
  longitude: number
  description: string
}

export async function fetchReservoirs(params: ReservoirQuery) {
  const response = await http.get<ApiResponse<PagedResult<ReservoirItem>>>('/api/reservoir', { params })
  return response.data.data
}

export async function fetchReservoirDetail(id: string) {
  const response = await http.get<ApiResponse<ReservoirDetail>>(`/api/reservoir/${id}`)
  return response.data.data
}

export async function createReservoir(payload: ReservoirFormModel) {
  const response = await http.post<ApiResponse<ReservoirDetail>>('/api/reservoir', payload)
  return response.data.data
}

export async function updateReservoir(id: string, payload: ReservoirFormModel) {
  const response = await http.put<ApiResponse<ReservoirDetail>>(`/api/reservoir/${id}`, payload)
  return response.data.data
}

export async function deleteReservoir(id: string) {
  await http.delete(`/api/reservoir/${id}`)
}
