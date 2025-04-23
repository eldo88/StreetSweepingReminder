<script setup>
import { useStreetsStore } from '@/stores/streets'
import { storeToRefs } from 'pinia'
import { useToast } from 'vue-toastification'
import { computed, ref } from 'vue'
import _ from 'lodash'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

import { ElSelect, ElOption, ElDialog, ElButton } from 'element-plus'
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

const scheduleSchema = toTypedSchema(
  z.object({
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

async function handleStreetChange(streetId, label) {
  loadedScheduleData.value = null
  selectedStreetId.value = streetId
  selectedStreetLabel.value = label || ''
  console.log('handleStreetChange - Street ID set to:', selectedStreetId.value)

  emit('streetSelected', streetId)
  emit('update:isReminderFormVisible', false)

  if (!streetId) {
    isScheduleEntryModalVisible.value = false
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

async function onScheduleEntrySubmit(values) {
  console.log('Modal Form submitted for creation:', values)
  console.log('Target Street ID:', selectedStreetId.value)

  if (!selectedStreetId.value) {
    toast.error('Internal Error: Street ID is missing.')
    isScheduleEntryModalVisible.value = false
    return
  }

  const { weekOfMonth, dayOfWeek, year } = values

  isScheduleLoading.value = true
  try {
    const createScheduleResult = await streetsStore.createSchedule(
      selectedStreetId.value,
      weekOfMonth,
      dayOfWeek,
      year,
    )

    if (createScheduleResult) {
      toast.success('Street Sweeping Schedule created successfully!')
      isScheduleEntryModalVisible.value = false

      const newScheduleData = await streetsStore.getSchedule(selectedStreetId.value)
      console.log('Newly created schedule data:', newScheduleData)
      loadedScheduleData.value = newScheduleData

      emit('scheduleCreated', selectedStreetId.value)
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

function handleModalClose() {
  isScheduleEntryModalVisible.value = false
  modalFormRef.value?.resetForm()
}

const handleStreetSearch = _.debounce(async (query) => {
  loadedScheduleData.value = null
  if (query) {
    streetsStore.searchStreets(query)
  } else {
    streetsStore.clearStreets()
    selectedStreetId.value = null
    selectedStreetLabel.value = ''
    emit('streetSelected', null)
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

        <div class="mb-4">
          <label for="street" class="block text-gray-700 text-sm font-bold mb-2">
            Street Name <span class="text-red-500">*</span>
          </label>
          <el-select
            :model-value="selectedStreetId"
            @update:modelValue="
              (val) => {
                const selectedOption = streetOptions.find((opt) => opt.value === val)
                handleStreetChange(val, selectedOption?.label)
              }
            "
            placeholder="Type to search for a street"
            filterable
            remote
            :remote-method="handleStreetSearch"
            :loading="isSearchLoading"
            clearable
            @clear="handleStreetChange(null, null)"
            class="w-full"
            id="street"
            value-key="value"
          >
            <el-option
              v-for="item in streetOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
            <template #empty>
              <p class="el-select-dropdown__empty">
                {{ isSearchLoading ? 'Loading...' : 'No streets found.' }}
              </p>
            </template>
          </el-select>
        </div>

        <div v-if="isScheduleLoading && selectedStreetId" class="text-center text-gray-500 my-4">
          Loading schedule details...
        </div>

        <div
          v-if="!isScheduleLoading && loadedScheduleData"
          class="mt-6 p-4 border rounded bg-green-50 border-green-200"
        >
          <h3 class="font-semibold text-lg text-gray-800 mb-2">Sweeping Schedule:</h3>
          <p>
            <span class="font-medium">Week:</span>
            {{ getOptionLabel(loadedScheduleData.weekOfMonth, weekOptions) }}
          </p>
          <p>
            <span class="font-medium">Day:</span>
            {{ getOptionLabel(loadedScheduleData.dayOfWeek, dayOptions) }}
          </p>
          <p><span class="font-medium">Year:</span> {{ loadedScheduleData.year }}</p>
        </div>
        <div
          v-if="
            selectedStreetId &&
            !isScheduleLoading &&
            !loadedScheduleData &&
            !isScheduleEntryModalVisible
          "
          class="mt-6 p-4 border rounded bg-yellow-50 border-yellow-200"
        >
          <p class="text-center text-gray-700">
            No schedule found for this street. Opening form to add details...
          </p>
        </div>
      </div>
    </div>

    <el-dialog
      v-model="isScheduleEntryModalVisible"
      :title="`Enter Schedule for ${selectedStreetLabel || 'Selected Street'}`"
      width="90%"
      :max-width="'500px'"
      @closed="handleModalClose"
      :close-on-click-modal="false"
      append-to-body
      top="5vh"
    >
      <p class="text-sm text-gray-600 mb-4">
        No existing schedule was found for this street. Please provide the details below.
      </p>
      <Form ref="modalFormRef" @submit="onScheduleEntrySubmit" :validation-schema="scheduleSchema">
        <div class="mb-4">
          <label for="modalWeekOfMonth" class="block text-gray-700 text-sm font-bold mb-2">
            Week of Month <span class="text-red-500">*</span>
          </label>
          <Field name="weekOfMonth" v-slot="{ field, errors }">
            <el-select
              :model-value="field.value"
              @update:modelValue="field.onChange($event)"
              placeholder="Select Week"
              clearable
              class="w-full"
              id="modalWeekOfMonth"
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
          <label for="modalDayOfWeek" class="block text-gray-700 text-sm font-bold mb-2">
            Day of Week <span class="text-red-500">*</span>
          </label>
          <Field name="dayOfWeek" v-slot="{ field, errors }">
            <el-select
              :model-value="field.value"
              @update:modelValue="field.onChange($event)"
              placeholder="Select Day"
              clearable
              class="w-full"
              id="modalDayOfWeek"
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
          <label for="modalYear" class="block text-gray-700 text-sm font-bold mb-2">
            Year <span class="text-red-500">*</span>
          </label>
          <Field name="year" v-slot="{ field, errors }">
            <el-select
              :model-value="field.value"
              @update:modelValue="field.onChange($event)"
              placeholder="Select Year"
              clearable
              class="w-full"
              id="modalYear"
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

        <div class="flex justify-end gap-2 mt-6">
          <el-button @click="handleModalClose" :disabled="isScheduleLoading">Cancel</el-button>
          <el-button type="primary" native-type="submit" :loading="isScheduleLoading">
            {{ isScheduleLoading ? 'Saving...' : 'Save Schedule' }}
          </el-button>
        </div>
      </Form>
    </el-dialog>
  </section>
</template>

<style scoped>
.el-select-invalid .el-input__wrapper {
  box-shadow: 0 0 0 1px theme('colors.red.500') !important;
}
/* Ensure dialog content is scrollable if it gets too long on small screens */
:deep(.el-dialog__body) {
  max-height: 70vh;
  overflow-y: auto;
}
</style>
