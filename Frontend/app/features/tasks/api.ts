import { useApi, handleResponseError } from '~/composables/api'
import type { Task, CreateTaskRequest, UpdateTaskRequest, PagingResult, GetTasksRequest, ApiResponse } from '~/task-api-contract'

export function useTaskApi() {
  const api = useApi()

  const getAll = async (params?: GetTasksRequest) => {
    try {
      const response = await api.get<PagingResult<Task>>('/tasks', { params })
      return response.data
    } catch (error) {
      handleResponseError(error)
      throw error
    }
  }

  const getById = async (id: number) => {
    try {
      const response = await api.get<ApiResponse<Task>>(`/tasks/${id}`)
      return response.data.data!
    } catch (error) {
      handleResponseError(error)
      throw error
    }
  }

  const create = async (task: CreateTaskRequest) => {
    try {
      const response = await api.post<ApiResponse<Task>>('/tasks', task)
      return response.data.data!
    } catch (error) {
      handleResponseError(error)
      throw error
    }
  }

  const update = async (id: number, task: UpdateTaskRequest) => {
    try {
      const response = await api.put<ApiResponse<Task>>(`/tasks/${id}`, task)
      return response.data.data!
    } catch (error) {
      handleResponseError(error)
      throw error
    }
  }

  const remove = async (id: number) => {
    try {
      await api.delete(`/tasks/${id}`)
    } catch (error) {
      handleResponseError(error)
      throw error
    }
  }

  return {
    getAll,
    getById,
    create,
    update,
    remove
  }
}
