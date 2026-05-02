import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { applyCssVariables } from '@/theme/applyCssVariables'
import '@/assets/styles/theme.scss'
import '@/assets/styles/base.scss'
import App from '@/App.vue'
import router from '@/router'

/* 颜色 CSS 变量由 tokens.ts 唯一生成，必须在组件渲染前同步写入 */
applyCssVariables()

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')
