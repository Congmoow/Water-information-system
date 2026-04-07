<template>
  <div class="page-shell">
    <PageCard title="河道管理" subtitle="支持新增、编辑、删除、查询和详情查看">
      <template #extra>
        <el-button v-if="isAdmin" type="primary" @click="openCreateDialog">新增河道</el-button>
      </template>

      <div class="toolbar">
        <el-input v-model="query.keyword" placeholder="按名称或流域搜索" clearable @keyup.enter="loadData" @clear="loadData" />
        <el-button type="primary" @click="loadData">查询</el-button>
      </div>

      <el-table :data="rows" v-loading="loading" border>
        <el-table-column prop="name" label="名称" min-width="140" />
        <el-table-column prop="basin" label="流域" min-width="160" />
        <el-table-column prop="length" label="长度(km)" width="120" />
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

    <el-dialog v-model="dialogVisible" :title="editingId ? '编辑河道' : '新增河道'" width="640px">
      <el-form :model="form" label-position="top">
        <el-row :gutter="16">
          <el-col :span="12"><el-form-item label="名称"><el-input v-model="form.name" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="流域"><el-input v-model="form.basin" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="长度(km)"><el-input-number v-model="form.length" :min="0" :precision="2" class="w-full" /></el-form-item></el-col>
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

    <el-drawer v-model="detailVisible" title="河道详情" size="460px">
      <el-descriptions v-if="detail" :column="1" border>
        <el-descriptions-item label="名称">{{ detail.name }}</el-descriptions-item>
        <el-descriptions-item label="流域">{{ detail.basin }}</el-descriptions-item>
        <el-descriptions-item label="长度">{{ detail.length }}</el-descriptions-item>
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
import { createRiver, deleteRiver, fetchRiverDetail, fetchRivers, updateRiver, type RiverFormModel } from '@/api/modules/river'
import { useAuthStore } from '@/stores/auth'
import type { RiverDetail, RiverItem } from '@/types/models'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Administrator')
const loading = ref(false)
const submitting = ref(false)
const dialogVisible = ref(false)
const detailVisible = ref(false)
const editingId = ref<string | null>(null)
const detail = ref<RiverDetail | null>(null)
const rows = ref<RiverItem[]>([])
const total = ref(0)
const query = reactive({ page: 1, pageSize: 10, keyword: '' })
const form = reactive<RiverFormModel>({
  name: '',
  length: 0,
  basin: '',
  latitude: 30.595,
  longitude: 114.398,
  description: ''
})

function resetForm() {
  Object.assign(form, { name: '', length: 0, basin: '', latitude: 30.595, longitude: 114.398, description: '' })
}

async function loadData() {
  loading.value = true
  try {
    const result = await fetchRivers(query)
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
  const result = await fetchRiverDetail(id)
  editingId.value = id
  Object.assign(form, result)
  dialogVisible.value = true
}

async function submitForm() {
  submitting.value = true
  try {
    if (editingId.value) {
      await updateRiver(editingId.value, form)
      ElMessage.success('河道信息已更新')
    } else {
      await createRiver(form)
      ElMessage.success('河道已新增')
    }
    dialogVisible.value = false
    await loadData()
  } finally {
    submitting.value = false
  }
}

async function removeRow(id: string) {
  await ElMessageBox.confirm('确认删除该河道记录吗？', '删除确认', { type: 'warning' })
  await deleteRiver(id)
  ElMessage.success('删除成功')
  await loadData()
}

async function showDetail(id: string) {
  detail.value = await fetchRiverDetail(id)
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
