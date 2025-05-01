export function getDayOfWeek(day) {
  const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']
  return days[day] ?? 'Unknown Day'
}

export function getSideOfStreet(side) {
  const sides = ['North', 'South', 'East', 'West']
  return sides[side] ?? 'Unknown Side'
}
