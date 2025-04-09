import { defineStore } from 'pinia'
import { useAuthStore } from './auth'
import api from '@/services/api'

export const useRemindersStore = defineStore('reminders', {
  state: () => ({
    reminders: [],
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

        const response = await api.get('Reminder/${userId}')

        if (response?.data) {
          this.reminders = response.data
        }
      } catch (error) {
        console.error('Failed to fetch reminders:', error)
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
          message: formData.message,
          scheduledDateTimeUtc: formData.date.toISOString(),
          status: 'Pending',
          phoneNumber: formData.phoneNumber,
          streetId: 1, // hard coded for now, need to implement api call
        }

        const response = await api.post('Reminder', payload)

        if (response?.data) {
          this.reminders.push(response.data)
        }

        return response.data
      } catch (error) {
        console.error('Failed to create reminder:', error)
        throw error
      }
    },
  },
})
