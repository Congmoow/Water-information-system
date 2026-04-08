<template>
  <div class="page-shell">
    <PageCard title="监测数据管理" subtitle="支持监测数据录入、按站点与时间范围筛选、历史记录和趋势展示">
      <template #extra>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">录入监测数据</el-button>
      </template>

      <div class="toolbar toolbar--monitoring">
        <el-select v-model="query.stationId" placeholder="监测站点" clearable filterable>
          <el-option v-for="station in stations" :key="station.id" :label="station.name" :value="station.id" />
        </el-select>
        <el-select v-model="query.dataType" placeholder="数据类型" clearable>
          <el-option v-for="item in dataTypeOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-date-picker
          v-model="timeRange"
          type="datetimerange"
          value-format="YYYY-MM-DDTHH:mm:ss"
          start-placeholder="开始时间"
          end-placeholder="结束时间"
        />
        <el-button type="primary" @click="loadPageData">查询</el-button>
      </div>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="stationName" label="站点名称" min-width="160" />
        <el-table-column label="数据类型" width="120">
          <template #default="{ row }">
            <el-tag :type="dataTypeTagType(row.dataType)">{{ dataTypeLabel(row.dataType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="value" label="监测值" width="120" />
        <el-table-column prop="collectedAt" label="采集时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.collectedAt) }}</template>
        </el-table-column>
        <el-table-column label="告警触发" width="120">
          <template #default="{ row }">
            <el-tag :type="row.triggeredAlarm ? 'danger' : 'success'">{{ row.triggeredAlarm ? '已触发' : '正常' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="remark" label="备注" min-width="180" show-overflow-tooltip />
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

    <PageCard title="历史趋势" subtitle="按照当前筛选条件展示采样变化趋势">
      <TrendLineChart
        v-if="historyPoints.length > 0"
        :points="historyPoints"
        series-type="waterLevel"
        unit=""
      />
      <el-empty v-else description="当前筛选条件下暂无历史数据" />
    </PageCard>

    <el-dialog v-model="dialogVisible" title="录入监测数据" width="620px">
      <el-form :model="form" label-position="top">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="监测站点">
              <el-select v-model="form.stationId" filterable class="w-full">
                <el-option v-for="station in stations" :key="station.id" :label="station.name" :value="station.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据类型">
              <el-select v-model="form.dataType" class="w-full">
                <el-option v-for="item in dataTypeOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="监测值">
              <el-input-number v-model="form.value" :precision="2" class="w-full" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="采集时间">
              <el-date-picker v-model="form.collectedAt" type="datetime" value-format="YYYY-MM-DDTHH:mm:ss" class="w-full" />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input v-model="form.remark" type="textarea" :rows="4" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import dayjs from 'dayjs'
import { ElMessage } from 'element-plus'
import PageCard from '@/components/common/PageCard.vue'
import TrendLineChart from '@/components/charts/TrendLineChart.vue'
import { createMonitoringRecord, fetchMonitoringHistory, fetchMonitoringRecords, type MonitoringFormModel } from '@/api/modules/monitoring'
import { useStationOptions } from '@/composables/useStationOptions'
import { useAuthStore } from '@/stores/auth'
import type { MonitoringItem, TrendPoint } from '@/types/models'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Administrator')
const { stations, loadStations } = useStationOptions()
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const rows = ref<MonitoringItem[]>([])
const historyPoints = ref<TrendPoint[]>([])
const total = ref(0)
const timeRange = ref<string[]>([])
const query = reactive({
  stationId: '',
  dataType: '',
  page: 1,
  pageSize: 10
})
const form = reactive<MonitoringFormModel>({
  stationId: '',
  dataType: 'WaterLevel',
  value: 0,
  collectedAt: dayjs().format('YYYY-MM-DDTHH:mm:ss'),
  remark: ''
})

const dataTypeOptions = [
  { label: '水位数据', value: 'WaterLevel' },
  { label: '雨量数据', value: 'Rainfall' },
  { label: '流量数据', value: 'Flow' }
]

function dataTypeLabel(value: string) {
  return dataTypeOptions.find((item) => item.value === value)?.label ?? value
}

function dataTypeTagType(value: string) {
  if (value === 'WaterLevel') return 'primary'
  if (value === 'Rainfall') return 'success'
  return 'warning'
}

function formatDateTime(value?: string) {
  return value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '--'
}

function buildQueryParams() {
  return {
    stationId: query.stationId || undefined,
    dataType: query.dataType || undefined,
    startTime: timeRange.value[0] || undefined,
    endTime: timeRange.value[1] || undefined,
    page: query.page,
    pageSize: query.pageSize
  }
}

function resetForm() {
  Object.assign(form, {
    stationId: stations.value[0]?.id ?? '',
    dataType: 'WaterLevel',
    value: 0,
    collectedAt: dayjs().format('YYYY-MM-DDTHH:mm:ss'),
    remark: ''
  })
}

async function loadPageData() {
  loading.value = true
  try {
    const [listResult, historyResult] = await Promise.all([
      fetchMonitoringRecords(buildQueryParams()),
      fetchMonitoringHistory(buildQueryParams())
    ])
    rows.value = listResult.items
    total.value = listResult.total
    historyPoints.value = historyResult
  } finally {
    loading.value = false
  }
}

function handlePageChange(page: number) {
  query.page = page
  loadPageData()
}

function openCreateDialog() {
  resetForm()
  dialogVisible.value = true
}

async function submitForm() {
  submitting.value = true
  try {
    const result = await createMonitoringRecord(form)
    ElMessage[result.triggeredAlarm ? 'warning' : 'success'](result.message)
    dialogVisible.value = false
    await loadPageData()
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  await loadStations()
  if (stations.value[0]) {
    form.stationId = stations.value[0].id
  }
  await loadPageData()
})
</script>

<style scoped lang="scss">
.toolbar--monitoring {
  grid-template-columns: 1fr 0.7fr 1.2fr auto;
}

@media (max-width: 1180px) {
  .toolbar--monitoring {
    grid-template-columns: 1fr;
  }
}
</style>
