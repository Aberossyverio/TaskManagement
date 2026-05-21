export default defineNuxtConfig({
  srcDir: 'app',
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  
  modules: ['@nuxt/ui'],
  
  css: ['~/assets/css/main.css'],
  
  colorMode: {
    preference: 'system'
  },
  
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5000'
    }
  },

  app: {
    head: {
      title: 'Task Management',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ]
    }
  }
})
