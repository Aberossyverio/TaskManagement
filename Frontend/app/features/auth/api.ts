import { useApi, useAuth, handleResponseError } from '~/composables/api'
import type { LoginRequest, LoginResponse, ApiResponse } from '~/task-api-contract'

export function useAuthApi() {
  const api = useApi()
  const auth = useAuth()

  const login = async (credentials: LoginRequest) => {
    try {
      const response = await api.post<ApiResponse<LoginResponse>>('/auth/login', credentials)
      const data = response.data.data!
      auth.setToken(data.accessToken)
      auth.setRefreshToken(data.refreshToken)
      
      return data
    } catch (error) {
      handleResponseError(error)
      throw error
    }
  }

  const logout = () => {
    auth.clearTokens()
    navigateTo('/')
  }

  return {
    login,
    logout
  }
}
