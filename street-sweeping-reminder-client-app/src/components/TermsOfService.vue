<script setup>
const lastUpdated = '2024-05-02'
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 py-8">
    <p class="text-sm text-gray-500 mb-4">Last Updated: {{ lastUpdated }}</p>
    <h1 class="text-3xl font-bold mb-6">Terms of Service</h1>

    <div class="prose prose-lg">
      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Agreement to Terms</h2>
        <p class="mb-4">
          By accessing and using Denver Street Sweeping Reminder, you agree to these Terms of
          Service and our Privacy Policy.
        </p>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Notification Services</h2>
        <p class="mb-4">
          Our service provides reminders about street sweeping schedules via text messages and other
          notification methods. By using our service, you understand and agree that:
        </p>
        <ul class="list-disc pl-6 mb-4">
          <li>You can opt out of notifications at any time through your account settings</li>
          <li>Standard message and data rates may apply for SMS notifications</li>
          <li>We will send notifications only for the reminders you explicitly set up</li>
          <li>
            Notification timing and delivery cannot be guaranteed due to factors outside our control
          </li>
        </ul>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Data Usage and Privacy</h2>
        <p class="mb-4">We are committed to protecting your privacy. As such:</p>
        <ul class="list-disc pl-6 mb-4">
          <li>Your data will never be sold or distributed to third parties</li>
          <li>Your information is used solely for providing the reminder service you requested</li>
          <li>You can request deletion of your data at any time</li>
          <li>We only collect information necessary for the functioning of the service</li>
        </ul>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">User Rights and Control</h2>
        <p class="mb-4">As a user, you have the right to:</p>
        <ul class="list-disc pl-6 mb-4">
          <li>Modify or delete your account at any time</li>
          <li>Opt out of any or all notifications</li>
          <li>Request a copy of your data</li>
          <li>Update your contact information and preferences</li>
        </ul>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Disclaimer</h2>
        <p class="mb-4">
          While we strive to provide accurate street sweeping schedule information, we cannot
          guarantee absolute accuracy. Users should always verify schedules with official city
          resources.
        </p>
      </section>

      <section class="mb-8">
        <h2 class="text-2xl font-semibold mb-4">Contact</h2>
        <p class="mb-4">
          For questions about these terms, please contact us at:
          <a href="mailto:support@yourapp.com" class="text-blue-600 hover:underline">
            support@yourapp.com
          </a>
        </p>
      </section>
    </div>
  </div>
</template>
