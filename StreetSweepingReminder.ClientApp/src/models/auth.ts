/*
 * Auth related types *
 */

export interface RegistrationData {
  email: string
  userName: string
  password: string
}

export interface RegistrationResponse {
  userId: string
  token: string
  statusCode: number
}

export interface ApiErrorResponse {
  statusCode: number
  message: string
  errors?: { [key: string]: string[] }
}
