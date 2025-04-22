export const formatDate = (dateString) => {
  if (!dateString) return 'Invalid Date'
  try {
    const date = new Date(dateString)
    if (isNaN(date.getTime())) {
      return 'Invalid Date'
    }

    return date.toLocaleDateString(undefined, {
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  } catch (error) {
    console.error('Error formatting date:', dateString, error)
    return 'Invalid Date'
  }
}
