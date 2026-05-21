import { useApi, useAuth, handleResponseError } from '~/composables/api'
import type { LoginRequest, LoginResponse, ApiResponse } from '~/task-api-contract'

export function useAuthApi() {
  const api = useApi()
  const auth = useAuth()

  const login = async (credentials: LoginRequest) => {
    try {
      const response = await api.post<ApiResponse<LoginResponse>>('/auth/login', credentials)
      console.log('Login response:', response.data)
      const data = response.data.data!
      console.log('Token data:', data)
      auth.setToken(data.accessToken)
      auth.setRefreshToken(data.refreshToken)
      
      // Verify tokens are saved
      const savedToken = auth.getToken()
      console.log('Token verification:', savedToken ? 'Token saved successfully' : 'Token save failed')
      
      return data
    } catch (error) {
      console.error('Login error:', error)
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
