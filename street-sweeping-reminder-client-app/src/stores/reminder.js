import { defineStore } from 'pinia'
import { useAuthStore } from './auth'
import api from '@/services/api'

export const useRemindersStore = defineStore('reminders', {
  state: () => ({
    reminders: [],
    streets: [],
    isLoading: false,
  }),

  actions: {
    async getReminders() {
      try {
        const authStore = useAuthStore()
        const userId = authStore.getUserId

        if (!userId) {
          console.warn('No user ID found.')
          return
        }

        const response = await api.get(`Reminder/${userId}`)

        if (response?.data) {
          this.reminders = response.data
        }
      } catch (error) {
        console.error('Failed to fetch reminders:', error)
        if (error.response?.status === 401) {
          // Show login modal
          window.dispatchEvent(new CustomEvent('unauthorized'))
        } else {
          throw error
        }
      }
    },

    async createReminder(formData) {
      try {
        const authStore = useAuthStore()
        const userId = authStore.getUserId
        console.log('User ID in createReminder: ' + userId)
        if (!userId) {
          console.warn('No user ID found.')
          return
        }

        const payload = {
          title: formData.title,
          scheduledDateTimeUtc: formData.reminderDate.toISOString(),
          status: 'Pending',
          phoneNumber: formData.phoneNumber,
          streetId: 1, // hard coded for now, need to implement api call
          isRecurring: formData.isRecurring,
          weekOfMonth: 2, // hard coded for now
          streetSweepingDate: formData.streetSweepingDate.toISOString(),
        }

        const response = await api.post('Reminder', payload)

        return response.data
      } catch (error) {
        console.error('Failed to create reminder:', error)
        console.log(`Error code: ${error.response?.status}`)
        if (error.response?.status === 401) {
          // Show login modal
          window.dispatchEvent(new CustomEvent('unauthorized'))
        } else {
          throw error
        }
      }
    },

    async getStreets() {
      try {
        if (this.streets.length === 0) {
          console.log(`Streets len: ${this.streets.length}`)
          const response = await api.get('Street/getAll')
          if (response.data) {
            this.streets = response.data
          }
        }
      } catch (error) {
        console.error('Failed to get streets' + error)
      }
    },

    async searchStreets(query = '') {
      if (!query.trim()) {
        this.streets = []
        return
      }
      this.isLoading = true
      try {
        const encodedQuery = encodeURIComponent(query)
        const response = await api.get(`Street/search?query=${encodedQuery}`)

        if (response.data) {
          this.streets = response.data
        } else {
          this.streets = []
        }
      } catch (error) {
        console.error('Failed to get streets' + error)
        this.streets = []
      } finally {
        this.isLoading = false
      }
    },

    clearStreets() {
      this.streets = []
      this.isLoading = false
    },
  },
})
