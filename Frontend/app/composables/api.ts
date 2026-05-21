import axios, { type AxiosInstance } from 'axios'

const TOKEN_KEY = 'auth_token'
const REFRESH_TOKEN_KEY = 'refresh_token'

export function useAuth() {
  const cookie = useCookie(TOKEN_KEY, { maxAge: 60 * 60 * 24 * 7 })
  const refreshCookie = useCookie(REFRESH_TOKEN_KEY, { maxAge: 60 * 60 * 24 * 30 })

  const getToken = () => {
    return cookie.value || null
  }

  const getRefreshToken = () => {
    return refreshCookie.value || null
  }

  const setToken = (token: string) => {
    cookie.value = token
  }

  const setRefreshToken = (token: string) => {
    refreshCookie.value = token
  }

  const clearTokens = () => {
    cookie.value = null
    refreshCookie.value = null
  }

  return {
    getToken,
    getRefreshToken,
    setToken,
    setRefreshToken,
    clearTokens
  }
}

export function useApi(): AxiosInstance {
  const config = useRuntimeConfig()
  const auth = useAuth()

  const instance = axios.create({
    baseURL: config.public.apiBase as string,
    headers: {
      'Content-Type': 'application/json'
    }
  })

  instance.interceptors.request.use((config) => {
    const token = auth.getToken()
    console.log('🔑 API Request to:', config.url)
    console.log('🔑 Token from cookie:', token ? `${token.substring(0, 20)}...` : 'NOT FOUND')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
      console.log('✅ Authorization header set')
    } else {
      console.error('❌ No token available!')
    }
    return config
  })

  instance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        auth.clearTokens()
        if (import.meta.client) {
          navigateTo('/')
        }
      }
      return Promise.reject(error)
    }
  )

  return instance
}

export async function $authedFetch<T>(url: string, options?: any): Promise<T> {
  const api = useApi()
  const response = await api.get(url, options)
  return response.data
}

export function handleResponseError(error: any) {
  const toast = useToast()
  
  if (error.response) {
    if (error.response.data?.errors) {
      return
    }
    const message = error.response.data?.error?.message || error.response.data?.message || error.response.statusText
    toast.add({
      title: 'Error',
      description: message,
      color: 'error'
    })
  } else if (error.request) {
    toast.add({
      title: 'Network Error',
      description: 'Unable to connect to the server',
      color: 'error'
    })
  } else {
    toast.add({
      title: 'Error',
      description: error.message || 'An unexpected error occurred',
      color: 'error'
    })
  }
}
