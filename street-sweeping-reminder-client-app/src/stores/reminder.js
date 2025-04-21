import { defineStore } from 'pinia'
import { useAuthStore } from './auth'
import { useStreetsStore } from './streets'
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
        const streetsStore = useStreetsStore()
        const payload = {
          title: formData.title,
          scheduledDateTimeUtc: formData.reminderDate.toISOString(),
          status: 'Pending',
          phoneNumber: formData.phoneNumber,
          streetId: streetsStore.streetId,
          isRecurring: formData.isRecurring,
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
  },
})
