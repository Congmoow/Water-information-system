<template>
  <div class="login-page">
    <!-- 动态水波纹效果 -->
    <div class="login-waves">
      <div class="wave"></div>
      <div class="wave"></div>
      <div class="wave"></div>
    </div>

    <!-- 浮动粒子 -->
    <div class="login-particles">
      <div
        v-for="item in particles"
        :key="item.id"
        class="particle"
        :style="item.style"
      ></div>
    </div>

    <div class="login-content">
      <section class="login-page__hero">
        <span class="login-page__badge">课程项目答辩演示系统</span>
        <h1>面向工程、站点、告警与空间展示的一体化水利信息管理后台</h1>
        <p>
          当前阶段先完成后台管理骨架与登录入口，下一阶段接入 JWT 认证、权限和真实业务数据。
        </p>
        <ul>
          <li>统一的工程与站点管理视图</li>
          <li>监测数据与告警处理闭环</li>
          <li>首页统计、趋势和地图联动</li>
        </ul>
      </section>

      <div class="login-form-wrapper">
        <div class="login-card">
          <h2>系统登录</h2>
          <form @submit.prevent="handleLogin">
            <div class="form-group">
              <label>用户名</label>
              <input
                v-model="form.username"
                class="form-input"
                placeholder="admin / viewer"
                required
              />
            </div>
            <div class="form-group">
              <label>密码</label>
              <input
                v-model="form.password"
                type="password"
                class="form-input"
                placeholder="请输入密码"
                required
              />
            </div>
            <div class="login-options">
              <label>
                <input type="checkbox" /> 记住密码
              </label>
              <a href="#">忘记密码？</a>
            </div>
            <el-button
              type="primary"
              size="large"
              style="width: 100%; border-radius: var(--radius-md); font-weight: 500; height: 44px; margin-top: 8px;"
              :loading="authStore.loading"
              native-type="submit"
            >
              登录系统
            </el-button>
          </form>
          <div class="login-card__tips">
            <p>默认管理员：admin / Admin@123</p>
            <p>默认普通用户：viewer / Viewer@123</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const form = reactive({
  username: 'admin',
  password: 'Admin@123'
})

// 生成粒子效果
const particles = reactive<Array<{ id: number; style: Record<string, string> }>>([])

onMounted(() => {
  for (let i = 0; i < 20; i++) {
    particles.push({
      id: i,
      style: {
        left: `${Math.random() * 100}%`,
        width: `${Math.random() * 4 + 2}px`,
        height: `${Math.random() * 4 + 2}px`,
        animationDuration: `${Math.random() * 6 + 4}s`,
        animationDelay: `${Math.random() * 5}s`
      }
    })
  }
})

async function handleLogin() {
  if (!form.username || !form.password) {
    ElMessage.warning('请输入用户名和密码')
    return
  }

  try {
    await authStore.login(form)
    ElMessage.success('登录成功')
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/dashboard'
    await router.push(redirect)
  } catch (error) {
    const message = error instanceof Error ? error.message : '登录失败，请检查账号密码'
    ElMessage.error(message)
  }
}
</script>

<style scoped>
.login-page {
  /* 基础变量 */
  --spacing-xs: 8px;
  --spacing-sm: 12px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
  --spacing-2xl: 40px;
  --radius-md: 8px;
  --radius-lg: 16px;
  --radius-xl: 24px;
  --text-primary: #1e293b;
  --text-secondary: #475569;
  --text-white: #ffffff;
  --text-muted: #94a3b8;
  --border-color: #e2e8f0;
  --bg-input: #f8fafc;
  --bg-white: #ffffff;
  --primary-color: #1a6fb5;
  --transition-fast: 0.2s ease;

  min-height: 100vh;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #0e1e38 0%, #1a365d 50%, #2a5298 100%);
  overflow: hidden;
  font-family: var(--el-font-family);
}

/* 布局内容区域 */
.login-content {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 1400px;
  display: grid;
  grid-template-columns: 1.2fr 0.8fr;
  gap: 60px;
  padding: 40px 60px;
}

/* 左侧 Hero 区域样式 */
.login-page__hero {
  color: var(--text-white);
  display: flex;
  flex-direction: column;
  justify-content: center;
  animation: fadeDown 0.8s ease-out;
}

.login-page__badge {
  display: inline-flex;
  padding: 8px 14px;
  width: fit-content;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.1);
  color: var(--text-white);
  font-size: 13px;
  font-weight: 700;
  backdrop-filter: blur(4px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin-bottom: 24px;
}

.login-page__hero h1 {
  margin: 0 0 24px;
  max-width: 760px;
  font-size: clamp(36px, 3.5vw, 52px);
  line-height: 1.15;
  letter-spacing: -0.02em;
  font-weight: 700;
  text-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.login-page__hero p {
  color: rgba(255, 255, 255, 0.85);
  font-size: 16px;
  line-height: 1.8;
  margin-bottom: 12px;
  max-width: 600px;
}

.login-page__hero ul {
  margin: 16px 0 0;
  padding-left: 20px;
  color: rgba(255, 255, 255, 0.85);
  font-size: 16px;
  line-height: 1.8;
}

.login-page__hero li {
  margin-bottom: 8px;
}

/* 右侧登录表单容器 */
.login-form-wrapper {
  display: flex;
  align-items: center;
  justify-content: flex-end; /* 或者 center */
}

/* 动态水波纹效果 */
.login-waves {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 200px;
  z-index: 0;
  overflow: hidden;
  pointer-events: none;
}

.login-waves .wave {
  position: absolute;
  bottom: 0;
  left: -50%;
  width: 200%;
  height: 100%;
  border-radius: 40%;
  opacity: 0.15;
}

.login-waves .wave:nth-child(1) {
  background: rgba(255, 255, 255, 0.2);
  animation: waveAnim 8s linear infinite;
}

.login-waves .wave:nth-child(2) {
  background: rgba(255, 255, 255, 0.15);
  animation: waveAnim 12s linear infinite reverse;
  bottom: -10px;
}

.login-waves .wave:nth-child(3) {
  background: rgba(255, 255, 255, 0.1);
  animation: waveAnim 16s linear infinite;
  bottom: -20px;
}

@keyframes waveAnim {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 浮动粒子 */
.login-particles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
  pointer-events: none;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  animation: floatUp linear infinite;
  bottom: 0;
}

@keyframes floatUp {
  0% {
    transform: translateY(100vh) scale(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-10vh) scale(1);
    opacity: 0;
  }
}

@keyframes fadeDown {
  from { opacity: 0; transform: translateY(-20px); }
  to { opacity: 1; transform: translateY(0); }
}

/* 右侧毛玻璃卡片 */
.login-card {
  width: 100%;
  max-width: 440px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: var(--radius-xl);
  padding: var(--spacing-2xl);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(20px);
  animation: fadeUp 0.8s ease-out 0.2s both;
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

.login-card h2 {
  text-align: center;
  font-size: 24px;
  margin-top: 0;
  margin-bottom: var(--spacing-lg);
  color: var(--text-primary);
  font-weight: 600;
}

.form-group {
  margin-bottom: var(--spacing-lg);
  text-align: left;
}

.form-group label {
  display: block;
  margin-bottom: var(--spacing-sm);
  font-weight: 500;
  color: var(--text-secondary);
  font-size: 14px;
}

.form-input {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  background: var(--bg-input);
  color: var(--text-primary);
  font-size: 14px;
  transition: all var(--transition-fast);
  box-sizing: border-box;
  outline: none;
}

.form-input:focus {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(26, 111, 181, 0.12);
  background: var(--bg-white);
}

.form-input::placeholder {
  color: var(--text-muted);
}

.login-options {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--spacing-lg);
  font-size: 13px;
}

.login-options label {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  cursor: pointer;
  color: var(--text-secondary);
}

.login-options a {
  color: var(--primary-color);
  text-decoration: none;
  font-size: 13px;
  transition: opacity var(--transition-fast);
}

.login-options a:hover {
  opacity: 0.8;
}

.login-card__tips {
  margin-top: 24px;
  padding-top: 16px;
  border-top: 1px solid var(--border-color);
  color: var(--text-muted);
  font-size: 13px;
  text-align: center;
}

.login-card__tips p {
  margin: 4px 0;
}

@media (max-width: 960px) {
  .login-content {
    grid-template-columns: 1fr;
    padding: 24px;
    gap: 40px;
  }

  .login-form-wrapper {
    justify-content: center;
  }
}
</style>
