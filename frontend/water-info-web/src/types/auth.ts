export interface UserProfile {
  id: string
  username: string
  fullName: string
  role: string
  createdAt: string
}

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResult {
  token: string
  expiresAt: string
  user: UserProfile
}
