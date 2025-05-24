import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import RegisterForm from '../RegisterForm.vue'
import { createTestingPinia } from '@pinia/testing'
import { Form, Field, ErrorMessage } from 'vee-validate'

describe('RegisterForm.vue', () => {
  const createWrapper = () => {
    return mount(RegisterForm, {
      global: {
        plugins: [createTestingPinia({ createSpy: vi.fn })],
        components: {
          Form,
          Field,
          ErrorMessage,
        },
      },
    })
  }

  it('validates required fields', async () => {
    const wrapper = createWrapper()

    // Simulate form submission
    await wrapper.find('form').trigger('submit')

    // Check for validation errors
    expect(wrapper.text()).toContain('Email not in valid format')
    expect(wrapper.text()).toContain('Password must be 5 characters')
  })

  it('submits form with valid data', async () => {
    const wrapper = createWrapper()

    // Fill in form fields
    await wrapper.find('input[name="email"]').setValue('test@example.com')
    await wrapper.find('input[name="password"]').setValue('password123')

    // Submit form
    await wrapper.find('form').trigger('submit')

    // Get store instance
    const store = wrapper.vm.$store

    // Verify store action was called
    expect(store.register).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123',
    })
  })
})
