<template>
  <div class="page-shell">
    <PageCard title="站点管理" subtitle="支持站点类型、状态、经纬度和父级工程维护">
      <template #extra>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">新增站点</el-button>
      </template>

      <div class="toolbar toolbar--station">
        <el-input v-model="query.keyword" placeholder="搜索站点名称" clearable @keyup.enter="loadData" @clear="loadData" />
        <el-select v-model="query.type" placeholder="站点类型" clearable>
          <el-option v-for="item in typeOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-select v-model="query.status" placeholder="站点状态" clearable>
          <el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-button type="primary" @click="loadData">查询</el-button>
      </div>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="name" label="站点名称" min-width="150" />
        <el-table-column prop="type" label="类型" width="120" />
        <el-table-column label="状态" width="120">
          <template #default="{ row }">
            <el-tag :type="statusTagType(row.status)">{{ row.status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="parentName" label="归属工程" min-width="160" />
        <el-table-column label="坐标" min-width="180">
          <template #default="{ row }">{{ row.latitude }}, {{ row.longitude }}</template>
        </el-table-column>
        <el-table-column label="阈值" min-width="150">
          <template #default="{ row }">{{ row.warningThreshold }} / {{ row.criticalThreshold }}</template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="showDetail(row.id)">详情</el-button>
            <el-button v-if="isAdmin" link type="primary" @click="openEditDialog(row.id)">编辑</el-button>
            <el-button v-if="isAdmin" link type="danger" @click="removeRow(row.id)">删除</el-button>
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

    <el-dialog v-model="dialogVisible" :title="editingId ? '编辑站点' : '新增站点'" width="760px">
      <el-form :model="form" label-position="top">
        <el-row :gutter="16">
          <el-col :span="12"><el-form-item label="站点名称"><el-input v-model="form.name" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="站点类型"><el-select v-model="form.type" class="w-full"><el-option v-for="item in typeOptions" :key="item.value" :label="item.label" :value="item.value" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="运行状态"><el-select v-model="form.status" class="w-full"><el-option v-for="item in statusOptions" :key="item.value" :label="item.label" :value="item.value" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="最近活跃时间"><el-date-picker v-model="form.lastActiveAt" type="datetime" value-format="YYYY-MM-DDTHH:mm:ss" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="纬度"><el-input-number v-model="form.latitude" :precision="6" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="经度"><el-input-number v-model="form.longitude" :precision="6" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="预警阈值"><el-input-number v-model="form.warningThreshold" :precision="2" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="严重阈值"><el-input-number v-model="form.criticalThreshold" :precision="2" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="归属水库"><el-select v-model="form.reservoirId" clearable class="w-full"><el-option v-for="item in reservoirOptions" :key="item.id" :label="item.name" :value="item.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="归属河道"><el-select v-model="form.riverId" clearable class="w-full"><el-option v-for="item in riverOptions" :key="item.id" :label="item.name" :value="item.id" /></el-select></el-form-item></el-col>
          <el-col :span="24"><el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="4" /></el-form-item></el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">保存</el-button>
      </template>
    </el-dialog>

    <el-drawer v-model="detailVisible" title="站点详情" size="460px">
      <el-descriptions v-if="detail" :column="1" border>
        <el-descriptions-item label="站点名称">{{ detail.name }}</el-descriptions-item>
        <el-descriptions-item label="站点类型">{{ detail.type }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ detail.status }}</el-descriptions-item>
        <el-descriptions-item label="归属水库">{{ detail.reservoirName || '--' }}</el-descriptions-item>
        <el-descriptions-item label="归属河道">{{ detail.riverName || '--' }}</el-descriptions-item>
        <el-descriptions-item label="坐标">{{ detail.latitude }}, {{ detail.longitude }}</el-descriptions-item>
        <el-descriptions-item label="阈值">{{ detail.warningThreshold }} / {{ detail.criticalThreshold }}</el-descriptions-item>
        <el-descriptions-item label="描述">{{ detail.description }}</el-descriptions-item>
      </el-descriptions>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageCard from '@/components/common/PageCard.vue'
import { fetchReservoirs } from '@/api/modules/reservoir'
import { fetchRivers } from '@/api/modules/river'
import { createStation, deleteStation, fetchStationDetail, fetchStations, updateStation, type StationFormModel } from '@/api/modules/station'
import { useAuthStore } from '@/stores/auth'
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

function statusTagType(status: string) {
  if (status === 'Online') return 'success'
  if (status === 'Offline') return 'info'
  return 'warning'
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
.toolbar--station {
  grid-template-columns: 1.2fr 0.7fr 0.7fr auto;
}

@media (max-width: 1080px) {
  .toolbar--station {
    grid-template-columns: 1fr;
  }
}
</style>
