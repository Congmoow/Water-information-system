import { computed, onMounted, reactive, ref, type Ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import type { PagedResult } from '@/types/common'

/**
 * CRUD 列表页面通用 API 接口形状。
 * TItem   — 列表行数据类型
 * TDetail — 详情数据类型
 * TForm   — 表单模型类型
 * TQuery  — 查询参数类型（必须包含 page / pageSize）
 */
export interface CrudApi<TItem, TDetail, TForm, TQuery extends { page?: number; pageSize?: number }> {
  fetchList: (params: TQuery) => Promise<PagedResult<TItem>>
  fetchDetail: (id: string) => Promise<TDetail>
  create: (payload: TForm) => Promise<unknown>
  update: (id: string, payload: TForm) => Promise<unknown>
  remove: (id: string) => Promise<unknown>
}

/**
 * CRUD 页面 composable 配置。
 */
export interface UseCrudPageOptions<
  TItem,
  TDetail,
  TForm extends object,
  TQuery extends { page?: number; pageSize?: number }
> {
  /** CRUD API 函数集 */
  api: CrudApi<TItem, TDetail, TForm, TQuery>
  /** 生成初始查询参数 */
  initialQuery: () => TQuery
  /** 生成表单初始值 */
  initialForm: () => TForm
  /** 删除确认提示文本 */
  deleteConfirmMessage: string
  /** 创建成功提示 */
  createSuccessMessage: string
  /** 更新成功提示 */
  updateSuccessMessage: string
  /** 删除成功提示 */
  deleteSuccessMessage: string
  /** 新建弹窗标题 */
  createDialogTitle: string
  /** 编辑弹窗标题 */
  editDialogTitle: string
  /**
   * openEditDialog 中从详情数据填充表单时的转换逻辑。
   * 默认行为是 Object.assign(form, detail)。
   */
  mapDetailToForm?: (detail: TDetail, form: TForm) => void
  /**
   * submitForm 中提交前对表单数据的转换逻辑。
   * 默认直接使用 form 作为 payload。
   */
  mapFormToPayload?: (form: TForm) => unknown
}

export function useCrudPage<
  TItem extends { id: string },
  TDetail extends { id: string },
  TForm extends object,
  TQuery extends { page?: number; pageSize?: number }
>(options: UseCrudPageOptions<TItem, TDetail, TForm, TQuery>) {
  const {
    api,
    initialQuery,
    initialForm,
    deleteConfirmMessage,
    createSuccessMessage,
    updateSuccessMessage,
    deleteSuccessMessage,
    mapDetailToForm,
    mapFormToPayload
  } = options

  const authStore = useAuthStore()
  const isAdmin = computed(() => authStore.user?.role === 'Administrator')

  const loading = ref(false)
  const submitting = ref(false)
  const dialogVisible = ref(false)
  const detailVisible = ref(false)
  const editingId = ref<string | null>(null)
  const detail = ref<TDetail | null>(null) as Ref<TDetail | null>
  const rows = ref<TItem[]>([]) as Ref<TItem[]>
  const total = ref(0)
  const query = reactive(initialQuery()) as TQuery
  const form = reactive(initialForm()) as TForm

  async function loadData() {
    loading.value = true
    try {
      const result = await api.fetchList(query)
      rows.value = result.items
      total.value = result.total
    } finally {
      loading.value = false
    }
  }

  function handlePageChange(page: number) {
    ;(query as { page: number }).page = page
    loadData()
  }

  function resetForm() {
    Object.assign(form as object, initialForm())
  }

  function openCreateDialog() {
    editingId.value = null
    resetForm()
    dialogVisible.value = true
  }

  async function openEditDialog(id: string) {
    const result = await api.fetchDetail(id)
    detailVisible.value = false
    editingId.value = id
    if (mapDetailToForm) {
      mapDetailToForm(result, form)
    } else {
      Object.assign(form as object, result)
    }
    dialogVisible.value = true
  }

  async function submitForm() {
    submitting.value = true
    try {
      const payload = mapFormToPayload ? mapFormToPayload(form) : form
      if (editingId.value) {
        await api.update(editingId.value, payload as TForm)
        ElMessage.success(updateSuccessMessage)
      } else {
        await api.create(payload as TForm)
        ElMessage.success(createSuccessMessage)
      }
      dialogVisible.value = false
      await loadData()
    } finally {
      submitting.value = false
    }
  }

  async function removeRow(id: string) {
    await ElMessageBox.confirm(deleteConfirmMessage, '删除确认', { type: 'warning' })
    await api.remove(id)
    ElMessage.success(deleteSuccessMessage)
    await loadData()
  }

  async function showDetail(id: string) {
    detail.value = await api.fetchDetail(id)
    detailVisible.value = true
  }

  onMounted(loadData)

  return {
    // 权限
    isAdmin,
    // 状态
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
    // 操作
    loadData,
    handlePageChange,
    resetForm,
    openCreateDialog,
    openEditDialog,
    submitForm,
    removeRow,
    showDetail
  }
}
