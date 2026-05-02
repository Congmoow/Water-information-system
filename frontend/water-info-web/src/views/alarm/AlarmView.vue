<template>
  <div class="page-shell">
    <TableSection
      title="告警事件列表"
      description="支持按等级与状态筛选，查看处理上下文。"
      :loading="loading"
      :has-data="rows.length > 0"
      :total="total"
      empty-description="当前筛选条件下暂无告警数据"
    >
      <FilterBar class="toolbar toolbar--alarm">
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
        <template #actions>
          <el-button type="primary" @click="loadData">查询</el-button>
        </template>
      </FilterBar>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="stationName" label="站点名称" min-width="160" />
        <el-table-column label="告警等级" width="120">
          <template #default="{ row }">
            <StatusTag category="alarmLevel" :value="row.level" />
          </template>
        </el-table-column>
        <el-table-column label="处理状态" width="120">
          <template #default="{ row }">
            <StatusTag category="alarmStatus" :value="row.status" />
          </template>
        </el-table-column>
        <el-table-column prop="message" label="告警内容" min-width="280" show-overflow-tooltip />
        <el-table-column prop="triggeredAt" label="触发时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.triggeredAt) }}</template>
        </el-table-column>
        <el-table-column prop="handledAt" label="处理时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.handledAt) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="showDetail(row.id)">查看事件</el-button>
            <el-button
              v-if="isAdmin && row.status !== 'Resolved'"
              link
              type="warning"
              @click="openHandleDialog(row.id)"
            >
              进入处理
            </el-button>
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

    <el-drawer v-model="detailVisible" title="告警详情" size="460px">
      <SideInfoPanel
        v-if="detail"
        :title="detail.stationName"
        subtitle="事件处理面板"
      >
        <template #status>
          <StatusTag category="alarmStatus" :value="detail.status" />
        </template>
        <template #meta>
          <div :class="`alarm-lifecycle alarm-lifecycle--${lifecycleMeta?.tone ?? 'info'}`">
            <span class="alarm-lifecycle__eyebrow">处置状态</span>
            <strong>{{ lifecycleMeta?.heading }}</strong>
            <p>{{ lifecycleMeta?.description }}</p>
          </div>
          <div class="detail-meta-grid">
            <div>
              <span>告警等级</span>
              <strong><StatusTag category="alarmLevel" :value="detail.level" /></strong>
            </div>
            <div>
              <span>触发时间</span>
              <strong>{{ formatDateTime(detail.triggeredAt) }}</strong>
            </div>
          </div>
        </template>
        <dl class="detail-list">
          <div>
            <dt>处理时间</dt>
            <dd>{{ formatDateTime(detail.handledAt) }}</dd>
          </div>
          <div>
            <dt>处理备注</dt>
            <dd>{{ detail.handleRemark || '--' }}</dd>
          </div>
          <div>
            <dt>告警内容</dt>
            <dd>{{ detail.message }}</dd>
          </div>
        </dl>
        <template #footer>
          <div class="alarm-detail-footer">
            <div>
              <span>当前处理状态</span>
              <strong>{{ statusLabel(detail.status) }}</strong>
            </div>
            <el-button
              v-if="isAdmin && detail.status !== 'Resolved'"
              type="primary"
              @click="openHandleDialog(detail.id)"
            >
              更新处理结果
            </el-button>
          </div>
        </template>
      </SideInfoPanel>
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
import { ElMessage } from 'element-plus'
import FilterBar from '@/components/common/FilterBar.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TableSection from '@/components/common/TableSection.vue'
import { fetchAlarmDetail, fetchAlarms, handleAlarm } from '@/api/modules/alarm'
import { useStationOptions } from '@/composables/useStationOptions'
import { useAuthStore } from '@/stores/auth'
import { getAlarmLifecycleMeta } from './alarmPresentation'
import { formatDateTime } from '@/utils/format'
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
const lifecycleMeta = computed(() => (detail.value ? getAlarmLifecycleMeta(detail.value) : null))

function levelLabel(value: string) {
  return levelOptions.find((item) => item.value === value)?.label ?? value
}

function statusLabel(value: string) {
  return statusOptions.find((item) => item.value === value)?.label ?? value
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
.alarm-lifecycle {
  padding: 16px;
  border-radius: var(--wi-app-radius-md);
  border: 1px solid var(--wi-app-border-subtle);
}

.alarm-lifecycle__eyebrow {
  display: block;
  color: var(--wi-text-tertiary);
  font-size: 12px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.alarm-lifecycle strong {
  display: block;
  margin-top: 10px;
  font-size: 18px;
}

.alarm-lifecycle p {
  margin: 8px 0 0;
  line-height: 1.8;
}

.alarm-lifecycle--warning {
  background: var(--wi-warning-soft);
  color: var(--wi-warning);
}

.alarm-lifecycle--success {
  background: var(--wi-success-soft);
  color: var(--wi-success);
}

.alarm-lifecycle--danger {
  background: var(--wi-danger-soft);
  color: var(--wi-danger);
}

.detail-meta-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;

  div {
    display: flex;
    flex-direction: column;
    gap: 6px;
  }

  span {
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  strong {
    color: var(--wi-text-primary);
    font-size: 14px;
    display: inline-flex;
    align-items: center;
  }
}

.detail-list {
  display: grid;
  gap: 16px;
  margin: 0;

  dt {
    margin-bottom: 6px;
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  dd {
    margin: 0;
    color: var(--wi-text-primary);
    line-height: 1.8;
  }
}

.alarm-detail-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;

  span,
  strong {
    display: block;
  }

  span {
    color: var(--wi-text-tertiary);
    font-size: 12px;
  }

  strong {
    margin-top: 6px;
    color: var(--wi-text-primary);
    font-size: 14px;
  }
}

@media (max-width: 1200px) {
  .alarm-summary-grid,
  .alarm-summary-grid__cards {
    grid-template-columns: 1fr;
  }

  .alarm-detail-footer {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
