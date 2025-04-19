<script setup>
import { useStreetsStore } from '@/stores/streets'
import { storeToRefs } from 'pinia'
import { useToast } from 'vue-toastification'
import { computed } from 'vue'
import _ from 'lodash'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

import { ElSelect, ElOption } from 'element-plus'
import 'element-plus/dist/index.css'

const schema = toTypedSchema(
  z.object({
    street: z
      .number({
        required_error: 'Street is required',
        invalid_type_error: 'Street must be selected',
      })
      .positive('Street must be selected'),
  }),
)

const toast = useToast()
const streetsStore = useStreetsStore()

const { streets, isLoading } = storeToRefs(streetsStore)

const streetOptions = computed(() => {
  return streets.value.map((street) => ({
    value: street.id,
    label: street.streetName + `, ${street.zipCode}`,
  }))
})

async function onSubmit(values) {
  try {
    const streetId = values.street
    const getSchedule = await streetsStore.getSchedule(streetId)
    if (getSchedule) {
      toast.success('Street Sweeping Schedule retrieved successfully!')
    } else {
      const createSchedule = await streetsStore.createSchedule(streetId, values.streetSweepingDate)
      if (createSchedule) {
        toast.success('Street Sweeping Schedule created successfully!')
      } else {
        toast.error('Failed to create Street Sweeping Schedule')
      }
    }
  } catch (error) {
    console.error('Street Sweeping Schedule creation failed:', error)
    toast.error('Failed to create Street Sweeping Schedule')
  }
}

const handleStreetSearch = _.debounce(async (query) => {
  if (query) {
    streetsStore.searchStreets(query)
  } else {
    streetsStore.clearStreets()
  }
}, 300)
</script>

<template>
  <section class="bg-blue-50 py-10">
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <div class="bg-white shadow-md rounded-lg px-4 sm:px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Search for your street</h2>
        <Form @submit="onSubmit" :validation-schema="schema">
          <!-- Street (Searchable Dropdown using Element Plus) -->
          <div class="mb-4">
            <label for="street" class="block text-gray-700 text-sm font-bold mb-2">
              Street Name <span class="text-red-500">*</span>
            </label>
            <!-- Use VeeValidate Field component for validation -->
            <Field name="street" v-slot="{ field, value, errors }">
              <el-select
                v-bind="field"
                :model-value="value"
                @update:modelValue="field.onChange($event)"
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
            <!-- Display validation error -->
            <ErrorMessage name="street" class="text-red-500 text-xs mt-1" />
          </div>
          <!-- End Street -->
        </Form>
      </div>
    </div>
  </section>
</template>
