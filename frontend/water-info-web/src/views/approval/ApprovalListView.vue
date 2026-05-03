<template>
  <div class="page-shell">
    <TableSection
      title="审批管理"
      description="管理涉水取水许可审批申请，查看 AI 合规审查结果。"
      :loading="loading"
      :has-data="rows.length > 0"
      :total="total"
      empty-description="暂无审批申请"
    >
      <template #actions>
        <el-button type="primary" @click="$router.push('/approvals/create')">新建申请</el-button>
      </template>

      <FilterBar class="toolbar">
        <el-input v-model="query.keyword" placeholder="按申请人、企业名称搜索" clearable @keyup.enter="loadData" @clear="loadData" />
        <el-select v-model="query.status" placeholder="审批状态" clearable @change="loadData" style="width: 140px">
          <el-option label="待处理" value="Pending" />
          <el-option label="审查中" value="Reviewing" />
          <el-option label="已通过" value="Approved" />
          <el-option label="已驳回" value="Rejected" />
        </el-select>
        <template #actions>
          <el-button type="primary" @click="loadData">查询</el-button>
        </template>
      </FilterBar>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="applicantName" label="申请人" min-width="100" />
        <el-table-column prop="enterpriseName" label="企业名称" min-width="140" />
        <el-table-column prop="waterIntakeLocation" label="取水地点" min-width="140" />
        <el-table-column prop="waterIntakePurpose" label="取水用途" min-width="120" />
        <el-table-column prop="waterIntakeAmount" label="取水量(m³)" width="110" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="statusTagType(row.status)">{{ statusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="applicationDate" label="申请日期" min-width="160">
          <template #default="{ row }">{{ formatDateTime(row.applicationDate) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="260" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="$router.push(`/approvals/${row.id}/review`)">审查结果</el-button>
            <el-button v-if="row.status === 'Pending'" link type="success" @click="handleSubmitReview(row.id)">提交审查</el-button>
            <el-button link type="danger" @click="removeRow(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <template #pagination>
        <el-pagination
          background
          layout="total, prev, pager, next"
          :current-page="query.page"
          :page-size="query.pageSize"
          :total="total"
          @current-change="handlePageChange"
        />
      </template>
    </TableSection>
  </div>
</template>

<script setup lang="ts">
import { ElMessage, ElMessageBox } from 'element-plus'
import FilterBar from '@/components/common/FilterBar.vue'
import TableSection from '@/components/common/TableSection.vue'
import { fetchApprovals, deleteApproval, submitForReview } from '@/api/modules/approval'
import { useCrudPage } from '@/composables/useCrudPage'
import type { ApprovalItem, ApprovalDetail, ApprovalFormModel, ApprovalQuery } from '@/types/approval'
import { formatDateTime } from '@/utils/format'

const {
  loading,
  rows,
  total,
  query,
  loadData,
  handlePageChange,
  removeRow,
} = useCrudPage<ApprovalItem, ApprovalDetail, ApprovalFormModel, ApprovalQuery>({
  api: {
    fetchList: fetchApprovals,
    fetchDetail: async () => ({ } as ApprovalDetail),
    create: async () => ({ } as ApprovalDetail),
    update: async () => ({ } as ApprovalDetail),
    remove: deleteApproval,
  },
  initialQuery: () => ({ page: 1, pageSize: 10, keyword: '', status: '' }),
  initialForm: () => ({
    applicantName: '',
    applicantIdCard: '',
    waterIntakeLocation: '',
    waterIntakePurpose: '',
    waterIntakeAmount: 0,
  }),
  deleteConfirmMessage: '确认删除该审批申请吗？',
  deleteSuccessMessage: '删除成功',
})

function statusTagType(status: string) {
  const map: Record<string, string> = {
    Pending: 'info',
    Reviewing: 'warning',
    Approved: 'success',
    Rejected: 'danger',
  }
  return map[status] || 'info'
}

function statusLabel(status: string) {
  const map: Record<string, string> = {
    Pending: '待处理',
    Reviewing: '审查中',
    Approved: '已通过',
    Rejected: '已驳回',
  }
  return map[status] || status
}

async function handleSubmitReview(id: string) {
  await ElMessageBox.confirm('确认提交该申请进行 AI 合规审查？', '提交审查')
  try {
    await submitForReview(id)
    ElMessage.success('审查完成')
    loadData()
  } catch {
    ElMessage.error('审查失败，请检查 AI 服务是否可用')
  }
}
</script>
