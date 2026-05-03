<template>
  <div class="page-shell">
    <PageHeader title="新建审批申请" subtitle="填写取水许可申请信息并上传相关附件" />

    <el-card shadow="never" class="form-card">
      <el-form ref="formRef" :model="form" :rules="rules" label-position="top" class="entity-form">
        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>申请人信息</h4>
            <p>填写申请人或申请企业的基本信息</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="申请人姓名" prop="applicantName">
                <el-input v-model="form.applicantName" placeholder="请输入申请人姓名" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="证件号码" prop="applicantIdCard">
                <el-input v-model="form.applicantIdCard" placeholder="请输入身份证号" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="企业名称">
                <el-input v-model="form.enterpriseName" placeholder="如有企业请输入企业名称" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="营业执照号">
                <el-input v-model="form.enterpriseLicenseNo" placeholder="如有请输入营业执照号" />
              </el-form-item>
            </el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>取水信息</h4>
            <p>填写取水地点、用途和申请水量</p>
          </div>
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="取水地点" prop="waterIntakeLocation">
                <el-input v-model="form.waterIntakeLocation" placeholder="请输入取水地点" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="取水用途" prop="waterIntakePurpose">
                <el-select v-model="form.waterIntakePurpose" placeholder="请选择取水用途" class="w-full">
                  <el-option label="工业用水" value="工业用水" />
                  <el-option label="农业灌溉" value="农业灌溉" />
                  <el-option label="生活用水" value="生活用水" />
                  <el-option label="生态用水" value="生态用水" />
                  <el-option label="其他" value="其他" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="申请取水量(m³)" prop="waterIntakeAmount">
                <el-input-number v-model="form.waterIntakeAmount" :min="0" :precision="2" class="w-full" />
              </el-form-item>
            </el-col>
          </el-row>
        </section>

        <section class="entity-form__section">
          <div class="entity-form__section-head">
            <h4>附件上传</h4>
            <p>上传申请书、身份证、营业执照等材料</p>
          </div>

          <div class="attachment-grid">
            <div v-for="slot in attachmentSlots" :key="slot.type" class="attachment-slot">
              <h5>{{ slot.label }}</h5>
              <p>{{ slot.description }}</p>
              <el-upload
                :auto-upload="false"
                :limit="1"
                :on-change="(file: any) => handleFileSelect(slot.type, file)"
                accept=".pdf,.docx,.doc,.jpg,.jpeg,.png"
              >
                <el-button size="small">选择文件</el-button>
              </el-upload>
              <span v-if="selectedFiles[slot.type]" class="file-name">{{ selectedFiles[slot.type]?.name }}</span>
            </div>
          </div>
        </section>
      </el-form>

      <div class="form-actions">
        <el-button @click="$router.back()">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">创建申请</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import PageHeader from '@/components/common/PageHeader.vue'
import { createApproval, uploadAttachment } from '@/api/modules/approval'
import type { ApprovalFormModel } from '@/types/approval'

const router = useRouter()
const formRef = ref<FormInstance>()
const submitting = ref(false)

const form = reactive<ApprovalFormModel>({
  applicantName: '',
  applicantIdCard: '',
  enterpriseName: '',
  enterpriseLicenseNo: '',
  waterIntakeLocation: '',
  waterIntakePurpose: '',
  waterIntakeAmount: 0,
})

const rules: FormRules = {
  applicantName: [{ required: true, message: '请输入申请人姓名', trigger: 'blur' }],
  applicantIdCard: [{ required: true, message: '请输入证件号码', trigger: 'blur' }],
  waterIntakeLocation: [{ required: true, message: '请输入取水地点', trigger: 'blur' }],
  waterIntakePurpose: [{ required: true, message: '请选择取水用途', trigger: 'change' }],
  waterIntakeAmount: [{ required: true, message: '请输入申请取水量', trigger: 'blur' }],
}

const attachmentSlots = [
  { type: 'application_form', label: '申请书', description: '取水许可申请书（PDF/Word）' },
  { type: 'id_card', label: '身份证', description: '申请人身份证件（JPG/PNG/PDF）' },
  { type: 'business_license', label: '营业执照', description: '企业营业执照（JPG/PNG/PDF）' },
  { type: 'other', label: '其他材料', description: '水资源论证报告等' },
]

const selectedFiles = reactive<Record<string, File | null>>({
  application_form: null,
  id_card: null,
  business_license: null,
  other: null,
})

function handleFileSelect(type: string, file: any) {
  selectedFiles[type] = file.raw
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const result = await createApproval(form)

    // 上传附件
    for (const [type, file] of Object.entries(selectedFiles)) {
      if (file) {
        await uploadAttachment(result.id, file, type)
      }
    }

    ElMessage.success('审批申请已创建')
    router.push('/approvals')
  } catch {
    ElMessage.error('创建失败')
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped lang="scss">
.form-card {
  max-width: 960px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 24px;
  border-top: 1px solid var(--el-border-color-lighter);
}

.attachment-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
}

.attachment-slot {
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 8px;
  padding: 16px;

  h5 {
    margin: 0 0 4px;
    font-size: 14px;
  }

  p {
    margin: 0 0 12px;
    font-size: 12px;
    color: var(--el-text-color-secondary);
  }
}

.file-name {
  display: block;
  margin-top: 8px;
  font-size: 12px;
  color: var(--el-color-success);
}
</style>
