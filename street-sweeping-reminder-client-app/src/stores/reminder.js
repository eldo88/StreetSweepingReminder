import { defineStore } from 'pinia'
import { useAuthStore } from './auth'
import api from '@/services/api'

const authStore = useAuthStore()

// int Id, string Message, DateTime ScheduledDateTimeUtc, string Status, string PhoneNumber, int StreetId

export const useRemindersStore = defineStore('reminders', {
  state: () => ({
    userId: authStore.getUserId(),
    reminders: [],
  }),

  actions: {
    async getReminders(userId) {
      if (userId) {
        api
          .get('Reminder')
          .then((response) => {
            console.log('Data: ', response.data)
            if (response) {
              this.reminders = response
            }
          })
          .catch((error) => {
            console.error('Error: ', error)
          })
      }
    },
  },
})
