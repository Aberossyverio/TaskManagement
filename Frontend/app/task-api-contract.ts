// API Response wrapper from backend
export interface ApiResponse<T = any> {
  data?: T
  isSuccess: boolean
  error?: {
    code: string
    message: string
  }
}

// Auth types
export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  userId: number
  username: string
  fullname: string
}

// Task types
export interface Task {
  id: number
  title: string
  description: string
  taskStatus: string
  userId: number
  createTime: string
  updateTime?: string
}

export interface CreateTaskRequest {
  title: string
  description: string
  status: string
}

export interface UpdateTaskRequest {
  title: string
  description: string
  status: string
}

// Paging types
export interface PagingQuery {
  page?: number
  pageSize?: number
  search?: string
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
}

export interface PagingResult<T> {
  rows: T[]
  totalRows: number
  page: number
  rowsPerPage: number
  totalPages: number
}

export interface GetTasksRequest extends PagingQuery {
  title?: string
  status?: string
}
