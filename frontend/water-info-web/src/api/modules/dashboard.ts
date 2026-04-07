import http from '@/api/http'
import type { ApiResponse } from '@/types/common'
import type { DashboardOverview } from '@/types/models'

export async function fetchDashboardOverview() {
  const response = await http.get<ApiResponse<DashboardOverview>>('/api/dashboard/overview')
  return response.data.data
}
