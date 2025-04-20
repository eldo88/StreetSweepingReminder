import { defineStore } from 'pinia'
import { setGlobalTimeout } from '@/utils/globalTmeout'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    isLoggedIn: false,
    userId: null,
    email: null,
    tokenExpirationInMinutes: null,
    tokenExpirationTimestamp: null,
    token: localStorage.getItem('jwtToken') || null,
  }),

  getters: {
    userIsLoggedIn: (state) => !!state.token,
    getUserId: (state) => state.userId || null,
    getEmail: (state) => state.email || null,
  },

  actions: {
    async login(credentials) {
      try {
        const response = await fetch('http://localhost:5010/api/Auth/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(credentials),
        })

        const data = await response.json()

        if (!response.ok) {
          throw new Error(data.message || 'Login failed')
        }
        console.log(data)
        this.token = data.token
        this.userId = data.userId
        this.email = data.email
        this.tokenExpirationInMinutes = data.tokenExpirationInMinutes
        this.tokenExpirationTimestamp = data.tokenExpirationTimestamp
        this.isLoggedIn = true

        console.log('User id in auth store: ' + this.userId)

        localStorage.setItem('jwtToken', data.token)
        setGlobalTimeout(this.tokenExpirationInMinutes)
      } catch (error) {
        console.error('Login error:', error)
        throw error // Optionally re-throw to let the component handle it
      }
    },

    async register({ email, password }) {
      try {
        const response = await fetch('http://localhost:5010/api/Auth/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ email, password }),
        })

        const data = await response.json()

        if (!response.ok) {
          throw new Error(data.message || 'Registration failed')
        }

        console.log('Registration successful', data)

        await this.login({ email, password })
      } catch (error) {
        console.error('Registration error:', error)
        throw error
      }
    },

    logout() {
      this.token = null
      this.user = null
      this.isLoggedIn = false
      localStorage.removeItem('jwtToken')
    },

    initialize() {
      const token = localStorage.getItem('jwtToken')
      if (token) {
        this.token = token
        this.isLoggedIn = true
      }
    },
  },

  persist: true,
})
