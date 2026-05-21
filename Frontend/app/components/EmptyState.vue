<template>
  <UCard>
    <div class="text-center py-12">
      <UIcon name="i-lucide-clipboard-list" class="w-16 h-16 mx-auto text-muted mb-4" />
      <h3 class="text-lg font-semibold mb-2">No tasks found</h3>
      <p class="text-muted mb-6">Get started by creating your first task</p>
      <UButton
        icon="i-lucide-plus"
        size="xl"
        @click="$emit('create')"
      >
        Create Task
      </UButton>
    </div>
  </UCard>
</template>

<script setup lang="ts">
defineEmits<{
  create: []
}>()
</script>
