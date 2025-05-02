import { useRemindersStore } from '@/stores/reminder'
import { useStreetsStore } from '@/stores/streets'
import { getSideOfStreet } from '@/utils/streetUtils'

export const generateReminderAttributes = (reminder) => {
  const attributes = []

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
  }

  return attributes
}

export const generateStreetSweepingAttributes = async (reminder, streetsStore) => {
  const attributes = []

  if (
    reminder.streetSweepingSchedule?.schedule &&
    Array.isArray(reminder.streetSweepingSchedule.schedule)
  ) {
    for (const sweepingSchedule of reminder.streetSweepingSchedule.schedule) {
      if (sweepingSchedule.streetSweepingDate) {
        try {
          const eventDate = new Date(sweepingSchedule.streetSweepingDate)
          const streetData = await streetsStore.getStreet(sweepingSchedule.streetId)
          const streetName = streetData ? streetData.streetName : 'Unknown Street'
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

  return attributes
}

export const generateCalendarAttributes = async () => {
  const remindersStore = useRemindersStore()
  const streetsStore = useStreetsStore()

  if (!remindersStore.reminders || remindersStore.reminders.length === 0) {
    if (!remindersStore.isLoading) {
      console.log('generateCalendarAttributes: No reminders found in store.')
    }
    return []
  }

  const allAttributes = []
  const promises = []

  for (const reminder of remindersStore.reminders) {
    allAttributes.push(...generateReminderAttributes(reminder))
    promises.push(generateStreetSweepingAttributes(reminder, streetsStore))
  }

  const sweepingResults = await Promise.all(promises)

  sweepingResults.forEach((sweepingAttrs) => {
    allAttributes.push(...sweepingAttrs)
  })
  return allAttributes
}
