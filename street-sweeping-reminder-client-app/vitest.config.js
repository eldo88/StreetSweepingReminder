import { fileURLToPath } from 'node:url'
import { mergeConfig, defineConfig, configDefaults } from 'vitest/config'
import viteConfig from './vite.config'

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: 'jsdom',
      exclude: [...configDefaults.exclude, 'e2e/**'],
      root: fileURLToPath(new URL('./', import.meta.url)),
      globals: true,
      setupFiles: ['./src/components/__tests__/setup/test-utils.js'],
      deps: {
        optimizer: {
          web: {
            include: ['@vue', '@vueuse', '@pinia', 'element-plus', 'vee-validate'],
          },
        },
        interopDefault: true,
        registerNodeLoader: true,
        transformCjsImport: true,
      },
    },
  }),
)
