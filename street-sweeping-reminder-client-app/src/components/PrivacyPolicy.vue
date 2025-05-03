<script setup>
const lastUpdated = '2024-05-02'
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 py-8 dark:text-gray-200">
    <p class="text-sm text-gray-500 dark:text-gray-400 mb-4">Last Updated: {{ lastUpdated }}</p>
    <h1 class="text-3xl font-bold mb-6 dark:text-gray-200">Privacy Policy</h1>

    <div class="prose prose-lg dark:prose-invert">
      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Introduction</h2>
        <p class="mb-4">
          This Privacy Policy explains how Denver Street Sweeping Reminder ("we", "us", or "our")
          collects, uses, and protects your personal information.
        </p>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Information We Collect</h2>
        <ul class="list-disc pl-6 mb-4">
          <li>Email address</li>
          <li>Phone number (for reminder notifications)</li>
          <li>Street address information</li>
          <li>Reminder preferences and settings</li>
        </ul>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">How We Use Your Information</h2>
        <p class="mb-4">We use the collected information to:</p>
        <ul class="list-disc pl-6 mb-4">
          <li>Send street sweeping reminders</li>
          <li>Manage your account</li>
          <li>Improve our services</li>
          <li>Communicate important updates</li>
        </ul>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Data Protection</h2>
        <p class="mb-4">
          We implement appropriate security measures to protect your personal information from
          unauthorized access, alteration, or disclosure.
        </p>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Contact Us</h2>
        <p class="mb-4">
          If you have questions about this Privacy Policy, please contact us at:
          <a
            href="mailto:privacy@yourapp.com"
            class="text-blue-600 hover:underline dark:text-blue-400 dark:hover:text-blue-300"
          >
            privacy@yourapp.com
          </a>
        </p>
      </section>
    </div>
  </div>
</template>
