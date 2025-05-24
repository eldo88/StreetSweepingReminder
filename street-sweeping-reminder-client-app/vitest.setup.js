import { config } from '@vue/test-utils'
import { vi, afterEach } from 'vitest'

// Mock Vue Router
config.global.mocks = {
  $router: {
    push: vi.fn(),
    replace: vi.fn(),
  },
  $route: {
    params: {},
    query: {},
  },
}

// Mock toast notifications
vi.mock('vue-toastification', () => ({
  useToast: () => ({
    error: vi.fn(),
    success: vi.fn(),
  }),
}))

// Reset handlers after each test
afterEach(() => {
  vi.clearAllMocks()
})
