import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { getProfile, login as loginApi, logout as logoutApi } from '@/api/modules/auth'
import { TOKEN_STORAGE_KEY, USER_STORAGE_KEY } from '@/constants/storage'
import type { LoginRequest, LoginResult, UserProfile } from '@/types/auth'

function parseUser(): UserProfile | null {
  const raw = localStorage.getItem(USER_STORAGE_KEY)
  return raw ? (JSON.parse(raw) as UserProfile) : null
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(TOKEN_STORAGE_KEY))
  const user = ref<UserProfile | null>(parseUser())
  const loading = ref(false)
  const isAuthenticated = computed(() => Boolean(token.value))

  function setSession(result: LoginResult) {
    token.value = result.token
    user.value = result.user
    localStorage.setItem(TOKEN_STORAGE_KEY, result.token)
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(result.user))
  }

  function clearSession() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_STORAGE_KEY)
    localStorage.removeItem(USER_STORAGE_KEY)
  }

  async function login(payload: LoginRequest) {
    loading.value = true
    try {
      const result = await loginApi(payload)
      setSession(result)
      return result
    } finally {
      loading.value = false
    }
  }

  async function refreshProfile() {
    if (!token.value) {
      return null
    }

    const profile = await getProfile()
    user.value = profile
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(profile))
    return profile
  }

  async function logout() {
    try {
      if (token.value) {
        await logoutApi()
      }
    } finally {
      clearSession()
    }
  }

  return {
    token,
    user,
    loading,
    isAuthenticated,
    setSession,
    clearSession,
    login,
    refreshProfile,
    logout
  }
})
