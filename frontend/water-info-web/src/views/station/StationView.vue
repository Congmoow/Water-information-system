<template>
  <div class="page-shell">
    <TableSection
      title="监测站点"
      description="维护站点档案、运行状态、阈值配置。"
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
            <h4>{{ stationFormSections.basic.title }}</h4>
            <p>{{ stationFormSections.basic.description }}</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12"><el-form-item label="站点名称"><el-input v-model="form.name" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="站点类型"><el-select v-model="form.type" class="w-full"><el-option v-for="item in typeOptions" :key="item.value" :label="item.label" :value="item.value" /></el-select></el-form-item></el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ stationFormSections.status.title }}</h4>
            <p>{{ stationFormSections.status.description }}</p>
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
            <h4>{{ stationFormSections.affiliation.title }}</h4>
            <p>{{ stationFormSections.affiliation.description }}</p>
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
            <h4>{{ stationFormSections.supplement.title }}</h4>
            <p>{{ stationFormSections.supplement.description }}</p>
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
import { computed, onMounted, ref } from 'vue'
import FilterBar from '@/components/common/FilterBar.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import StatusTag from '@/components/common/StatusTag.vue'
import TableSection from '@/components/common/TableSection.vue'
import { fetchReservoirs } from '@/api/modules/reservoir'
import { fetchRivers } from '@/api/modules/river'
import { createStation, deleteStation, fetchStationDetail, fetchStations, updateStation, type StationFormModel } from '@/api/modules/station'
import { useCrudPage } from '@/composables/useCrudPage'
import { buildStationOverview, getStationFormSections, getStationStatusInsight, stationTypeLabel } from './stationPresentation'
import type { ReservoirItem, RiverItem, StationDetail, StationItem } from '@/types/models'
import { formatDateTime } from '@/utils/format'
import { DEFAULT_LATITUDE, DEFAULT_LONGITUDE } from '@/constants/coordinates'

const {
  isAdmin,
  loading,
  submitting,
  dialogVisible,
  detailVisible,
  editingId,
  detail,
  rows,
  total,
  query,
  form,
  loadData,
  handlePageChange,
  openCreateDialog,
  openEditDialog,
  submitForm,
  removeRow,
  showDetail
} = useCrudPage<StationItem, StationDetail, StationFormModel, { page: number; pageSize: number; keyword: string; type: string; status: string }>({
  api: {
    fetchList: fetchStations,
    fetchDetail: fetchStationDetail,
    create: createStation,
    update: updateStation,
    remove: deleteStation
  },
  initialQuery: () => ({ page: 1, pageSize: 10, keyword: '', type: '', status: '' }),
  initialForm: () => ({
    name: '',
    type: 'WaterLevel',
    longitude: DEFAULT_LONGITUDE,
    latitude: DEFAULT_LATITUDE,
    status: 'Online',
    warningThreshold: 18.5,
    criticalThreshold: 20,
    description: '',
    lastActiveAt: '',
    reservoirId: '',
    riverId: ''
  }),
  deleteConfirmMessage: '确认删除该站点记录吗？',
  createSuccessMessage: '站点已新增',
  updateSuccessMessage: '站点信息已更新',
  deleteSuccessMessage: '删除成功',
  createDialogTitle: '新增监测站',
  editDialogTitle: '维护站点资料',
  mapDetailToForm: (detail, form) => {
    Object.assign(form, {
      ...detail,
      reservoirId: detail.reservoirId || '',
      riverId: detail.riverId || ''
    })
  },
  mapFormToPayload: (form) => ({
    ...form,
    reservoirId: form.reservoirId || undefined,
    riverId: form.riverId || undefined,
    lastActiveAt: form.lastActiveAt || undefined
  })
})

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
const overview = computed(() => buildStationOverview(rows.value))
const stationInsight = computed(() => (detail.value ? getStationStatusInsight(detail.value) : null))
const stationFormSections = getStationFormSections()

function statusLabel(value: string) {
  return statusOptions.find((item) => item.value === value)?.label ?? value
}

async function loadOptions() {
  const [reservoirResult, riverResult] = await Promise.all([
    fetchReservoirs({ page: 1, pageSize: 50 }),
    fetchRivers({ page: 1, pageSize: 50 })
  ])
  reservoirOptions.value = reservoirResult.items
  riverOptions.value = riverResult.items
}

onMounted(loadOptions)
</script>

<style scoped lang="scss">
.station-panel-meta {
  margin-top: 16px;
}
</style>
