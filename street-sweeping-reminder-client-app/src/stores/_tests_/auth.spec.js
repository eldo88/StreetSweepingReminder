import { describe, it, expect, beforeEach, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useAuthStore } from '../auth'

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.mock('@/services/api', () => ({
      default: {
        post: vi.fn(),
      },
    }))
  })

  it('should update state on successful login', async () => {
    const store = useAuthStore()
    const mockUserData = {
      token: 'fake-token',
      userId: '123',
      email: 'test@example.com',
    }

    globalThis.fetch = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve(mockUserData),
    })

    await store.login({ email: 'test@example.com', password: 'password' })

    expect(store.isLoggedIn).toBe(true)
    expect(store.token).toBe(mockUserData.token)
    expect(store.userId).toBe(mockUserData.userId)
  })
})
