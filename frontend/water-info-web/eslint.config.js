import js from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginVue from 'eslint-plugin-vue';
import prettierConfig from 'eslint-config-prettier';
import prettierPlugin from 'eslint-plugin-prettier';
import globals from 'globals';

export default [
  // 全局忽略
  {
    ignores: ['dist/**', 'node_modules/**', '**/*.d.ts', 'src/auto-imports.d.ts', 'src/components.d.ts'],
  },

  // 浏览器全局变量
  {
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.es2021,
      },
    },
  },

  // JS/TS 推荐规则
  js.configs.recommended,

  // TypeScript 推荐规则
  ...tseslint.configs.recommended,

  // Vue 3 推荐规则（直接展开，插件注册和文件限制由插件内部处理）
  ...pluginVue.configs['flat/recommended'],

  // Vue 文件的 TypeScript 配置（使用 vue-eslint-parser + typescript-eslint 解析器）
  {
    files: ['**/*.vue'],
    languageOptions: {
      parserOptions: {
        parser: tseslint.parser,
        ecmaVersion: 'latest',
        sourceType: 'module',
      },
    },
  },

  // TypeScript 文件配置
  {
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module',
      },
    },
  },

  // 全局规则调整（适合课程项目，不要太严格）
  {
    rules: {
      // TypeScript 规则 - 降级为 warn，不阻塞开发
      '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_' }],
      '@typescript-eslint/no-explicit-any': 'warn',
      '@typescript-eslint/no-empty-object-type': 'warn',
      '@typescript-eslint/no-require-imports': 'off',

      // 通用规则
      'no-console': ['warn', { allow: ['warn', 'error'] }],
      'no-debugger': 'warn',
    },
  },

  // Vue 文件专用规则覆盖
  {
    files: ['**/*.vue'],
    rules: {
      'vue/multi-word-component-names': 'off', // 课程项目中单词组件名很常见
      'vue/max-attributes-per-line': 'off',
      'vue/singleline-html-element-content-newline': 'off',
      'vue/html-self-closing': [
        'warn',
        { html: { void: 'always', normal: 'never', component: 'always' } },
      ],
      'vue/require-default-prop': 'off',
      'vue/require-explicit-emits': 'warn',
      '@typescript-eslint/no-explicit-any': 'warn',
    },
  },

  // Prettier 集成（必须放在最后，禁用冲突规则）
  prettierConfig,
  {
    plugins: { prettier: prettierPlugin },
    rules: {
      'prettier/prettier': 'warn',
    },
  },
];
