<template>
  <div class="page-shell">
    <PageCard title="告警管理" subtitle="展示超阈值告警记录，支持按等级与状态筛选，以及管理员处理闭环">
      <div class="toolbar toolbar--alarm">
        <el-select v-model="query.stationId" placeholder="监测站点" clearable filterable>
          <el-option v-for="station in stations" :key="station.id" :label="station.name" :value="station.id" />
        </el-select>
        <el-select v-model="query.level" placeholder="告警等级" clearable>
          <el-option v-for="item in levelOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-select v-model="query.status" placeholder="处理状态" clearable>
          <el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-date-picker
          v-model="timeRange"
          type="datetimerange"
          value-format="YYYY-MM-DDTHH:mm:ss"
          start-placeholder="开始时间"
          end-placeholder="结束时间"
        />
        <el-button type="primary" @click="loadData">查询</el-button>
      </div>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="stationName" label="站点名称" min-width="160" />
        <el-table-column label="告警等级" width="120">
          <template #default="{ row }">
            <el-tag :type="levelTagType(row.level)">{{ levelLabel(row.level) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="处理状态" width="120">
          <template #default="{ row }">
            <el-tag :type="statusTagType(row.status)">{{ statusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="message" label="告警内容" min-width="220" show-overflow-tooltip />
        <el-table-column prop="triggeredAt" label="触发时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.triggeredAt) }}</template>
        </el-table-column>
        <el-table-column prop="handledAt" label="处理时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.handledAt) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="210" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="showDetail(row.id)">详情</el-button>
            <el-button
              v-if="isAdmin && row.status !== 'Resolved'"
              link
              type="warning"
              @click="openHandleDialog(row.id)"
            >
              处理
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <div class="table-footer">
        <el-pagination
          background
          layout="total, prev, pager, next"
          :current-page="query.page"
          :page-size="query.pageSize"
          :total="total"
          @current-change="handlePageChange"
        />
      </div>
    </PageCard>

    <el-drawer v-model="detailVisible" title="告警详情" size="460px">
      <el-descriptions v-if="detail" :column="1" border>
        <el-descriptions-item label="站点名称">{{ detail.stationName }}</el-descriptions-item>
        <el-descriptions-item label="等级">{{ levelLabel(detail.level) }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ statusLabel(detail.status) }}</el-descriptions-item>
        <el-descriptions-item label="触发时间">{{ formatDateTime(detail.triggeredAt) }}</el-descriptions-item>
        <el-descriptions-item label="处理时间">{{ formatDateTime(detail.handledAt) }}</el-descriptions-item>
        <el-descriptions-item label="处理备注">{{ detail.handleRemark || '--' }}</el-descriptions-item>
        <el-descriptions-item label="告警内容">{{ detail.message }}</el-descriptions-item>
      </el-descriptions>
    </el-drawer>

    <el-dialog v-model="dialogVisible" title="处理告警" width="560px">
      <el-form :model="handleForm" label-position="top">
        <el-form-item label="处理状态">
          <el-select v-model="handleForm.status" class="w-full">
            <el-option label="处理中" value="Processing" />
            <el-option label="已解决" value="Resolved" />
          </el-select>
        </el-form-item>
        <el-form-item label="处理备注">
          <el-input v-model="handleForm.handleRemark" type="textarea" :rows="4" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitHandle">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import dayjs from 'dayjs'
import { ElMessage } from 'element-plus'
import PageCard from '@/components/common/PageCard.vue'
import { fetchAlarmDetail, fetchAlarms, handleAlarm } from '@/api/modules/alarm'
import { useStationOptions } from '@/composables/useStationOptions'
import { useAuthStore } from '@/stores/auth'
import type { AlarmDetail, AlarmItem } from '@/types/models'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Administrator')
const { stations, loadStations } = useStationOptions()
const loading = ref(false)
const submitting = ref(false)
const detailVisible = ref(false)
const dialogVisible = ref(false)
const rows = ref<AlarmItem[]>([])
const detail = ref<AlarmDetail | null>(null)
const total = ref(0)
const handlingId = ref('')
const timeRange = ref<string[]>([])
const query = reactive({
  stationId: '',
  level: '',
  status: '',
  page: 1,
  pageSize: 10
})
const handleForm = reactive({
  status: 'Processing',
  handleRemark: ''
})

const levelOptions = [
  { label: '提示', value: 'Info' },
  { label: '预警', value: 'Warning' },
  { label: '严重', value: 'Critical' }
]

const statusOptions = [
  { label: '待处理', value: 'Pending' },
  { label: '处理中', value: 'Processing' },
  { label: '已解决', value: 'Resolved' }
]

function levelLabel(value: string) {
  return levelOptions.find((item) => item.value === value)?.label ?? value
}

function statusLabel(value: string) {
  return statusOptions.find((item) => item.value === value)?.label ?? value
}

function levelTagType(value: string) {
  if (value === 'Critical') return 'danger'
  if (value === 'Warning') return 'warning'
  return 'info'
}

function statusTagType(value: string) {
  if (value === 'Resolved') return 'success'
  if (value === 'Processing') return 'warning'
  return 'danger'
}

function formatDateTime(value?: string) {
  return value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '--'
}

function buildQueryParams() {
  return {
    stationId: query.stationId || undefined,
    level: query.level || undefined,
    status: query.status || undefined,
    startTime: timeRange.value[0] || undefined,
    endTime: timeRange.value[1] || undefined,
    page: query.page,
    pageSize: query.pageSize
  }
}

async function loadData() {
  loading.value = true
  try {
    const result = await fetchAlarms(buildQueryParams())
    rows.value = result.items
    total.value = result.total
  } finally {
    loading.value = false
  }
}

function handlePageChange(page: number) {
  query.page = page
  loadData()
}

async function showDetail(id: string) {
  detail.value = await fetchAlarmDetail(id)
  detailVisible.value = true
}

function openHandleDialog(id: string) {
  handlingId.value = id
  handleForm.status = 'Processing'
  handleForm.handleRemark = ''
  dialogVisible.value = true
}

async function submitHandle() {
  submitting.value = true
  try {
    await handleAlarm(handlingId.value, handleForm)
    ElMessage.success('告警处理状态已更新')
    dialogVisible.value = false
    await loadData()
    if (detailVisible.value && detail.value?.id === handlingId.value) {
      detail.value = await fetchAlarmDetail(handlingId.value)
    }
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  await loadStations()
  await loadData()
})
</script>

<style scoped lang="scss">
.toolbar {
  display: grid;
  gap: 14px;
  margin-bottom: 18px;
}

.toolbar--alarm {
  grid-template-columns: 1fr 0.7fr 0.7fr 1.1fr auto;
}

.table-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 18px;
}

.w-full {
  width: 100%;
}

@media (max-width: 1240px) {
  .toolbar--alarm {
    grid-template-columns: 1fr;
  }
}
</style>
