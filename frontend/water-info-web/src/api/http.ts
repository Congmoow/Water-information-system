import axios from 'axios'
import { ElMessage } from 'element-plus'
import { TOKEN_STORAGE_KEY, USER_STORAGE_KEY } from '@/constants/storage'

// 扩展 AxiosRequestConfig 以支持跳过全局错误提示
declare module 'axios' {
  interface AxiosRequestConfig {
    skipGlobalError?: boolean
  }
}

const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api',
  timeout: 15000
})

http.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

http.interceptors.response.use(
  (response) => response,
  (error) => {
    // 标记了 skipGlobalError 的请求，由调用方自行处理错误提示
    if (error.config?.skipGlobalError) {
      return Promise.reject(error)
    }

    if (error.response?.status === 401) {
      localStorage.removeItem(TOKEN_STORAGE_KEY)
      localStorage.removeItem(USER_STORAGE_KEY)
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
      return Promise.reject(error)
    }

    // 从后端 ApiResponse 结构中提取错误消息
    const apiMessage = error.response?.data?.message

    switch (error.response?.status) {
      case 403:
        ElMessage.error(apiMessage ?? '权限不足，无法执行此操作')
        break
      case 404:
        ElMessage.error(apiMessage ?? '请求的资源不存在')
        break
      case 422:
        ElMessage.error(apiMessage ?? '提交的数据格式有误')
        break
      case 409:
        ElMessage.error(apiMessage ?? '数据冲突，请刷新后重试')
        break
      default:
        if (error.code === 'ERR_NETWORK') {
          ElMessage.error('网络连接异常，请检查网络后重试')
        } else if (error.response?.status && error.response.status >= 500) {
          ElMessage.error(apiMessage ?? '服务器内部错误，请稍后重试')
        } else if (error.code === 'ECONNABORTED') {
          ElMessage.error('请求超时，请稍后重试')
        } else if (apiMessage) {
          ElMessage.error(apiMessage)
        }
        break
    }

    return Promise.reject(error)
  }
)

export default http
