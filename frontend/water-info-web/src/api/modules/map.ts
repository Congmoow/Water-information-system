import http from '@/api/http'
import type { ApiResponse } from '@/types/common'
import type { MapPoint } from '@/types/models'

interface MapResponse {
  items: MapPoint[]
}

export async function fetchMapPoints() {
  const response = await http.get<ApiResponse<MapResponse>>('/api/map/points')
  return response.data.data.items
}
