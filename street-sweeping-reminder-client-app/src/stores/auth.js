import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    isLoggedIn: false,
    username: null,
    userId: null,
    email: null,
    tokenExpirationInMinutes: null,
    tokenExpirationTimestamp: null,
    token: localStorage.getItem('jwtToken') || null,
  }),

  getters: {
    userIsLoggedIn: (state) => !!state.token,
    getUser: (state) => state.user,
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

        this.token = data.token
        this.username = data.username
        this.userId = data.userId
        this.email = data.email
        this.tokenExpirationInMinutes = data.tokenExpirationInMinutes
        this.tokenExpirationTimestamp = data.tokenExpirationTimestamp
        this.isLoggedIn = true

        localStorage.setItem('jwtToken', data.token)
      } catch (error) {
        console.error('Login error:', error)
        throw error // Optionally re-throw to let the component handle it
      }
    },

    async register({ username, email, password }) {
      try {
        const response = await fetch('http://localhost:5010/api/Auth/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ username, email, password }),
        })

        const data = await response.json()

        if (!response.ok) {
          throw new Error(data.message || 'Registration failed')
        }

        console.log('Registration successful', data)

        // Optionally: log them in immediately
        await this.login({ username, password })
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
})
