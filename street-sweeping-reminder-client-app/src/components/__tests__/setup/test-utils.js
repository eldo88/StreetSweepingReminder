import { config } from '@vue/test-utils'
import { vi } from 'vitest'

// Mock Element Plus components
config.global.stubs = {
  'el-select': {
    template: '<div><slot></slot></div>',
    props: ['modelValue'],
    emits: ['update:modelValue'],
  },
  'el-option': {
    template: '<div><slot></slot></div>',
    props: ['value', 'label'],
  },
  'el-dialog': {
    template: '<div><slot></slot></div>',
    props: ['modelValue'],
    emits: ['update:modelValue'],
  },
  'el-button': {
    template: '<button><slot></slot></button>',
  },
  'el-radio-group': {
    template: '<div><slot></slot></div>',
    props: ['modelValue'],
    emits: ['update:modelValue'],
  },
  'el-radio': {
    template: '<div><slot></slot></div>',
    props: ['label'],
  },
  'el-table': {
    template: '<div><slot></slot></div>',
    props: ['data'],
  },
  'el-table-column': {
    template: '<div><slot></slot></div>',
    props: ['prop', 'label'],
  },
}

// Mock vue-router
config.global.mocks = {
  $router: {
    push: vi.fn(),
  },
}
