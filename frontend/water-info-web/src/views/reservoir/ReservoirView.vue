<template>
  <div class="page-shell">
    <TableSection
      title="水库资产"
      description="维护水库档案、容量信息、空间位置和管理单位。"
      :loading="loading"
      :has-data="rows.length > 0"
      :total="total"
      empty-description="暂无水库数据"
    >
      <template #actions>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">新增水库资产</el-button>
      </template>

      <FilterBar class="toolbar">
        <el-input v-model="query.keyword" placeholder="按名称、位置或管理单位搜索" clearable @keyup.enter="loadData" @clear="loadData" />
        <template #actions>
          <el-button type="primary" @click="loadData">查询</el-button>
        </template>
      </FilterBar>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="name" label="水库名称" min-width="160" />
        <el-table-column prop="location" label="所在位置" min-width="180" />
        <el-table-column prop="capacity" label="容量(万m³)" width="120" />
        <el-table-column prop="managementUnit" label="管理单位" min-width="160" />
        <el-table-column prop="updatedAt" label="最近更新" min-width="170">
          <template #default="{ row }">{{ formatDateTime(row.updatedAt) }}</template>
        </el-table-column>
        <el-table-column label="坐标" min-width="180">
          <template #default="{ row }">{{ row.latitude }}, {{ row.longitude }}</template>
        </el-table-column>
        <el-table-column label="操作" width="260" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="showDetail(row.id)">查看档案</el-button>
            <el-button v-if="isAdmin" link type="primary" @click="openEditDialog(row.id)">维护资料</el-button>
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

    <el-dialog v-model="dialogVisible" :title="editingId ? '维护水库档案' : '新增水库资产'" width="640px">
      <el-form :model="form" label-position="top" class="entity-form">
        <div class="entity-form__intro">
          <strong>{{ editingId ? '更新水库基础档案' : '创建新的水库资产对象' }}</strong>
          <p>请按基础信息、空间信息和补充说明顺序完善档案，保持资产对象信息稳定完整。</p>
        </div>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ formSections.basic.title }}</h4>
            <p>{{ formSections.basic.description }}</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12"><el-form-item label="名称"><el-input v-model="form.name" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="所在位置"><el-input v-model="form.location" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="容量(万m³)"><el-input-number v-model="form.capacity" :min="0" :precision="2" class="w-full" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="管理单位"><el-input v-model="form.managementUnit" /></el-form-item></el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ formSections.spatial.title }}</h4>
            <p>{{ formSections.spatial.description }}</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12"><el-form-item label="纬度"><el-input-number v-model="form.latitude" :precision="6" class="w-full" /></el-form-item></el-col>
            <el-col :span="12"><el-form-item label="经度"><el-input-number v-model="form.longitude" :precision="6" class="w-full" /></el-form-item></el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>{{ formSections.supplement.title }}</h4>
            <p>{{ formSections.supplement.description }}</p>
          </div>
          <el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="4" /></el-form-item>
        </section>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">{{ editingId ? '保存更新' : '创建资产' }}</el-button>
      </template>
    </el-dialog>

    <el-drawer v-model="detailVisible" title="水库详情" size="460px">
      <SideInfoPanel
        v-if="detail"
        :title="detail.name"
        subtitle="水库资产档案"
      >
        <template #status>
          <span class="entity-pill">资产对象</span>
        </template>
        <template #meta>
          <div class="entity-state-note entity-state-note--info">
            <span class="entity-state-note__eyebrow">档案摘要</span>
            <strong>{{ panelMeta.heading }}</strong>
            <p>{{ panelMeta.description }}</p>
          </div>
          <div class="entity-facts-grid entity-facts-grid--2 reservoir-panel-meta">
            <div v-for="fact in panelMeta.keyFacts" :key="fact.label" class="entity-facts-grid__item">
              <span>{{ fact.label }}</span>
              <strong>{{ fact.value }}</strong>
            </div>
          </div>
        </template>
        <dl class="entity-detail-list">
          <div>
            <dt>中心坐标</dt>
            <dd>{{ detail.latitude }}, {{ detail.longitude }}</dd>
          </div>
          <div>
            <dt>最近更新</dt>
            <dd>{{ formatDateTime(detail.updatedAt) }}</dd>
          </div>
          <div>
            <dt>补充说明</dt>
            <dd>{{ detail.description || '暂无补充说明' }}</dd>
          </div>
        </dl>
        <template #footer>
          <div class="entity-panel-footer">
            <div>
              <span>对象类型</span>
              <strong>水库资产档案</strong>
            </div>
            <el-button v-if="isAdmin" type="primary" @click="openEditDialog(detail.id)">维护资料</el-button>
          </div>
        </template>
      </SideInfoPanel>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import FilterBar from '@/components/common/FilterBar.vue'
import SideInfoPanel from '@/components/common/SideInfoPanel.vue'
import TableSection from '@/components/common/TableSection.vue'
import {
  createReservoir,
  deleteReservoir,
  fetchReservoirDetail,
  fetchReservoirs,
  updateReservoir,
  type ReservoirFormModel
} from '@/api/modules/reservoir'
import { useCrudPage } from '@/composables/useCrudPage'
import { buildReservoirOverview, getReservoirFormSections, getReservoirPanelMeta } from './reservoirPresentation'
import type { ReservoirDetail, ReservoirItem } from '@/types/models'
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
} = useCrudPage<ReservoirItem, ReservoirDetail, ReservoirFormModel, { page: number; pageSize: number; keyword: string }>({
  api: {
    fetchList: fetchReservoirs,
    fetchDetail: fetchReservoirDetail,
    create: createReservoir,
    update: updateReservoir,
    remove: deleteReservoir
  },
  initialQuery: () => ({ page: 1, pageSize: 10, keyword: '' }),
  initialForm: () => ({
    name: '',
    location: '',
    capacity: 0,
    managementUnit: '',
    latitude: DEFAULT_LATITUDE,
    longitude: DEFAULT_LONGITUDE,
    description: ''
  }),
  deleteConfirmMessage: '确认删除该水库记录吗？',
  createSuccessMessage: '水库已新增',
  updateSuccessMessage: '水库信息已更新',
  deleteSuccessMessage: '删除成功',
  createDialogTitle: '新增水库资产',
  editDialogTitle: '维护水库档案'
})

const overview = computed(() => buildReservoirOverview(rows.value))
const formSections = getReservoirFormSections()
const panelMeta = computed(() => detail.value ? getReservoirPanelMeta(detail.value) : getReservoirPanelMeta({
  id: '',
  name: '',
  location: '',
  capacity: 0,
  managementUnit: '',
  latitude: 0,
  longitude: 0,
  description: '',
  updatedAt: '',
  createdAt: ''
}))
</script>

<style scoped lang="scss">
.reservoir-panel-meta {
  margin-top: 16px;
}
</style>
