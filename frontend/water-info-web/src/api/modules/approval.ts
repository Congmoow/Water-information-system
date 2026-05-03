import http from '@/api/http'
import type { ApiResponse, PagedResult } from '@/types/common'
import type { ApprovalItem, ApprovalDetail, ApprovalFormModel, ApprovalQuery } from '@/types/approval'

export async function fetchApprovals(params: ApprovalQuery) {
  const response = await http.get<ApiResponse<PagedResult<ApprovalItem>>>('/api/approval', { params })
  return response.data.data
}

export async function fetchApprovalDetail(id: string) {
  const response = await http.get<ApiResponse<ApprovalDetail>>(`/api/approval/${id}`)
  return response.data.data
}

export async function createApproval(payload: ApprovalFormModel) {
  const response = await http.post<ApiResponse<ApprovalDetail>>('/api/approval', payload)
  return response.data.data
}

export async function submitForReview(id: string) {
  const response = await http.post<ApiResponse<ApprovalDetail>>(`/api/approval/${id}/submit-review`)
  return response.data.data
}

export async function uploadAttachment(applicationId: string, file: File, attachmentType: string) {
  const formData = new FormData()
  formData.append('file', file)
  formData.append('fileName', file.name)
  formData.append('fileType', file.name.split('.').pop() || '')
  formData.append('attachmentType', attachmentType)

  const response = await http.post<ApiResponse<ApprovalDetail>>(
    `/api/approval/${applicationId}/attachments`,
    formData,
    { headers: { 'Content-Type': 'multipart/form-data' } }
  )
  return response.data.data
}

export async function deleteApproval(id: string) {
  await http.delete(`/api/approval/${id}`)
}
