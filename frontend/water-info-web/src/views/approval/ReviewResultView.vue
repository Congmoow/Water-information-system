<template>
  <div class="page-shell">
    <PageHeader title="初审结果" subtitle="查看 AI Agent 合规审查结果" />

    <div v-loading="loading" class="review-container">
      <template v-if="detail">
        <!-- 申请基本信息 -->
        <el-card shadow="never" class="info-card">
          <template #header>
            <div class="card-header">
              <span>申请信息</span>
              <el-tag :type="statusTagType(detail.status)" size="large">{{ statusLabel(detail.status) }}</el-tag>
            </div>
          </template>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="申请人">{{ detail.applicantName }}</el-descriptions-item>
            <el-descriptions-item label="证件号">{{ detail.applicantIdCard }}</el-descriptions-item>
            <el-descriptions-item label="企业名称">{{ detail.enterpriseName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="营业执照号">{{ detail.enterpriseLicenseNo || '-' }}</el-descriptions-item>
            <el-descriptions-item label="取水地点">{{ detail.waterIntakeLocation }}</el-descriptions-item>
            <el-descriptions-item label="取水用途">{{ detail.waterIntakePurpose }}</el-descriptions-item>
            <el-descriptions-item label="申请取水量">{{ detail.waterIntakeAmount }} m³</el-descriptions-item>
            <el-descriptions-item label="申请日期">{{ formatDateTime(detail.applicationDate) }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <!-- 附件列表 -->
        <el-card v-if="detail.attachments.length" shadow="never" class="info-card">
          <template #header>申请附件</template>
          <el-table :data="detail.attachments" border size="small">
            <el-table-column prop="fileName" label="文件名" />
            <el-table-column prop="fileType" label="类型" width="100" />
            <el-table-column prop="attachmentType" label="附件类别" width="140">
              <template #default="{ row }">{{ attachmentTypeLabel(row.attachmentType) }}</template>
            </el-table-column>
          </el-table>
        </el-card>

        <!-- 审查结果 -->
        <el-card v-if="detail.reviewResults.length" shadow="never" class="info-card">
          <template #header>
            <div class="card-header">
              <span>AI 审查结论</span>
              <el-button
                v-if="detail.status === 'Pending'"
                type="primary"
                size="small"
                :loading="reviewing"
                @click="handleReview"
              >
                提交审查
              </el-button>
            </div>
          </template>

          <div v-for="result in detail.reviewResults" :key="result.id" class="review-result-block">
            <div class="review-summary">
              <el-icon :size="24" :color="result.isPassed ? '#67c23a' : '#f56c6c'">
                <component :is="result.isPassed ? 'CircleCheck' : 'CircleClose'" />
              </el-icon>
              <div>
                <strong>{{ result.isPassed ? '审查通过' : '审查未通过' }}</strong>
                <p>{{ result.summary }}</p>
              </div>
              <span class="review-time">{{ formatDateTime(result.reviewedAt) }}</span>
            </div>

            <!-- 不合规项列表 -->
            <div v-if="result.findings.length" class="findings-list">
              <div
                v-for="finding in result.findings"
                :key="finding.id"
                class="finding-item"
                :class="`finding-item--${finding.severity.toLowerCase()}`"
              >
                <div class="finding-header">
                  <el-tag :type="severityTagType(finding.severity)" size="small">{{ finding.severity }}</el-tag>
                  <el-tag type="info" size="small">{{ finding.category }}</el-tag>
                </div>
                <p class="finding-desc">{{ finding.description }}</p>
                <p v-if="finding.suggestion" class="finding-suggestion">
                  <strong>建议：</strong>{{ finding.suggestion }}
                </p>
              </div>
            </div>

            <el-empty v-else description="未发现不合规问题" :image-size="60" />
          </div>
        </el-card>

        <el-card v-else shadow="never" class="info-card">
          <el-empty description="暂无审查结果，请先提交审查">
            <el-button type="primary" :loading="reviewing" @click="handleReview">提交 AI 审查</el-button>
          </el-empty>
        </el-card>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { CircleCheck, CircleClose } from '@element-plus/icons-vue'
import PageHeader from '@/components/common/PageHeader.vue'
import { fetchApprovalDetail, submitForReview } from '@/api/modules/approval'
import type { ApprovalDetail } from '@/types/approval'
import { formatDateTime } from '@/utils/format'

const route = useRoute()
const loading = ref(false)
const reviewing = ref(false)
const detail = ref<ApprovalDetail | null>(null)

const id = route.params.id as string

async function loadData() {
  loading.value = true
  try {
    detail.value = await fetchApprovalDetail(id)
  } catch {
    ElMessage.error('加载审批详情失败')
  } finally {
    loading.value = false
  }
}

async function handleReview() {
  reviewing.value = true
  try {
    detail.value = await submitForReview(id)
    ElMessage.success('审查完成')
  } catch {
    ElMessage.error('审查失败，请检查 AI 服务是否可用')
  } finally {
    reviewing.value = false
  }
}

function statusTagType(status: string) {
  const map: Record<string, string> = { Pending: 'info', Reviewing: 'warning', Approved: 'success', Rejected: 'danger' }
  return map[status] || 'info'
}

function statusLabel(status: string) {
  const map: Record<string, string> = { Pending: '待处理', Reviewing: '审查中', Approved: '已通过', Rejected: '已驳回' }
  return map[status] || status
}

function severityTagType(severity: string) {
  const map: Record<string, string> = { Error: 'danger', Warning: 'warning', Info: 'info' }
  return map[severity] || 'info'
}

function attachmentTypeLabel(type: string) {
  const map: Record<string, string> = {
    application_form: '申请书',
    id_card: '身份证',
    business_license: '营业执照',
    other: '其他材料',
  }
  return map[type] || type
}

onMounted(loadData)
</script>

<style scoped lang="scss">
.review-container {
  max-width: 960px;
}

.info-card {
  margin-bottom: 16px;
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.review-result-block {
  margin-bottom: 16px;
}

.review-summary {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: var(--el-fill-color-lighter);
  border-radius: 8px;
  margin-bottom: 16px;

  strong {
    font-size: 16px;
  }

  p {
    margin: 4px 0 0;
    color: var(--el-text-color-secondary);
    font-size: 13px;
  }
}

.review-time {
  margin-left: auto;
  font-size: 12px;
  color: var(--el-text-color-secondary);
}

.findings-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.finding-item {
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 8px;
  padding: 16px;
  border-left: 4px solid var(--el-border-color);

  &--error {
    border-left-color: var(--el-color-danger);
    background: var(--el-color-danger-light-9);
  }

  &--warning {
    border-left-color: var(--el-color-warning);
    background: var(--el-color-warning-light-9);
  }

  &--info {
    border-left-color: var(--el-color-info);
  }
}

.finding-header {
  display: flex;
  gap: 8px;
  margin-bottom: 8px;
}

.finding-desc {
  margin: 0;
  font-size: 14px;
}

.finding-suggestion {
  margin: 8px 0 0;
  font-size: 13px;
  color: var(--el-text-color-secondary);
}
</style>
