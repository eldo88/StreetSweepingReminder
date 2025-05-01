import { useRemindersStore } from '@/stores/reminder'
import { useStreetsStore } from '@/stores/streets'
import { reactive } from 'vue'
import { getSideOfStreet } from '@/utils/streetUtils'

const streetNames = reactive(new Map())

const getStreetName = async (streetId) => {
  const streetsStore = useStreetsStore()
  if (streetNames.has(streetId)) {
    return streetNames.get(streetId)
  }

  try {
    const street = await streetsStore.getStreet(streetId)
    if (street) {
      streetNames.set(street.id, street.streetName)
      return street.name
    } else {
      console.error('Street not found for ID:', streetId)
      return 'Unknown Street'
    }
  } catch (error) {
    console.error('Error fetching street name:', error)
    return 'Error Fetching Street'
  }
}

export const generateCalendarAttributes = async () => {
  const remindersStore = useRemindersStore()
  if (!remindersStore.reminders || remindersStore.reminders.length === 0) {
    console.log('No reminders found.')
    return
  }

  const attributes = []

  for (const reminder of remindersStore.reminders) {
    if (reminder.reminderSchedule?.schedule && Array.isArray(reminder.reminderSchedule.schedule)) {
      for (const scheduleItem of reminder.reminderSchedule.schedule) {
        if (scheduleItem.isActive && scheduleItem.nextNotificationDate) {
          try {
            const eventDate = new Date(scheduleItem.nextNotificationDate)
            const formattedTime = eventDate.toLocaleTimeString([], {
              hour: '2-digit',
              minute: '2-digit',
            })

            attributes.push({
              key: `schedule-${scheduleItem.id}`,
              highlight: {
                color: 'blue',
                fillMode: 'light',
              },
              popover: {
                label: `${reminder.title} at ${formattedTime}`,
                visibility: 'hover',
              },
              dates: eventDate,
            })
          } catch (e) {
            console.error(
              `Invalid date format for schedule item ${scheduleItem.id}:`,
              scheduleItem.scheduledDateTimeUtc,
              e,
            )
          }
        }
      }

      for (const sweepingSchedule of reminder.streetSweepingSchedule.schedule) {
        if (sweepingSchedule.streetSweepingDate) {
          try {
            const eventDate = new Date(sweepingSchedule.streetSweepingDate)
            const streetName = await getStreetName(reminder.streetId)
            const sideOfStreet = getSideOfStreet(sweepingSchedule.sideOfStreet)

            attributes.push({
              key: `sweeping-${sweepingSchedule.id}`,
              highlight: {
                color: 'red',
                fillMode: 'light',
              },
              popover: {
                label: `Street Sweeping: ${streetName}, ${sideOfStreet} side`,
                visibility: 'hover',
              },
              dates: eventDate,
            })
          } catch (e) {
            console.error(
              `Invalid date format for street sweeping item ${sweepingSchedule.id}:`,
              sweepingSchedule.scheduledDateTimeUtc,
              e,
            )
          }
        }
      }
    }
  }

  return attributes
}
