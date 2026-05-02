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
        <span class="login-page__badge">水利信息系统</span>
        <h1>面向工程、站点、告警与空间展示的一体化水利信息管理后台</h1>
        <p class="hero-en-title">Integrated Water Conservancy Information Management System</p>
        <p class="hero-description">
          本系统旨在通过数字化手段，实现水利工程全要素的实时监测、告警闭环与空间决策分析，提升水务管理的科学化水平。
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
              <el-input
                v-model="form.username"
                placeholder="请输入用户名"
                size="large"
              />
            </div>
            <div class="form-group">
              <label>密码</label>
              <el-input
                v-model="form.password"
                type="password"
                placeholder="请输入密码"
                size="large"
                show-password
              />
            </div>
            <el-button
              type="primary"
              size="large"
              class="login-submit"
              :loading="authStore.loading"
              native-type="submit"
            >
              登录系统
            </el-button>
          </form>
          <div class="login-card__tips">
            <p>请联系管理员获取账号密码</p>
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
  username: '',
  password: ''
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
  } catch (error: unknown) {
    const msg = (error as { code?: string })?.code === 'ERR_NETWORK'
      ? '无法连接到服务器，请确认后端服务已启动'
      : error instanceof Error ? error.message : '登录失败，请检查账号密码'
    ElMessage.error(msg)
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--wi-auth-hero-background);
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
  color: var(--wi-text-inverse-primary);
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
  background: var(--wi-auth-hero-decorative-badge-bg);
  color: var(--wi-text-inverse-primary);
  font-size: 13px;
  font-weight: 700;
  backdrop-filter: blur(4px);
  border: 1px solid var(--wi-auth-hero-decorative-badge-border);
  margin-bottom: 24px;
}

.login-page__hero h1 {
  margin: 0 0 24px;
  max-width: 760px;
  font-size: clamp(36px, 3.5vw, 52px);
  line-height: 1.3;
  letter-spacing: -0.02em;
  font-weight: 700;
  text-shadow: var(--wi-auth-hero-decorative-title-shadow);
  margin-bottom: 8px;
}

/*英文副标题样式 */
.hero-en-title {
  font-size: 18px !important;
  color: var(--wi-color-brand-300) !important;
  opacity: 0.8;
  letter-spacing: 0.05em;
  margin-bottom: 24px !important;
  font-style: italic;
}

.login-page__hero p {
  color: var(--wi-text-inverse-secondary);
  font-size: 16px;
  line-height: 1.8;
  margin-bottom: 12px;
  max-width: 600px;
}

.login-page__hero ul {
  margin: 16px 0 0;
  padding-left: 20px;
  color: var(--wi-text-inverse-secondary);
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
  justify-content: flex-end;
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
  background: var(--wi-auth-hero-overlay-strong);
  animation: waveAnim 8s linear infinite;
}

.login-waves .wave:nth-child(2) {
  background: var(--wi-auth-hero-overlay-medium);
  animation: waveAnim 12s linear infinite reverse;
  bottom: -10px;
}

.login-waves .wave:nth-child(3) {
  background: var(--wi-auth-hero-overlay-soft);
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
  background: var(--wi-auth-hero-decorative-particle);
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
  background: color-mix(in srgb, var(--wi-auth-glass-card-bg) 96%, transparent);
  border: 1px solid color-mix(in srgb, var(--wi-text-inverse-primary) 14%, transparent);
  border-radius: var(--wi-app-radius-lg, 18px);
  padding: 40px;
  box-shadow:
    var(--wi-auth-glass-card-shadow),
    0 0 0 1px var(--wi-auth-glass-card-inset-border) inset;
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
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
  margin-bottom: 24px;
  color: var(--wi-text-primary);
  font-weight: 600;
}

.form-group {
  margin-bottom: 24px;
  text-align: left;
}

.form-group label {
  display: block;
  margin-bottom: 12px;
  font-weight: 500;
  color: var(--wi-text-secondary);
  font-size: 14px;
}

.login-options {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
  color: var(--wi-text-secondary);
  font-size: 13px;
}

.login-options label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  color: var(--wi-text-secondary);
}

.login-options a {
  color: var(--wi-primary);
  text-decoration: none;
  font-size: 13px;
  transition: opacity 0.2s ease;
}

.login-options a:hover {
  opacity: 0.8;
}

.login-card__tips {
  margin-top: 24px;
  padding-top: 16px;
  border-top: 1px solid var(--wi-border-default);
  color: var(--wi-text-disabled);
  font-size: 13px;
  text-align: center;
}

.login-card__tips p {
  margin: 4px 0;
}

.login-submit {
  width: 100%;
  height: 44px;
  margin-top: 8px;
  border-radius: var(--wi-app-radius-sm, 10px);
  font-weight: 500;
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
