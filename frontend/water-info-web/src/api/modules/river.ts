import http from '@/api/http'
import type { ApiResponse, PagedResult } from '@/types/common'
import type { RiverDetail, RiverItem } from '@/types/models'

export interface RiverQuery {
  page?: number
  pageSize?: number
  keyword?: string
}

export interface RiverFormModel {
  name: string
  length: number
  basin: string
  latitude: number
  longitude: number
  description: string
}

export async function fetchRivers(params: RiverQuery) {
  const response = await http.get<ApiResponse<PagedResult<RiverItem>>>('/api/river', { params })
  return response.data.data
}

export async function fetchRiverDetail(id: string) {
  const response = await http.get<ApiResponse<RiverDetail>>(`/api/river/${id}`)
  return response.data.data
}

export async function createRiver(payload: RiverFormModel) {
  const response = await http.post<ApiResponse<RiverDetail>>('/api/river', payload)
  return response.data.data
}

export async function updateRiver(id: string, payload: RiverFormModel) {
  const response = await http.put<ApiResponse<RiverDetail>>(`/api/river/${id}`, payload)
  return response.data.data
}

export async function deleteRiver(id: string) {
  await http.delete(`/api/river/${id}`)
}
