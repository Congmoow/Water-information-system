<template>
  <div class="page-shell">
    <div class="monitoring-overview-grid">
      <div class="monitoring-overview-grid__cards">
        <MetricCard
          label="当前筛选记录"
          :value="monitoringOverview.total"
          description="基于当前筛选条件返回的监测记录总量。"
          highlight="记录规模"
          tone="info"
        />
        <MetricCard
          :label="`${metricMeta.label}最新值`"
          :value="latestValueLabel"
          description="优先呈现当前筛选结果中的最新采样值，方便先判断当前状态。"
          highlight="最新采样"
          tone="info"
        />
        <MetricCard
          label="趋势变化"
          :value="deltaLabel"
          :description="deltaDescription"
          :highlight="directionLabel"
          tone="info"
        />
        <MetricCard
          label="预警触发"
          :value="monitoringOverview.triggeredCount"
          description="当前结果集中触发预警判断的样本数量。"
          :highlight="monitoringOverview.triggeredCount > 0 ? '需关注' : '运行平稳'"
          :tone="monitoringOverview.triggeredCount > 0 ? 'warning' : 'success'"
        />
      </div>

      <SideInfoPanel title="监测概览" :subtitle="metricMeta.label">
        <template #status>
          <StatusTag
            category="riskStatus"
            :value="monitoringOverview.triggeredCount > 0 ? 'Warning' : 'Normal'"
          />
        </template>
        <template #meta>
          <div class="entity-facts-grid entity-facts-grid--2 monitoring-panel-meta">
            <div class="entity-facts-grid__item">
              <span>最近更新时间</span>
              <strong>{{ formatDateTime(monitoringOverview.latestCollectedAt) }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>当前时间范围</span>
              <strong>{{ timeRangeLabel }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>当前监测对象</span>
              <strong>{{ latestStationName }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>当前指标语义</span>
              <strong>{{ metricMeta.description }}</strong>
            </div>
          </div>
        </template>
        <div class="monitoring-overview-copy">
          <p class="monitoring-overview-copy__title">页面阅读顺序</p>
          <p>先看摘要判断当前监测规模和异常信号，再看主趋势分析区理解变化方向，最后在历史记录区核对采样明细与录入入口。</p>
        </div>
      </SideInfoPanel>
    </div>

    <ChartSection
      :title="`${metricMeta.label}趋势分析`"
      description="以当前筛选条件为上下文，先看趋势，再结合右侧摘要确认最近一次采样、变化幅度与风险状态。"
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
          <el-empty v-else description="当前筛选条件下暂无可用于趋势分析的历史数据" />
        </div>

        <div class="monitoring-analysis-layout__aside">
          <div :class="`entity-state-note entity-state-note--${trendNoteTone}`">
            <span class="entity-state-note__eyebrow">主趋势判断</span>
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

          <dl class="entity-detail-list monitoring-analysis-details">
            <div>
              <dt>指标说明</dt>
              <dd>{{ metricMeta.description }}</dd>
            </div>
            <div>
              <dt>筛选上下文</dt>
              <dd>{{ filterSummary }}</dd>
            </div>
            <div>
              <dt>读取提示</dt>
              <dd>若未显式指定指标类型，则页面会按当前返回结果中的主指标语义展示趋势，方便先完成展示层判断，再进入历史记录核对明细。</dd>
            </div>
          </dl>
        </div>
      </div>
    </ChartSection>

    <TableSection
      title="历史记录与采样明细"
      description="在主趋势下方继续核对站点、采样值与触发结果，形成监测分析与明细记录的自然闭环。"
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
            <h4>{{ dialogSections[0] }}</h4>
            <p>定义采样归属的站点、监测指标与采集时间。</p>
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
            <h4>{{ dialogSections[1] }}</h4>
            <p>录入本次采样的监测值，系统会沿用当前阈值规则进行风险判断。</p>
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
            <h4>{{ dialogSections[2] }}</h4>
            <p>补充记录采样背景、现场情况或需要在历史记录中保留的说明。</p>
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
import MetricCard from '@/components/common/MetricCard.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TableSection from '@/components/common/TableSection.vue'
import TrendLineChart from '@/components/charts/TrendLineChart.vue'
import { createMonitoringRecord, fetchMonitoringHistory, fetchMonitoringRecords, type MonitoringFormModel } from '@/api/modules/monitoring'
import { useStationOptions } from '@/composables/useStationOptions'
import { useAuthStore } from '@/stores/auth'
import { buildMonitoringOverview, getMonitoringDialogSections, getMonitoringMetricMeta } from './monitoringPresentation'
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
  if (monitoringOverview.value.triggeredCount > 0) return '当前趋势需结合预警样本重点关注'
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

function formatDateTime(value?: string) {
  return value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '--'
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
.monitoring-overview-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.4fr) minmax(320px, 0.6fr);
  gap: 24px;
}

.monitoring-overview-grid__cards {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 24px;
}

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
