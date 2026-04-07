<template>
  <div class="login-page">
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

    <section class="login-card panel">
      <div class="login-card__head">
        <h2>系统登录</h2>
        <p>使用已初始化的演示账户进入系统</p>
      </div>
      <el-form label-position="top" @submit.prevent="handleLogin">
        <el-form-item label="用户名">
          <el-input v-model="form.username" placeholder="admin / viewer" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="form.password" type="password" placeholder="请输入密码" show-password @keyup.enter="handleLogin" />
        </el-form-item>
        <el-button type="primary" size="large" class="login-card__button" :loading="authStore.loading" @click="handleLogin">
          登录系统
        </el-button>
      </el-form>
      <div class="login-card__tips">
        <p>默认管理员：admin / Admin@123</p>
        <p>默认普通用户：viewer / Viewer@123</p>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
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

<style scoped lang="scss">
.login-page {
  min-height: 100vh;
  display: grid;
  grid-template-columns: 1.2fr 0.8fr;
  gap: 28px;
  padding: 40px;
}

.login-page__hero,
.login-card {
  padding: 34px;
}

.login-page__hero {
  color: var(--wi-text);
  display: flex;
  flex-direction: column;
  justify-content: center;

  h1 {
    margin: 16px 0 18px;
    max-width: 760px;
    font-size: clamp(36px, 4vw, 58px);
    line-height: 1.08;
    letter-spacing: -0.03em;
  }

  p,
  li {
    color: var(--wi-text-soft);
    font-size: 15px;
    line-height: 1.8;
  }

  ul {
    margin: 18px 0 0;
    padding-left: 20px;
  }
}

.login-page__badge {
  display: inline-flex;
  padding: 8px 14px;
  width: fit-content;
  border-radius: 999px;
  background: rgba(15, 108, 123, 0.08);
  color: var(--wi-primary);
  font-size: 13px;
  font-weight: 700;
}

.login-card {
  align-self: center;
}

.login-card__head {
  margin-bottom: 26px;

  h2 {
    margin: 0;
    font-size: 28px;
  }

  p {
    margin: 10px 0 0;
    color: var(--wi-text-soft);
  }
}

.login-card__button {
  width: 100%;
}

.login-card__tips {
  margin-top: 18px;
  color: var(--wi-text-soft);
  font-size: 13px;
}

@media (max-width: 960px) {
  .login-page {
    grid-template-columns: 1fr;
    padding: 24px;
  }
}
</style>
