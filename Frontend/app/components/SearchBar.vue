<template>
  <div class="p-4 bg-elevated border border-default rounded-lg">
    <div class="flex flex-col sm:flex-row gap-3">
      <UInput
        :model-value="searchQuery"
        @update:model-value="$emit('update:searchQuery', $event)"
        icon="i-lucide-search"
        size="xl"
        placeholder="Search tasks..."
        class="flex-1"
      />

      <div class="relative w-full sm:w-48">
        <USelect
          :model-value="statusFilter"
          @update:model-value="$emit('update:statusFilter', $event)"
          :items="statusOptions"
          value-key="value"
          :color="statusColor"
          :highlight="!!statusFilter"
          variant="soft"
          size="xl"
          placeholder="All Status"
          trailing-icon="i-lucide-chevron-down"
          class="w-full"
        />
        <UButton
          v-if="statusFilter"
          icon="i-lucide-x"
          color="neutral"
          variant="ghost"
          size="xs"
          class="absolute right-8 top-1/2 -translate-y-1/2"
          @click="$emit('update:statusFilter', '')"
        />
      </div>

      <UButton
        icon="i-lucide-plus"
        size="xl"
        class="w-full sm:w-auto"
        @click="$emit('create')"
      >
        <span class="hidden sm:inline">New Task</span>
        <span class="sm:hidden">New</span>
      </UButton>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  searchQuery: string
  statusFilter: string
}>()

defineEmits<{
  'update:searchQuery': [value: string]
  'update:statusFilter': [value: string]
  create: []
}>()

const statusOptions = [
  { label: 'To Do', value: 'Todo' },
  { label: 'In Progress', value: 'InProgress' },
  { label: 'Done', value: 'Done' }
]

const statusColor = computed((): 'neutral' | 'info' | 'warning' | 'success' => {
  const colors: Record<string, 'neutral' | 'info' | 'warning' | 'success'> = {
    '': 'neutral',
    'Todo': 'info',
    'InProgress': 'warning',
    'Done': 'success'
  }
  return colors[props.statusFilter] || 'neutral'
})
</script>
