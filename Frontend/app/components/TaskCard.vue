<template>
  <UCard>
    <template #header>
      <div class="flex justify-between items-start gap-3">
        <h3 class="text-lg font-semibold flex-1">{{ task.title }}</h3>
        <UBadge :color="statusColor" variant="subtle">
          {{ statusLabel }}
        </UBadge>
      </div>
    </template>

    <p class="text-muted text-sm line-clamp-3 min-h-[3.75rem]">{{ task.description || 'No description' }}</p>

    <template #footer>
      <div class="flex items-center justify-between">
        <span class="text-xs text-dimmed">{{ formattedDate }}</span>
        <div class="flex gap-2">
          <UButton
            icon="i-lucide-pencil"
            size="sm"
            color="primary"
            variant="ghost"
            @click="$emit('edit', task)"
          />
          <UButton
            icon="i-lucide-trash-2"
            size="sm"
            color="error"
            variant="ghost"
            @click="$emit('delete', task.id)"
          />
        </div>
      </div>
    </template>
  </UCard>
</template>

<script setup lang="ts">
import type { Task } from '~/task-api-contract'

const props = defineProps<{
  task: Task
}>()

defineEmits<{
  edit: [task: Task]
  delete: [id: number]
}>()

const statusColor = computed(() => {
  const colors: Record<string, string> = {
    'Todo': 'info',
    'InProgress': 'warning',
    'Done': 'success'
  }
  return colors[props.task.taskStatus] || 'neutral'
})

const statusLabel = computed(() => {
  const labels: Record<string, string> = {
    'Todo': 'To Do',
    'InProgress': 'In Progress',
    'Done': 'Done'
  }
  return labels[props.task.taskStatus] || props.task.taskStatus
})

const formattedDate = computed(() => {
  const date = new Date(props.task.createTime)
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
})
</script>

<style scoped>
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
