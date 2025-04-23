<script setup>
import { useStreetsStore } from '@/stores/streets'
import { storeToRefs } from 'pinia'
import { useToast } from 'vue-toastification'
import { computed, ref } from 'vue'
import _ from 'lodash'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

import { ElSelect, ElOption } from 'element-plus'
import 'element-plus/dist/index.css'

defineProps({
  isReminderFormVisible: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['update:isReminderFormVisible', 'streetSelected', 'scheduleCreated'])

const toast = useToast()
const streetsStore = useStreetsStore()

const { streets, isLoading: isSearchLoading } = storeToRefs(streetsStore)

const formRef = ref(null)
const modalFormRef = ref(null)

const isScheduleLoading = ref(false)

const isScheduleEntryModalVisible = ref(false)

const selectedStreetId = ref(null)
const selectedStreetLabel = ref('')
const loadedScheduleData = ref(null)

const streetOptions = computed(() => {
  return streets.value.map((street) => ({
    value: street.id,
    label: street.streetName + `, ${street.zipCode}`,
  }))
})

const weekOptions = ref([
  { value: 1, label: '1st Week' },
  { value: 2, label: '2nd Week' },
  { value: 3, label: '3rd Week' },
  { value: 4, label: '4th Week' },
])

const dayOptions = ref([
  { value: 1, label: 'Monday' },
  { value: 2, label: 'Tuesday' },
  { value: 3, label: 'Wednesday' },
  { value: 4, label: 'Thursday' },
  { value: 5, label: 'Friday' },
])

const currentYear = new Date().getFullYear()
const yearOptions = ref(
  Array.from({ length: 6 }, (_, i) => ({
    value: currentYear + i,
    label: String(currentYear + i),
  })),
)

const schema = toTypedSchema(
  z.object({
    street: z
      .number({
        required_error: 'Street is required',
        invalid_type_error: 'Street must be selected',
      })
      .positive('Street must be selected'),
    weekOfMonth: z
      .number({
        required_error: 'Week of month is required',
        invalid_type_error: 'Week of month must be selected',
      })
      .int()
      .min(1, 'Week must be selected')
      .max(4),
    dayOfWeek: z
      .number({
        required_error: 'Day of week is required',
        invalid_type_error: 'Day of week must be selected',
      })
      .int()
      .min(1, 'Day must be selected')
      .max(5),
    year: z
      .number({
        required_error: 'Year is required',
        invalid_type_error: 'Year must be selected',
      })
      .int()
      .min(currentYear, 'Year must be current or future'),
  }),
)

async function handleStreetChange(streetId) {
  formRef.value?.resetField('weekOfMonth')
  formRef.value?.resetField('dayOfWeek')
  formRef.value?.resetField('year')

  emit('streetSelected', streetId)

  if (!streetId) {
    return
  }

  isScheduleLoading.value = true
  try {
    console.log(`Fetching schedule for street ID: ${streetId}`)
    const scheduleData = await streetsStore.getSchedule(streetId)

    console.log('scheduleData received:', scheduleData)
    if (scheduleData && scheduleData.weekOfMonth && scheduleData.dayOfWeek && scheduleData.year) {
      formRef.value?.setFieldValue('weekOfMonth', scheduleData.weekOfMonth)
      formRef.value?.setFieldValue('dayOfWeek', scheduleData.dayOfWeek)
      formRef.value?.setFieldValue('year', scheduleData.year)
      toast.info('Existing schedule data loaded.')
      // emit('scheduleLoaded', true);
      emit('update:isReminderFormVisible', true)
      isScheduleEntryModalVisible.value = false
    } else {
      console.log('No existing schedule found for this street.')

      toast.info('No schedule found for this street. Please enter details.')

      isScheduleEntryModalVisible.value = true
      modalFormRef.value?.resetForm()
    }
  } catch (error) {
    console.error('Failed to fetch street schedule:', error)
    toast.error('Could not fetch schedule details.')

    formRef.value?.resetField('weekOfMonth')
    formRef.value?.resetField('dayOfWeek')
    formRef.value?.resetField('year')
  } finally {
    isScheduleLoading.value = false
  }
}

async function onSubmit(values) {
  console.log('Form submitted for creation:', values)

  const { street: streetId, weekOfMonth, dayOfWeek, year } = values

  if (
    streetId === undefined ||
    weekOfMonth === undefined ||
    dayOfWeek === undefined ||
    year === undefined
  ) {
    toast.error('Form data is incomplete. Please check all fields.')
    return
  }

  isScheduleLoading.value = true
  try {
    const createScheduleResult = await streetsStore.createSchedule(
      streetId,
      weekOfMonth,
      dayOfWeek,
      year,
    )

    if (createScheduleResult) {
      toast.success('Street Sweeping Schedule created successfully!')
      emit('update:isReminderFormVisible', true)
    } else {
      toast.error('Failed to create Street Sweeping Schedule (API reported failure).')
    }
  } catch (error) {
    console.error('Street Sweeping Schedule creation failed:', error)
    const errorMessage =
      error?.response?.data?.message || 'Failed to create Street Sweeping Schedule'
    toast.error(errorMessage)
  } finally {
    isScheduleLoading.value = false
  }
}

// Handler for submitting the schedule form FROM THE MODAL
async function onScheduleEntrySubmit(values) {
  console.log('Modal Form submitted for creation:', values)
  console.log('Target Street ID:', selectedStreetId.value)

  if (!selectedStreetId.value) {
    toast.error('Internal Error: Street ID is missing.')
    return
  }

  const { weekOfMonth, dayOfWeek, year } = values

  isScheduleLoading.value = true
  try {
    // Use the existing createSchedule store action
    const createScheduleResult = await streetsStore.createSchedule(
      selectedStreetId.value,
      weekOfMonth,
      dayOfWeek,
      year,
    )

    if (createScheduleResult) {
      toast.success('Street Sweeping Schedule created successfully!')
      isScheduleEntryModalVisible.value = false // Close modal

      // Fetch the newly created schedule to display it
      const newScheduleData = await streetsStore.getSchedule(selectedStreetId.value)
      loadedScheduleData.value = newScheduleData

      // Emit events: one for general creation, one to update parent visibility
      emit('scheduleCreated', selectedStreetId.value) // Let parent know schedule exists now
      emit('update:isReminderFormVisible', true) // Tell parent to show the ReminderForm
    } else {
      // This else might not be reachable if createSchedule throws on failure
      toast.error('Failed to create Street Sweeping Schedule (API reported failure).')
    }
  } catch (error) {
    console.error('Street Sweeping Schedule creation failed:', error)
    const errorMessage =
      error?.response?.data?.message || 'Failed to create Street Sweeping Schedule'
    toast.error(errorMessage)
    // Keep modal open on error? Or close? User decision.
    // isScheduleEntryModalVisible.value = false;
  } finally {
    isScheduleLoading.value = false
  }
}

function handleModalClose() {
  isScheduleEntryModalVisible.value = false
  modalFormRef.value?.resetForm()
}

const handleStreetSearch = _.debounce(async (query) => {
  if (query) {
    streetsStore.searchStreets(query)
  } else {
    streetsStore.clearStreets()
  }
}, 300)

const getOptionLabel = (value, options) =>
  options.find((opt) => opt.value === value)?.label || 'N/A'
</script>

<template>
  <section class="bg-blue-50 py-10">
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <div class="bg-white shadow-md rounded-lg px-4 sm:px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Search for your street</h2>
        <Form ref="formRef" @submit="onSubmit" :validation-schema="schema">
          <div class="mb-4">
            <label for="street" class="block text-gray-700 text-sm font-bold mb-2">
              Street Name <span class="text-red-500">*</span>
            </label>
            <Field name="street" v-slot="{ field, value, errors }">
              <el-select
                v-bind="field"
                :model-value="value"
                @update:modelValue="
                  (newValue) => {
                    field.onChange(newValue)
                    handleStreetChange(newValue)
                  }
                "
                placeholder="Select or search for a street"
                filterable
                remote
                :remote-method="handleStreetSearch"
                :loading="isLoading"
                clearable
                class="w-full"
                id="street"
                :class="{ 'el-select-invalid': errors?.street }"
              >
                <el-option
                  v-for="item in streetOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </Field>
            <ErrorMessage name="street" class="text-red-500 text-xs mt-1" />
          </div>

          <div class="mb-4">
            <label for="weekOfMonth" class="block text-gray-700 text-sm font-bold mb-2">
              Week of Month <span class="text-red-500">*</span>
            </label>
            <Field name="weekOfMonth" v-slot="{ field, value, errors }">
              <el-select
                :model-value="value"
                @update:modelValue="field.onChange($event)"
                placeholder="Select Week"
                clearable
                :disabled="isScheduleLoading"
                class="w-full"
                id="weekOfMonth"
                :class="{ 'el-select-invalid': errors?.length }"
              >
                <el-option
                  v-for="item in weekOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </Field>
            <ErrorMessage name="weekOfMonth" class="text-red-500 text-xs mt-1" />
          </div>

          <div class="mb-4">
            <label for="dayOfWeek" class="block text-gray-700 text-sm font-bold mb-2">
              Day of Week <span class="text-red-500">*</span>
            </label>
            <Field name="dayOfWeek" v-slot="{ field, value, errors }">
              <el-select
                :model-value="value"
                @update:modelValue="field.onChange($event)"
                placeholder="Select Day"
                clearable
                :disabled="isScheduleLoading"
                class="w-full"
                id="dayOfWeek"
                :class="{ 'el-select-invalid': errors?.length }"
              >
                <el-option
                  v-for="item in dayOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </Field>
            <ErrorMessage name="dayOfWeek" class="text-red-500 text-xs mt-1" />
          </div>

          <div class="mb-4">
            <label for="year" class="block text-gray-700 text-sm font-bold mb-2">
              Year <span class="text-red-500">*</span>
            </label>
            <Field name="year" v-slot="{ field, value, errors }">
              <el-select
                :model-value="value"
                @update:modelValue="field.onChange($event)"
                placeholder="Select Year"
                clearable
                :disabled="isScheduleLoading"
                class="w-full"
                id="year"
                :class="{ 'el-select-invalid': errors?.length }"
              >
                <el-option
                  v-for="item in yearOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </Field>
            <ErrorMessage name="year" class="text-red-500 text-xs mt-1" />
          </div>

          <div class="flex flex-col gap-3">
            <button
              type="submit"
              class="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
              :disabled="isScheduleLoading"
            >
              {{ isScheduleLoading ? 'Processing...' : 'Create Schedule' }}
            </button>
          </div>
        </Form>
      </div>
    </div>
  </section>
</template>
