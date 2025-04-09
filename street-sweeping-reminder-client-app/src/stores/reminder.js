import { defineStore } from 'pinia'
import { useAuthStore } from './auth'
import api from '@/services/api'

// int Id, string Message, DateTime ScheduledDateTimeUtc, string Status, string PhoneNumber, int StreetId

export const useRemindersStore = defineStore('reminders', {
  state: () => ({
    reminders: [],
  }),

  actions: {
    async getReminders() {
      try {
        const authStore = useAuthStore()
        const userId = authStore.getUserId()

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
  },
})
