<script setup>
import { useStreetsStore } from '@/stores/streets'
import { storeToRefs } from 'pinia'
import { useToast } from 'vue-toastification'
import { computed, ref, nextTick } from 'vue'
import _ from 'lodash'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

import {
  ElSelect,
  ElOption,
  ElDialog,
  ElButton,
  ElRadioGroup,
  ElRadio,
  ElTable,
  ElTableColumn,
} from 'element-plus'
import 'element-plus/dist/index.css'

defineProps({
  isReminderFormVisible: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits([
  'update:isReminderFormVisible',
  'streetSelected',
  'scheduleCreated',
  'sideSelected',
])

const toast = useToast()
const streetsStore = useStreetsStore()

const { streets, isLoading: isSearchLoading } = storeToRefs(streetsStore)

const modalFormRef = ref(null)

const isScheduleLoading = ref(false)

const isScheduleEntryModalVisible = ref(false)
const isScheduleSelectionModalVisible = ref(false)

const selectedStreetId = ref(null)
const selectedStreetLabel = ref('')
const loadedSchedules = ref([])
const finalSelectedSchedule = ref(null)
const modalSelectedScheduleIndex = ref(null)

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

const sideOfStreetOptions = ref([
  { value: 0, label: 'North' },
  { value: 1, label: 'South' },
  { value: 2, label: 'East' },
  { value: 3, label: 'West' },
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
      .max(5),
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
    sideOfStreet: z
      .number({
        required_error: 'Side is required',
        invalid_type_error: 'Side must be selected',
      })
      .int()
      .min(0, 'year must be selected')
      .max(3),
  }),
)

const getOptionLabel = (value, options) => {
  // Add a check to ensure options is actually an array before calling find
  if (!Array.isArray(options)) {
    console.warn('getOptionLabel received non-array options:', options)
    return 'Invalid Options'
  }
  return options.find((opt) => opt.value === value)?.label || 'N/A'
}

const formatScheduleForDisplay = (schedule) => {
  if (!schedule) return 'N/A'
  // Use .value when accessing refs from script
  const weekLabel = getOptionLabel(schedule.weekOfMonth, weekOptions.value)
  const dayLabel = getOptionLabel(schedule.dayOfWeek, dayOptions.value)
  return `Wk: ${weekLabel}, Day: ${dayLabel}, Yr: ${schedule.year}`
}

const formatSideOfStreetForDisplay = (schedule) => {
  if (!schedule) return 'N/A'
  const sideOfStreetlabel = getOptionLabel(schedule.sideOfStreet, sideOfStreetOptions.value)
  return `Side: ${sideOfStreetlabel}`
}

async function handleStreetChange(streetId, label) {
  loadedSchedules.value = []
  finalSelectedSchedule.value = null
  selectedStreetId.value = streetId
  selectedStreetLabel.value = label || ''
  modalSelectedScheduleIndex.value = null

  console.log('handleStreetChange - Street ID set to:', selectedStreetId.value)

  emit('streetSelected', streetId)
  emit('update:isReminderFormVisible', false)

  isScheduleEntryModalVisible.value = false
  isScheduleSelectionModalVisible.value = false

  if (!streetId) {
    isScheduleEntryModalVisible.value = false
    return
  }

  isScheduleLoading.value = true
  try {
    console.log(`Fetching schedule for street ID: ${streetId}`)
    const scheduleData = await streetsStore.getSchedule(streetId)

    console.log('scheduleData received:', scheduleData)
    if (Array.isArray(scheduleData) && scheduleData.length > 0) {
      console.log(`${scheduleData.length} schedule pattern(s) found.`)
      loadedSchedules.value = scheduleData
      finalSelectedSchedule.value = null
      toast.info('Schedule(s) found. Please select one.')
      emit('update:isReminderFormVisible', false)
      isScheduleSelectionModalVisible.value = true
      modalSelectedScheduleIndex.value = 0
    } else {
      console.log('No existing schedule patterns found for this street.')
      loadedSchedules.value = []
      finalSelectedSchedule.value = null
      toast.info('No schedule found. Please enter the details.')
      emit('update:isReminderFormVisible', false)
      isScheduleEntryModalVisible.value = true
      await nextTick()
    }
  } catch (error) {
    console.error('Failed to fetch street schedule patterns:', error)
    toast.error('Could not fetch schedule details.')
    loadedSchedules.value = []
    finalSelectedSchedule.value = null
    isScheduleEntryModalVisible.value = false
    isScheduleSelectionModalVisible.value = false
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

  const { weekOfMonth, dayOfWeek, year, sideOfStreet } = values

  isScheduleLoading.value = true
  try {
    const createScheduleResult = await streetsStore.createSchedule(
      selectedStreetId.value,
      weekOfMonth,
      dayOfWeek,
      year,
      sideOfStreet,
    )

    if (createScheduleResult) {
      toast.success('Schedule pattern created successfully!')
      isScheduleEntryModalVisible.value = false

      console.log('Refreshing schedule patterns after creation...')
      const newScheduleData = await streetsStore.getSchedule(selectedStreetId.value)
      if (Array.isArray(newScheduleData) && newScheduleData.length > 0) {
        loadedSchedules.value = newScheduleData
        finalSelectedSchedule.value = null
        isScheduleSelectionModalVisible.value = true
        modalSelectedScheduleIndex.value = 0
        emit('update:isReminderFormVisible', false)
        toast.info('Schedule created. Please select from available schedules.')
        emit('scheduleCreated', selectedStreetId.value)
      } else {
        toast.error('Could not retrieve schedules after creation. Please search again.')
        loadedSchedules.value = []
        finalSelectedSchedule.value = null
        emit('update:isReminderFormVisible', false)
      }
    } else {
      toast.error('Failed to create Schedule Pattern (API reported failure).')
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

function handleScheduleSelectionConfirm() {
  if (modalSelectedScheduleIndex.value === null || modalSelectedScheduleIndex.value === undefined) {
    toast.warn('Please select a schedule.')
    return
  }

  const selectedIndex = modalSelectedScheduleIndex.value
  if (loadedSchedules.value && loadedSchedules.value[selectedIndex]) {
    finalSelectedSchedule.value = loadedSchedules.value[selectedIndex]
    console.log('Schedule selected by user:', finalSelectedSchedule.value)
    isScheduleSelectionModalVisible.value = false
    emit('update:isReminderFormVisible', true)
    emit('sideSelected', finalSelectedSchedule.value.sideOfStreet)
    toast.success('Schedule selected.')
  } else {
    console.error('Error confirming selection: Invalid index or loadedSchedules array.')
    toast.error('Could not select schedule. Please try again.')
    isScheduleSelectionModalVisible.value = false
  }
}

function handleSelectionModalClose() {
  isScheduleSelectionModalVisible.value = false
  if (!finalSelectedSchedule.value) {
    modalSelectedScheduleIndex.value = null
  }
}

function handleModalClose() {
  isScheduleEntryModalVisible.value = false
  modalFormRef.value?.resetForm()
}

function handleNewSchedule() {
  isScheduleSelectionModalVisible.value = false
  isScheduleEntryModalVisible.value = true
  modalFormRef.value?.resetForm()
}

const handleStreetSearch = _.debounce(async (query) => {
  loadedSchedules.value = []
  finalSelectedSchedule.value = null
  emit('update:isReminderFormVisible', false)
  isScheduleEntryModalVisible.value = false
  isScheduleSelectionModalVisible.value = false
  if (query) {
    streetsStore.searchStreets(query)
  } else {
    streetsStore.clearStreets()
    selectedStreetId.value = null
    selectedStreetLabel.value = ''
    emit('streetSelected', null)
  }
}, 300)
</script>

<template>
  <section class="bg-blue-50 py-10 dark:bg-gray-900">
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <div class="bg-white shadow-md rounded-lg px-4 sm:px-8 pt-6 pb-8 mb-4 dark:bg-gray-800">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6 dark:text-gray-200">
          Search for your street
        </h2>

        <div class="mb-4">
          <label for="street" class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200">
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
          v-if="!isScheduleLoading && finalSelectedSchedule"
          class="mt-6 p-4 border rounded bg-green-50 border-green-200"
        >
          <h3 class="font-semibold text-lg text-gray-800 mb-2">Sweeping Schedule:</h3>
          <p>
            <span class="font-medium">Week:</span>
            {{ getOptionLabel(finalSelectedSchedule.weekOfMonth, weekOptions) }}
          </p>
          <p>
            <span class="font-medium">Day:</span>
            {{ getOptionLabel(finalSelectedSchedule.dayOfWeek, dayOptions) }}
          </p>
          <p><span class="font-medium">Year:</span> {{ finalSelectedSchedule.year }}</p>
        </div>
        <div
          v-if="selectedStreetId && !isScheduleLoading && isScheduleSelectionModalVisible"
          class="mt-6 p-4 border rounded bg-blue-50 border-blue-200"
        >
          <p class="text-center text-gray-700 dark:text-gray-200">
            Multiple schedules found. Please make a selection in the dialog.
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
      class="dark:bg-gray-800"
    >
      <p class="text-sm text-gray-600 mb-4 dark:text-gray-200">
        No existing schedule was found for this street. Please provide the details below.
      </p>
      <Form ref="modalFormRef" @submit="onScheduleEntrySubmit" :validation-schema="scheduleSchema">
        <div class="mb-4">
          <label
            for="modalWeekOfMonth"
            class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
          >
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
          <label
            for="modalDayOfWeek"
            class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
          >
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
          <label
            for="modalYear"
            class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
          >
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

        <div class="mb-4">
          <label
            for="modalSide"
            class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
          >
            Side <span class="text-red-500">*</span>
          </label>
          <Field name="sideOfStreet" v-slot="{ field, errors }">
            <el-select
              :model-value="field.value"
              @update:modelValue="field.onChange($event)"
              placeholder="Select Side"
              clearable
              class="w-full"
              id="modalSide"
              :class="{ 'el-select-invalid': errors?.length }"
            >
              <el-option
                v-for="item in sideOfStreetOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </Field>
          <ErrorMessage name="sideOfStreet" class="text-red-500 text-xs mt-1" />
        </div>

        <div class="flex justify-end gap-2 mt-6">
          <el-button @click="handleModalClose" :disabled="isScheduleLoading">Cancel</el-button>
          <el-button
            type="primary"
            native-type="submit"
            :loading="isScheduleLoading"
            class="dark:bg-purple-900"
          >
            {{ isScheduleLoading ? 'Saving...' : 'Save Schedule' }}
          </el-button>
        </div>
      </Form>
    </el-dialog>

    <el-dialog
      v-model="isScheduleSelectionModalVisible"
      :title="`Select Schedule for ${selectedStreetLabel || 'Selected Street'}`"
      width="90%"
      :max-width="'600px'"
      @closed="handleSelectionModalClose"
      :close-on-click-modal="false"
      append-to-body
      top="5vh"
      class="dark:bg-gray-800"
    >
      <p class="text-sm text-gray-600 mb-4">
        Multiple schedules found for this street. Please select the one you want to use:
      </p>

      <el-radio-group v-model="modalSelectedScheduleIndex" class="w-full">
        <el-table :data="loadedSchedules" border-style="width: 100%" highlight-current-row>
          <el-table-column width="55" text-align="center">
            <template #default="scope">
              <el-radio
                :value="scope.$index"
                size="large"
                @click.prevent="modalSelectedScheduleIndex = scope.$index"
                ><i></i
              ></el-radio>
            </template>
          </el-table-column>
          <el-table-column prop="side" label="Side/Details">
            <template #default="scope">
              {{ formatSideOfStreetForDisplay(scope.row) || `Side ${scope.$index + 1}` }}
            </template>
          </el-table-column>
          <el-table-column label="Schedule">
            <template #default="scope">
              {{ formatScheduleForDisplay(scope.row) }}
            </template>
          </el-table-column>
        </el-table>
      </el-radio-group>

      <div class="flex justify-end gap-2 mt-6">
        <el-button @click="handleNewSchedule">Add New</el-button>
        <el-button @click="handleSelectionModalClose">Cancel</el-button>
        <el-button
          type="primary"
          @click="handleScheduleSelectionConfirm"
          :disabled="modalSelectedScheduleIndex === null"
          class="dark:bg-purple-900"
        >
          Confirm Selection
        </el-button>
      </div>
    </el-dialog>
  </section>
</template>

<style scoped>
.el-select-invalid .el-input__wrapper {
  box-shadow: 0 0 0 1px theme('colors.red.500') !important;
}

:deep(.el-dialog__body) {
  max-height: 70vh;
  overflow-y: auto;
}

:deep(.el-table .el-radio__label) {
  display: none;
}

:deep(.el-dialog__body) {
  padding-bottom: 10px;
}

:deep(.el-dialog) {
  --el-bg-color: #1f2937 !important;
  --el-text-color-primary: #e5e7eb !important;
}

:deep(.el-table) {
  --el-bg-color: #1f2937 !important;
  --el-bg-color-overlay: #374151 !important;
  --el-text-color-primary: #e5e7eb !important;
  --el-border-color: #4b5563 !important;
}

:deep(.el-table__header) {
  background-color: #374151 !important;
}

:deep(.el-table__cell) {
  background-color: #1f2937 !important;
  color: #e5e7eb !important;
}

:deep(.el-table__row:hover > td.el-table__cell) {
  background-color: #374151 !important;
}

:deep(.el-radio__input.is-checked .el-radio__inner) {
  background-color: #10b981 !important;
  border-color: #10b981 !important;
}

:deep(.el-dialog__title) {
  color: #e5e7eb !important;
}

:deep(.el-table--border),
:deep(.el-table--border th),
:deep(.el-table--border td) {
  border-color: #4b5563 !important;
}
</style>
