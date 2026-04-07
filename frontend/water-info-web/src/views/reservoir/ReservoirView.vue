<template>
  <div class="page-shell">
    <PageCard title="水库管理" subtitle="支持新增、编辑、删除、查询和详情查看">
      <template #extra>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">新增水库</el-button>
      </template>

      <div class="toolbar">
        <el-input v-model="query.keyword" placeholder="按名称、位置或管理单位搜索" clearable @keyup.enter="loadData" @clear="loadData" />
        <el-button type="primary" @click="loadData">查询</el-button>
      </div>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="name" label="名称" min-width="140" />
        <el-table-column prop="location" label="所在位置" min-width="180" />
        <el-table-column prop="capacity" label="容量(万m³)" width="120" />
        <el-table-column prop="managementUnit" label="管理单位" min-width="160" />
        <el-table-column label="坐标" min-width="180">
          <template #default="{ row }">{{ row.latitude }}, {{ row.longitude }}</template>
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

    <el-dialog v-model="dialogVisible" :title="editingId ? '编辑水库' : '新增水库'" width="640px">
      <el-form :model="form" label-position="top">
        <el-row :gutter="16">
          <el-col :span="12"><el-form-item label="名称"><el-input v-model="form.name" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="所在位置"><el-input v-model="form.location" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="容量(万m³)"><el-input-number v-model="form.capacity" :min="0" :precision="2" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="管理单位"><el-input v-model="form.managementUnit" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="纬度"><el-input-number v-model="form.latitude" :precision="6" class="w-full" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="经度"><el-input-number v-model="form.longitude" :precision="6" class="w-full" /></el-form-item></el-col>
          <el-col :span="24"><el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="4" /></el-form-item></el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="submitForm">保存</el-button>
      </template>
    </el-dialog>

    <el-drawer v-model="detailVisible" title="水库详情" size="460px">
      <el-descriptions v-if="detail" :column="1" border>
        <el-descriptions-item label="名称">{{ detail.name }}</el-descriptions-item>
        <el-descriptions-item label="位置">{{ detail.location }}</el-descriptions-item>
        <el-descriptions-item label="容量">{{ detail.capacity }}</el-descriptions-item>
        <el-descriptions-item label="管理单位">{{ detail.managementUnit }}</el-descriptions-item>
        <el-descriptions-item label="坐标">{{ detail.latitude }}, {{ detail.longitude }}</el-descriptions-item>
        <el-descriptions-item label="描述">{{ detail.description }}</el-descriptions-item>
      </el-descriptions>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageCard from '@/components/common/PageCard.vue'
import {
  createReservoir,
  deleteReservoir,
  fetchReservoirDetail,
  fetchReservoirs,
  updateReservoir,
  type ReservoirFormModel
} from '@/api/modules/reservoir'
import { useAuthStore } from '@/stores/auth'
import type { ReservoirDetail, ReservoirItem } from '@/types/models'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Administrator')
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const detailVisible = ref(false)
const editingId = ref<string | null>(null)
const detail = ref<ReservoirDetail | null>(null)
const rows = ref<ReservoirItem[]>([])
const total = ref(0)
const query = reactive({
  page: 1,
  pageSize: 10,
  keyword: ''
})
const form = reactive<ReservoirFormModel>({
  name: '',
  location: '',
  capacity: 0,
  managementUnit: '',
  latitude: 30.625,
  longitude: 114.347,
  description: ''
})

function resetForm() {
  Object.assign(form, {
    name: '',
    location: '',
    capacity: 0,
    managementUnit: '',
    latitude: 30.625,
    longitude: 114.347,
    description: ''
  })
}

async function loadData() {
  loading.value = true
  try {
    const result = await fetchReservoirs(query)
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
  const result = await fetchReservoirDetail(id)
  editingId.value = id
  Object.assign(form, result)
  dialogVisible.value = true
}

async function submitForm() {
  submitting.value = true
  try {
    if (editingId.value) {
      await updateReservoir(editingId.value, form)
      ElMessage.success('水库信息已更新')
    } else {
      await createReservoir(form)
      ElMessage.success('水库已新增')
    }

    dialogVisible.value = false
    await loadData()
  } finally {
    submitting.value = false
  }
}

async function removeRow(id: string) {
  await ElMessageBox.confirm('确认删除该水库记录吗？', '删除确认', { type: 'warning' })
  await deleteReservoir(id)
  ElMessage.success('删除成功')
  await loadData()
}

async function showDetail(id: string) {
  detail.value = await fetchReservoirDetail(id)
  detailVisible.value = true
}

onMounted(loadData)
</script>

<style scoped lang="scss">
.toolbar {
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 14px;
  margin-bottom: 18px;
}

.table-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 18px;
}

.w-full {
  width: 100%;
}
</style>
