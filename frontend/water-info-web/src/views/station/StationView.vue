<template>
  <div class="page-shell">
    <div class="entity-summary-grid station-summary-grid">
      <MetricCard
        label="站点总数"
        :value="overview.total"
        description="当前列表中的监测站点数量。"
        highlight="监测节点"
        tone="info"
      />
      <MetricCard
        label="在线站点"
        :value="overview.onlineCount"
        description="当前在线且可持续采样的站点数量。"
        highlight="运行正常"
        tone="success"
      />
      <MetricCard
        label="预警站点"
        :value="overview.warningCount"
        description="当前处于预警状态的监测节点。"
        highlight="需关注"
        tone="warning"
      />
      <MetricCard
        label="离线站点"
        :value="overview.offlineCount"
        description="当前未在线的监测节点数量。"
        highlight="需排查"
        tone="info"
      />
    </div>

    <TableSection
      title="监测站点对象管理"
      description="统一维护站点档案、运行状态、阈值配置和归属工程关系。"
      :loading="loading"
      :has-data="rows.length > 0"
      :total="total"
      empty-description="暂无站点数据"
    >
      <template #actions>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">新增监测站</el-button>
      </template>

      <FilterBar class="toolbar toolbar--station">
        <el-input v-model="query.keyword" placeholder="搜索站点名称" clearable @keyup.enter="loadData" @clear="loadData" />
        <el-select v-model="query.type" placeholder="站点类型" clearable>
          <el-option v-for="item in typeOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-select v-model="query.status" placeholder="站点状态" clearable>
          <el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <template #actions>
          <el-button type="primary" @click="loadData">查询</el-button>
        </template>
      </FilterBar>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="name" label="站点名称" min-width="150" />
        <el-table-column label="类型" width="120">
          <template #default="{ row }">
            <span class="entity-pill">{{ stationTypeLabel(row.type) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="120">
          <template #default="{ row }">
            <StatusTag category="stationStatus" :value="row.status" />
          </template>
        </el-table-column>
        <el-table-column prop="parentName" label="归属工程" min-width="160" />
        <el-table-column label="最近活跃" min-width="170">
          <template #default="{ row }">{{ formatDateTime(row.lastActiveAt) }}</template>
        </el-table-column>
        <el-table-column label="坐标" min-width="180">
          <template #default="{ row }">{{ row.latitude }}, {{ row.longitude }}</template>
        </el-table-column>
        <el-table-column label="阈值" min-width="150">
          <template #default="{ row }">{{ row.warningThreshold }} / {{ row.criticalThreshold }}</template>
        </el-table-column>
        <el-table-column label="操作" width="260" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="showDetail(row.id)">查看面板</el-button>
            <el-button v-if="isAdmin" link type="primary" @click="openEditDialog(row.id)">维护站点</el-button>
            <el-button v-if="isAdmin" link type="danger" @click="removeRow(row.id)">删除记录</el-button>
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

    <el-dialog v-model="dialogVisible" :title="editingId ? '维护站点资料' : '新增监测站'" width="760px">
      <el-form :model="form" label-position="top" class="entity-form">
        <div class="entity-form__intro">
          <strong>{{ editingId ? '更新站点档案与运行配置' : '创建新的监测站点对象' }}</strong>
          <p>请先填写基础信息，再配置运行状态、阈值和归属关系，保持站点档案与运行状态同步。</p>
        </div>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ stationFormSections[0] }}</h4>
            <p>定义站点名称与监测类型，作为对象档案的基础标识。</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12"><el-form-item label="站点名称"><el-input v-model="form.name" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="站点类型"><el-select v-model="form.type" class="w-full"><el-option v-for="item in typeOptions" :key="item.value" :label="item.label" :value="item.value" /></el-select></el-form-item></el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ stationFormSections[1] }}</h4>
            <p>设置当前运行状态、最近活跃时间以及告警阈值。</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12"><el-form-item label="运行状态"><el-select v-model="form.status" class="w-full"><el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="item.value" /></el-select></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="最近活跃时间"><el-date-picker v-model="form.lastActiveAt" type="datetime" value-format="YYYY-MM-DDTHH:mm:ss" class="w-full" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="预警阈值"><el-input-number v-model="form.warningThreshold" :precision="2" class="w-full" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="严重阈值"><el-input-number v-model="form.criticalThreshold" :precision="2" class="w-full" /></el-form-item></el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ stationFormSections[2] }}</h4>
            <p>维护站点的空间坐标以及归属工程关系。</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12"><el-form-item label="纬度"><el-input-number v-model="form.latitude" :precision="6" class="w-full" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="经度"><el-input-number v-model="form.longitude" :precision="6" class="w-full" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="归属水库"><el-select v-model="form.reservoirId" clearable class="w-full"><el-option v-for="item in reservoirOptions" :key="item.id" :label="item.name" :value="item.id" /></el-select></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="归属河道"><el-select v-model="form.riverId" clearable class="w-full"><el-option v-for="item in riverOptions" :key="item.id" :label="item.name" :value="item.id" /></el-select></el-form-item></el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ stationFormSections[3] }}</h4>
            <p>补充说明当前站点用途、现场情况或维护备注。</p>
          </div>
          <el-form-item label="描述">
            <el-input v-model="form.description" type="textarea" :rows="4" />
          </el-form-item>
        </section>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">{{ editingId ? '保存更新' : '创建站点' }}</el-button>
      </template>
    </el-dialog>

    <el-drawer v-model="detailVisible" title="站点详情" size="460px">
      <SideInfoPanel
        v-if="detail"
        :title="detail.name"
        :subtitle="stationTypeLabel(detail.type)"
      >
        <template #status>
          <StatusTag category="stationStatus" :value="detail.status" />
        </template>
        <template #meta>
          <div :class="`entity-state-note entity-state-note--${stationInsight?.tone ?? 'info'}`">
            <span class="entity-state-note__eyebrow">运行摘要</span>
            <strong>{{ stationInsight?.heading }}</strong>
            <p>{{ stationInsight?.description }}</p>
          </div>
          <div class="entity-facts-grid entity-facts-grid--2 station-panel-meta">
            <div class="entity-facts-grid__item">
              <span>最近活跃</span>
              <strong>{{ formatDateTime(detail.lastActiveAt) }}</strong>
            </div>
            <div class="entity-facts-grid__item">
              <span>归属工程</span>
              <strong>{{ detail.reservoirName || detail.riverName || detail.parentName || '--' }}</strong>
            </div>
          </div>
        </template>
        <dl class="entity-detail-list">
          <div>
            <dt>告警阈值</dt>
            <dd>预警 {{ detail.warningThreshold }} / 严重 {{ detail.criticalThreshold }}</dd>
          </div>
          <div>
            <dt>坐标位置</dt>
            <dd>{{ detail.latitude }}, {{ detail.longitude }}</dd>
          </div>
          <div>
            <dt>归属水库</dt>
            <dd>{{ detail.reservoirName || '--' }}</dd>
          </div>
          <div>
            <dt>归属河道</dt>
            <dd>{{ detail.riverName || '--' }}</dd>
          </div>
          <div>
            <dt>补充说明</dt>
            <dd>{{ detail.description || '暂无补充说明' }}</dd>
          </div>
        </dl>
        <template #footer>
          <div class="entity-panel-footer">
            <div>
              <span>当前状态</span>
              <strong>{{ statusLabel(detail.status) }}</strong>
            </div>
            <el-button v-if="isAdmin" type="primary" @click="openEditDialog(detail.id)">维护站点</el-button>
          </div>
        </template>
      </SideInfoPanel>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import FilterBar from '@/components/common/FilterBar.vue'
import MetricCard from '@/components/common/MetricCard.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TableSection from '@/components/common/TableSection.vue'
import { fetchReservoirs } from '@/api/modules/reservoir'
import { fetchRivers } from '@/api/modules/river'
import { createStation, deleteStation, fetchStationDetail, fetchStations, updateStation, type StationFormModel } from '@/api/modules/station'
import { useAuthStore } from '@/stores/auth'
import { buildStationOverview, getStationFormSections, getStationStatusInsight, stationTypeLabel } from './stationPresentation'
import type { ReservoirItem, RiverItem, StationDetail, StationItem } from '@/types/models'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Administrator')
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const detailVisible = ref(false)
const editingId = ref<string | null>(null)
const detail = ref<StationDetail | null>(null)
const rows = ref<StationItem[]>([])
const total = ref(0)
const reservoirOptions = ref<ReservoirItem[]>([])
const riverOptions = ref<RiverItem[]>([])
const typeOptions = [
  { label: '水位站', value: 'WaterLevel' },
  { label: '雨量站', value: 'Rainfall' },
  { label: '流量站', value: 'Flow' }
]
const statusOptions = [
  { label: '在线', value: 'Online' },
  { label: '离线', value: 'Offline' },
  { label: '预警', value: 'Warning' }
]
const query = reactive({
  page: 1,
  pageSize: 10,
  keyword: '',
  type: '',
  status: ''
})
const form = reactive<StationFormModel>({
  name: '',
  type: 'WaterLevel',
  longitude: 114.347,
  latitude: 30.625,
  status: 'Online',
  warningThreshold: 18.5,
  criticalThreshold: 20,
  description: '',
  lastActiveAt: '',
  reservoirId: '',
  riverId: ''
})
const overview = computed(() => buildStationOverview(rows.value))
const stationInsight = computed(() => (detail.value ? getStationStatusInsight(detail.value) : null))
const stationFormSections = getStationFormSections()

function statusLabel(value: string) {
  return statusOptions.find((item) => item.value === value)?.label ?? value
}

function formatDateTime(value?: string) {
  if (!value) return '--'
  return value.replace('T', ' ').slice(0, 16)
}

function resetForm() {
  Object.assign(form, {
    name: '',
    type: 'WaterLevel',
    longitude: 114.347,
    latitude: 30.625,
    status: 'Online',
    warningThreshold: 18.5,
    criticalThreshold: 20,
    description: '',
    lastActiveAt: '',
    reservoirId: '',
    riverId: ''
  })
}

async function loadOptions() {
  const [reservoirResult, riverResult] = await Promise.all([
    fetchReservoirs({ page: 1, pageSize: 50 }),
    fetchRivers({ page: 1, pageSize: 50 })
  ])
  reservoirOptions.value = reservoirResult.items
  riverOptions.value = riverResult.items
}

async function loadData() {
  loading.value = true
  try {
    const result = await fetchStations({
      page: query.page,
      pageSize: query.pageSize,
      keyword: query.keyword || undefined,
      type: query.type || undefined,
      status: query.status || undefined
    })
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

function openCreateDialog() {
  editingId.value = null
  resetForm()
  dialogVisible.value = true
}

async function openEditDialog(id: string) {
  const result = await fetchStationDetail(id)
  detailVisible.value = false
  editingId.value = id
  Object.assign(form, {
    ...result,
    reservoirId: result.reservoirId || '',
    riverId: result.riverId || ''
  })
  dialogVisible.value = true
}

async function submitForm() {
  submitting.value = true
  try {
    const payload = {
      ...form,
      reservoirId: form.reservoirId || undefined,
      riverId: form.riverId || undefined,
      lastActiveAt: form.lastActiveAt || undefined
    }

    if (editingId.value) {
      await updateStation(editingId.value, payload)
      ElMessage.success('站点信息已更新')
    } else {
      await createStation(payload)
      ElMessage.success('站点已新增')
    }

    dialogVisible.value = false
    await loadData()
  } finally {
    submitting.value = false
  }
}

async function removeRow(id: string) {
  await ElMessageBox.confirm('确认删除该站点记录吗？', '删除确认', { type: 'warning' })
  await deleteStation(id)
  ElMessage.success('删除成功')
  await loadData()
}

async function showDetail(id: string) {
  detail.value = await fetchStationDetail(id)
  detailVisible.value = true
}

onMounted(async () => {
  await Promise.all([loadOptions(), loadData()])
})
</script>

<style scoped lang="scss">
.station-summary-grid {
  grid-template-columns: repeat(4, minmax(0, 1fr));
}

.station-panel-meta {
  margin-top: 16px;
}
</style>
