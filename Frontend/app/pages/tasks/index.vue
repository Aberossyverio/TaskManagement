<template>
  <div class="min-h-screen bg-default">
    <header class="bg-elevated border-b border-default sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div class="flex justify-between items-center gap-3">
          <div>
            <h1 class="text-xl sm:text-2xl font-bold text-highlighted">Task Management</h1>
            <p class="text-xs sm:text-sm text-muted mt-1">Organize your work efficiently</p>
          </div>
          <div class="flex items-center gap-2 sm:gap-3">
            <UColorModeButton size="md" />
            <UButton 
              @click="openLogoutModal" 
              color="error"
              variant="soft"
              icon="i-lucide-log-out"
              size="md"
            >
              <span class="hidden sm:inline">Logout</span>
            </UButton>
          </div>
        </div>
      </div>
    </header>

    <main ref="mainRef" class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      <div class="relative z-50 mb-6">
        <SearchBar
          v-model:search-query="searchQuery"
          v-model:status-filter="statusFilter"
          @create="openCreateModal"
        />
      </div>

      <div v-if="loading && tasks.length === 0" class="flex justify-center py-20">
        <UIcon name="i-lucide-loader-2" class="w-12 h-12 animate-spin text-primary" />
      </div>

      <EmptyState v-else-if="tasks.length === 0" @create="openCreateModal" />

      <div v-else>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
          <TaskCard
            v-for="task in tasks"
            :key="task.id"
            :task="task"
            @edit="openEditModal"
            @delete="openDeleteModal"
          />
        </div>

        <div v-if="loadingMore" class="flex justify-center py-8">
          <UIcon name="i-lucide-loader-2" class="w-8 h-8 animate-spin text-primary" />
        </div>

        <div v-if="totalPages > 1" class="hidden sm:flex justify-center mt-8">
          <UPagination
            :page="currentPage"
            :total="totalCount"
            :items-per-page="pageSize"
            @update:page="handlePageChange"
          />
        </div>
      </div>
    </main>

    <Transition name="fade">
      <UButton
        v-if="showBackToTop"
        icon="i-lucide-arrow-up"
        color="primary"
        size="lg"
        class="sm:hidden fixed bottom-6 right-6 z-50 shadow-lg rounded-full"
        @click="scrollToTop"
      />
    </Transition>

    <UModal v-model:open="showModal" :title="isEditing ? 'Edit Task' : 'Create New Task'" description="Fill in the task details below.">
      <template #body>
        <UForm id="task-form" :state="localForm" :errors="formErrors" class="space-y-4" @submit="handleSubmit">
          <UFormField label="Title" name="title" required :error="formErrors.title">
            <UInput
              v-model="localForm.title"
              placeholder="Enter task title"
              size="xl"
              class="w-full"
            />
          </UFormField>

          <UFormField label="Description" name="description" :error="formErrors.description">
            <UTextarea
              v-model="localForm.description"
              placeholder="Enter task description"
              :rows="4"
              size="xl"
              class="w-full"
            />
          </UFormField>

          <UFormField label="Status" name="status" required :error="formErrors.status">
            <USelect
              v-model="localForm.status"
              :items="statusOptions"
              size="xl"
              class="w-full"
            />
          </UFormField>
        </UForm>
      </template>

      <template #footer="{ close }">
        <UButton
          label="Cancel"
          color="neutral"
          variant="outline"
          @click="close"
        />
        <UButton
          type="submit"
          form="task-form"
          :label="isEditing ? 'Update' : 'Create'"
          :loading="submitting"
        />
      </template>
    </UModal>

    <UModal v-model:open="showDeleteModal" title="Delete Task" description="Are you sure you want to delete this task? This action cannot be undone.">
      <template #footer="{ close }">
        <UButton label="Cancel" color="neutral" variant="outline" @click="close" />
        <UButton label="Delete" color="error" :loading="deleting" @click="confirmDelete" />
      </template>
    </UModal>

    <UModal v-model:open="showLogoutModal" title="Logout" description="Are you sure you want to logout?">
      <template #footer="{ close }">
        <UButton label="Cancel" color="neutral" variant="outline" @click="close" />
        <UButton label="Logout" color="error" @click="confirmLogout" />
      </template>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { useAuthApi } from '~/features/auth/api'
import { useApi, handleResponseError } from '~/composables/api'
import type { Task } from '~/task-api-contract'
import { useInfiniteScroll, useWindowSize } from '@vueuse/core'

definePageMeta({
  middleware: 'auth'
})

const authApi = useAuthApi()
const toast = useToast()

const loading = ref(false)
const loadingMore = ref(false)
const submitting = ref(false)
const deleting = ref(false)
const tasks = ref<Task[]>([])
const showModal = ref(false)
const showDeleteModal = ref(false)
const showLogoutModal = ref(false)
const isEditing = ref(false)
const editingTaskId = ref<number | null>(null)
const deletingTaskId = ref<number | null>(null)
const searchQuery = ref('')
const statusFilter = ref('')
const currentPage = ref(1)
const pageSize = ref(9)
const totalPages = ref(0)
const totalCount = ref(0)
const mainRef = ref<HTMLElement | null>(null)
const showBackToTop = ref(false)

const { width } = useWindowSize()
const isMobile = computed(() => width.value < 640)

// Watch for viewport changes and reset
watch(isMobile, async (newIsMobile, oldIsMobile) => {
  if (oldIsMobile !== undefined && newIsMobile !== oldIsMobile) {
    currentPage.value = 1
    tasks.value = []
    await loadTasks()
  }
})

const form = ref({
  title: '',
  description: '',
  status: 'Todo'
})

const localForm = ref({ ...form.value })
const formErrors = ref<Record<string, string>>({})

const statusOptions = [
  { label: 'To Do', value: 'Todo' },
  { label: 'In Progress', value: 'InProgress' },
  { label: 'Done', value: 'Done' }
]

watch([searchQuery, statusFilter], async () => {
  currentPage.value = 1
  tasks.value = []
  await loadTasks()
})

watch(() => showModal.value, (isOpen) => {
  if (isOpen) {
    localForm.value = { ...form.value }
    formErrors.value = {}
  }
})

const loadTasks = async (append = false) => {
  if (append) {
    loadingMore.value = true
  } else {
    loading.value = true
  }
  
  try {
    const params: any = {
      page: currentPage.value,
      rowsPerPage: pageSize.value
    }
    if (searchQuery.value) params.title = searchQuery.value
    if (statusFilter.value) params.status = statusFilter.value
    
    const api = useApi()
    const result = await api.get('/tasks', { params })
    
    if (append) {
      tasks.value = [...tasks.value, ...result.data.rows]
    } else {
      tasks.value = result.data.rows
    }
    
    totalCount.value = result.data.totalRows
    totalPages.value = result.data.totalPages
  } catch (err) {
    handleResponseError(err)
  } finally {
    loading.value = false
    loadingMore.value = false
  }
}

const handlePageChange = async (page: number) => {
  currentPage.value = page
  await loadTasks()
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const openCreateModal = () => {
  isEditing.value = false
  form.value = { title: '', description: '', status: 'Todo' }
  showModal.value = true
}

const openEditModal = (task: Task) => {
  isEditing.value = true
  editingTaskId.value = task.id
  form.value = {
    title: task.title,
    description: task.description,
    status: task.taskStatus
  }
  showModal.value = true
}

const openDeleteModal = (id: number) => {
  deletingTaskId.value = id
  showDeleteModal.value = true
}

const openLogoutModal = () => {
  showLogoutModal.value = true
}

const confirmLogout = () => {
  authApi.logout()
  showLogoutModal.value = false
}

const handleSubmit = async () => {
  formErrors.value = {}
  
  if (!localForm.value.title?.trim()) {
    formErrors.value.title = 'Title is required'
    return
  }
  
  submitting.value = true
  try {
    const api = useApi()
    if (isEditing.value && editingTaskId.value) {
      await api.put(`/tasks/${editingTaskId.value}`, localForm.value)
      toast.add({
        title: 'Success',
        description: 'Task updated successfully',
        color: 'success'
      })
    } else {
      await api.post('/tasks', localForm.value)
      toast.add({
        title: 'Success',
        description: 'Task created successfully',
        color: 'success'
      })
    }
    showModal.value = false
    currentPage.value = 1
    tasks.value = []
    await loadTasks()
  } catch (err: any) {
    if (err.response?.data?.errors) {
      const errors = err.response.data.errors
      Object.keys(errors).forEach(key => {
        const fieldName = key.toLowerCase()
        formErrors.value[fieldName] = errors[key][0]
      })
    }
  } finally {
    submitting.value = false
  }
}

const confirmDelete = async () => {
  if (!deletingTaskId.value) return
  
  deleting.value = true
  try {
    const api = useApi()
    await api.delete(`/tasks/${deletingTaskId.value}`)
    toast.add({
      title: 'Success',
      description: 'Task deleted successfully',
      color: 'success'
    })
    showDeleteModal.value = false
    currentPage.value = 1
    tasks.value = []
    await loadTasks()
  } catch (err) {
    handleResponseError(err)
  } finally {
    deleting.value = false
  }
}

onMounted(() => {
  loadTasks()
  
  // Infinite scroll only for mobile
  useInfiniteScroll(
    window,
    () => {
      if (isMobile.value && currentPage.value < totalPages.value && !loadingMore.value && !loading.value) {
        currentPage.value++
        loadTasks(true)
      }
    },
    { distance: 300 }
  )
  
  // Back to top button visibility
  window.addEventListener('scroll', () => {
    showBackToTop.value = window.scrollY > 300
  })
})
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
