import http from '@/api/http'
import type { ApiResponse } from '@/types/common'
import type { LoginRequest, LoginResult, UserProfile } from '@/types/auth'

export async function login(payload: LoginRequest): Promise<LoginResult> {
  const response = await http.post<ApiResponse<LoginResult>>('/api/auth/login', payload)
  return response.data.data
}

export async function getProfile(): Promise<UserProfile> {
  const response = await http.get<ApiResponse<UserProfile>>('/api/auth/profile')
  return response.data.data
}

export async function logout(): Promise<void> {
  await http.post('/api/auth/logout')
}
