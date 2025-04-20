import { ref } from 'vue'

const timeoutId = ref(null)

export function setGlobalTimeout(minutes) {
  if (timeoutId.value) {
    clearTimeout(timeoutId.value)
  }

  timeoutId.value = setTimeout(
    () => {
      const event = new Event('unauthorized')
      window.dispatchEvent(event)
    },
    minutes * 60 * 1000,
  )
}
