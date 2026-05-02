<template>
  <div class="page-shell">
    <ChartSection
      :title="`${metricMeta.label}趋势分析`"
      description="查看当前筛选条件下的历史趋势变化。"
    >
      <template #actions>
        <div class="monitoring-section-actions">
          <span class="entity-pill">{{ metricMeta.label }}</span>
          <span class="entity-pill">{{ timeRangeLabel }}</span>
        </div>
      </template>

      <div class="monitoring-analysis-layout">
        <div class="monitoring-analysis-layout__chart">
          <TrendLineChart
            v-if="historyPoints.length > 0"
            :points="historyPoints"
            :series-type="metricMeta.seriesType"
            :unit="metricMeta.unit"
          />
          <el-empty v-else description="当前筛选条件下暂无历史数据" />
        </div>

        <div class="monitoring-analysis-layout__aside">
          <div :class="`entity-state-note entity-state-note--${trendNoteTone}`">
            <span class="entity-state-note__eyebrow">趋势判断</span>
            <strong>{{ trendHeading }}</strong>
            <p>{{ trendDescription }}</p>
          </div>

          <div class="entity-facts-grid entity-facts-grid--2 monitoring-analysis-facts">
            <div class="entity-facts-grid__item">
              <span>最新采样值</span>
              <strong>{{ latestValueLabel }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>变化幅度</span>
              <strong>{{ deltaLabel }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>最新采样时间</span>
              <strong>{{ formatDateTime(monitoringOverview.latestCollectedAt) }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>当前站点</span>
              <strong>{{ latestStationName }}</strong>
            </div>
          </div>
        </div>
      </div>
    </ChartSection>

    <TableSection
      title="历史记录"
      description="查看监测采样明细与风险判断结果。"
      :loading="loading"
      :has-data="rows.length > 0"
      :total="total"
      empty-description="当前筛选条件下暂无监测记录"
    >
      <template #actions>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">录入监测数据</el-button>
      </template>

      <FilterBar class="toolbar toolbar--monitoring">
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
        <template #actions>
          <el-button type="primary" @click="loadPageData">查询</el-button>
        </template>
      </FilterBar>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="stationName" label="站点名称" min-width="160" />
        <el-table-column label="监测指标" width="130">
          <template #default="{ row }">
            <span class="entity-pill">{{ dataTypeLabel(row.dataType) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="监测值" width="150">
          <template #default="{ row }">{{ formatMonitoringValue(row.value, row.dataType) }}</template>
        </el-table-column>
        <el-table-column prop="collectedAt" label="采集时间" min-width="180">
          <template #default="{ row }">{{ formatDateTime(row.collectedAt) }}</template>
        </el-table-column>
        <el-table-column label="风险判断" width="120">
          <template #default="{ row }">
            <StatusTag category="riskStatus" :value="row.triggeredAlarm ? 'Warning' : 'Normal'" />
          </template>
        </el-table-column>
        <el-table-column prop="remark" label="备注" min-width="220" show-overflow-tooltip />
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

    <el-dialog v-model="dialogVisible" title="录入监测数据" width="720px">
      <el-form :model="form" label-position="top" class="entity-form">
        <div class="entity-form__intro">
          <strong>创建新的监测采样记录</strong>
          <p>先确认采集站点与采样时间，再录入监测值和补充说明，让监测历史与趋势分析保持同一上下文。</p>
        </div>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ dialogSections.collect.title }}</h4>
            <p>{{ dialogSections.collect.description }}</p>
          </div>
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
            <el-col :span="24">
              <el-form-item label="采集时间">
                <el-date-picker v-model="form.collectedAt" type="datetime" value-format="YYYY-MM-DDTHH:mm:ss" class="w-full" />
              </el-form-item>
            </el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ dialogSections.value.title }}</h4>
            <p>{{ dialogSections.value.description }}</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="监测值">
                <el-input-number v-model="form.value" :precision="2" class="w-full" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <div class="entity-form__intro monitoring-dialog-note">
                <strong>当前指标</strong>
                <p>{{ getMonitoringMetricMeta(form.dataType).label }}，单位 {{ getMonitoringMetricMeta(form.dataType).unit }}</p>
              </div>
            </el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ dialogSections.supplement.title }}</h4>
            <p>{{ dialogSections.supplement.description }}</p>
          </div>
          <el-form-item label="备注">
            <el-input v-model="form.remark" type="textarea" :rows="4" />
          </el-form-item>
        </section>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">提交记录</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import dayjs from 'dayjs'
import { ElMessage } from 'element-plus'
import ChartSection from '@/components/common/ChartSection.vue'
import FilterBar from '@/components/common/FilterBar.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TableSection from '@/components/common/TableSection.vue'
import TrendLineChart from '@/components/charts/TrendLineChart.vue'
import { createMonitoringRecord, fetchMonitoringHistory, fetchMonitoringRecords, type MonitoringFormModel } from '@/api/modules/monitoring'
import { useStationOptions } from '@/composables/useStationOptions'
import { useAuthStore } from '@/stores/auth'
import { buildMonitoringOverview, getMonitoringDialogSections, getMonitoringMetricMeta } from './monitoringPresentation'
import type { MonitoringItem, TrendPoint } from '@/types/models'
import { formatDateTime } from '@/utils/format'

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

const dialogSections = getMonitoringDialogSections()

const latestRow = computed(() => [...rows.value].sort((left, right) => right.collectedAt.localeCompare(left.collectedAt))[0] ?? null)
const currentMetricType = computed(() => query.dataType || latestRow.value?.dataType || form.dataType)
const metricMeta = computed(() => getMonitoringMetricMeta(currentMetricType.value))
const monitoringOverview = computed(() => buildMonitoringOverview(rows.value, historyPoints.value, total.value, currentMetricType.value))
const latestStationName = computed(() => latestRow.value?.stationName || '--')
const latestValueLabel = computed(() => formatValueWithUnit(monitoringOverview.value.latestValue, metricMeta.value.unit))
const deltaLabel = computed(() => `${monitoringOverview.value.delta > 0 ? '+' : ''}${monitoringOverview.value.delta} ${metricMeta.value.unit}`.trim())
const directionLabel = computed(() => {
  if (monitoringOverview.value.direction === 'up') return '持续上升'
  if (monitoringOverview.value.direction === 'down') return '有所回落'
  return '基本平稳'
})
const deltaDescription = computed(() => `与上一采样点相比，${metricMeta.value.label}当前呈现${directionLabel.value}。`)
const trendNoteTone = computed(() => {
  if (monitoringOverview.value.triggeredCount > 0) return 'warning'
  if (monitoringOverview.value.direction === 'flat') return 'info'
  return 'success'
})
const trendHeading = computed(() => {
  if (monitoringOverview.value.triggeredCount > 0) return '当前需结合预警样本重点关注'
  if (monitoringOverview.value.direction === 'up') return `${metricMeta.value.label}处于上行阶段`
  if (monitoringOverview.value.direction === 'down') return `${metricMeta.value.label}出现回落`
  return `${metricMeta.value.label}保持平稳`
})
const trendDescription = computed(() => {
  if (monitoringOverview.value.triggeredCount > 0) {
    return '当前结果集中已经出现触发预警判断的样本，建议优先结合右侧摘要和下方历史记录核对异常区间。'
  }

  return `主图聚焦${metricMeta.value.label}的连续变化，右侧摘要用于快速确认最新值、变化幅度与筛选上下文。`
})
const timeRangeLabel = computed(() => {
  if (timeRange.value.length === 2) {
    return `${formatDateTime(timeRange.value[0])} 至 ${formatDateTime(timeRange.value[1])}`
  }

  return '未限定时间范围'
})
const filterSummary = computed(() => {
  const stationLabel = stations.value.find((item) => item.id === query.stationId)?.name || '全部站点'
  const metricLabel = query.dataType ? dataTypeLabel(query.dataType) : '全部指标'
  return `${stationLabel} / ${metricLabel} / ${timeRangeLabel.value}`
})

function dataTypeLabel(value: string) {
  return dataTypeOptions.find((item) => item.value === value)?.label ?? value
}

function formatValueWithUnit(value: number, unit: string) {
  return `${value}${unit ? ` ${unit}` : ''}`.trim()
}

function formatMonitoringValue(value: number, dataType: string) {
  const currentMeta = getMonitoringMetricMeta(dataType)
  return formatValueWithUnit(value, currentMeta.unit)
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
.monitoring-panel-meta {
  gap: 12px;
}

.monitoring-overview-copy__title {
  margin: 0 0 8px;
  color: var(--wi-text-primary);
  font-size: 14px;
  font-weight: 600;
}

.monitoring-overview-copy p:last-child {
  margin: 0;
  color: var(--wi-text-secondary);
  line-height: 1.8;
}

.monitoring-section-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  justify-content: flex-end;
}

.monitoring-analysis-layout {
  display: grid;
  grid-template-columns: minmax(0, 1.45fr) minmax(280px, 0.55fr);
  gap: 22px;
  align-items: flex-start;
}

.monitoring-analysis-layout__chart {
  min-width: 0;
}

.monitoring-analysis-layout__aside {
  display: grid;
  gap: 16px;
}

.monitoring-analysis-facts,
.monitoring-analysis-details {
  padding: 16px;
  border-radius: var(--wi-app-radius-md);
  background: var(--wi-app-surface-secondary);
  border: 1px solid var(--wi-app-border-subtle);
}

.monitoring-dialog-note {
  height: 100%;
}

@media (max-width: 1200px) {
  .monitoring-overview-grid,
  .monitoring-overview-grid__cards,
  .monitoring-analysis-layout {
    grid-template-columns: 1fr;
  }
}
</style>
