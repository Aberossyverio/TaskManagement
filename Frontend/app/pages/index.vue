<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 p-4">
    <UCard class="w-full max-w-md">
      <template #header>
        <div class="text-center space-y-2">
          <h1 class="text-3xl font-bold">Task Management</h1>
          <p class="text-muted">Sign in to your account</p>
        </div>
      </template>

      <UForm :state="state" class="space-y-4" @submit="handleLogin">
        <UFormField label="Username" name="username" required>
          <UInput
            v-model="state.username"
            placeholder="Enter your username"
            size="xl"
            icon="i-lucide-user"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Password" name="password" required>
          <UInput
            v-model="state.password"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Enter your password"
            size="xl"
            icon="i-lucide-lock"
            class="w-full"
          >
            <template #trailing>
              <UButton
                :icon="showPassword ? 'i-lucide-eye-off' : 'i-lucide-eye'"
                color="neutral"
                variant="link"
                size="sm"
                :padded="false"
                @click="showPassword = !showPassword"
              />
            </template>
          </UInput>
        </UFormField>

        <UButton
          type="submit"
          size="xl"
          block
          :loading="loading"
        >
          Sign In
        </UButton>
      </UForm>
    </UCard>
  </div>
</template>

<script setup lang="ts">
import { useAuthApi } from '~/features/auth/api'

const state = reactive({
  username: '',
  password: ''
})

const loading = ref(false)
const showPassword = ref(false)
const authApi = useAuthApi()

const handleLogin = async () => {
  loading.value = true
  try {
    const result = await authApi.login(state)
    console.log('Login completed, navigating to tasks...')
    
    // Ensure navigation happens after login is fully complete
    if (result && result.accessToken) {
      await navigateTo('/tasks', { replace: true })
    }
  } catch (error) {
    // Error already handled by handleResponseError in authApi
    console.error('Login failed, staying on login page')
  } finally {
    loading.value = false
  }
}
</script>
