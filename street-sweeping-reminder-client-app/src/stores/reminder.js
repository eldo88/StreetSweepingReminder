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

    async deleteReminder(reminderId) {
      if (!reminderId) {
        console.error('deleteReminder action called without a reminderId!')
        throw new Error('Reminder ID is required to delete a reminder.')
      }
      try {
        const response = await api.delete(`Reminder/${reminderId}`)
        if (response.status === 200) {
          this.reminders = this.reminders.filter((reminder) => reminder.id !== reminderId)
        }
      } catch (error) {
        console.error('Failed to delete reminder:', error)
        if (error.response?.status === 401) {
          // Show login modal
          window.dispatchEvent(new CustomEvent('unauthorized'))
        } else {
          throw error
        }
      }
    },

    async cancelIndividualReminder(reminderId, scheduleItemId) {
      if (!reminderId || !scheduleItemId) {
        console.error(
          'cancelIndividualReminder action called without reminderId or scheduleItemId!',
        )
        throw new Error('Both Reminder ID and Schedule Item ID are required to cancel.')
      }

      try {
        const response = await api.post(`Reminder/${scheduleItemId}/cancel`)

        if (response.status === 200) {
          const reminderIndex = this.reminders.findIndex((r) => r.id === reminderId)

          if (reminderIndex !== -1) {
            const reminder = this.reminders[reminderIndex]
            if (reminder.reminderSchedule?.schedule) {
              const scheduleItemIndex = reminder.reminderSchedule.schedule.findIndex(
                (item) => item.id === scheduleItemId,
              )

              if (scheduleItemIndex !== -1) {
                this.reminders[reminderIndex].reminderSchedule.schedule[
                  scheduleItemIndex
                ].isActive = false
                console.log(
                  `Successfully updated isActive flag for schedule item ${scheduleItemId} in reminder ${reminderId}`,
                )
              } else {
                console.warn(
                  `Schedule item with ID ${scheduleItemId} not found within reminder ${reminderId} in the store.`,
                )
              }
            } else {
              console.warn(`Reminder ${reminderId} schedule data is missing or invalid in store.`)
            }
          } else {
            console.warn(`Reminder with ID ${reminderId} not found in the store.`)
          }
        } else {
          console.warn(
            `Cancellation request for ${scheduleItemId} returned status ${response.status}`,
          )
          throw new Error(`API responded with status ${response.status}`)
        }
      } catch (error) {
        console.error(`Failed to cancel reminder schedule item ${scheduleItemId}:`, error)
        if (error.response?.status === 401) {
          window.dispatchEvent(new CustomEvent('unauthorized'))
        } else {
          throw error
        }
      }
    },

    async createReminder(formData, streetId, sideofStreet) {
      if (!streetId) {
        console.error('createReminder action called without a streetId!')
        throw new Error('Street ID is required to create a reminder.')
      }
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
          streetId: streetId,
          isRecurring: formData.isRecurring,
          sideofStreet: sideofStreet,
        }
        console.log('Reminder Payload being sent:', JSON.stringify(payload, null, 2))
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
