export interface ApprovalItem {
  id: string
  applicantName: string
  applicantIdCard: string
  enterpriseName?: string
  waterIntakeLocation: string
  waterIntakePurpose: string
  waterIntakeAmount: number
  applicationDate: string
  status: string
  updatedAt: string
}

export interface ApprovalAttachment {
  id: string
  fileName: string
  fileType: string
  attachmentType: string
  createdAt: string
}

export interface ReviewFinding {
  id: string
  category: string
  severity: string
  description: string
  suggestion?: string
}

export interface ReviewResult {
  id: string
  isPassed: boolean
  summary: string
  reviewedAt: string
  agentVersion?: string
  findings: ReviewFinding[]
}

export interface ApprovalDetail extends ApprovalItem {
  enterpriseLicenseNo?: string
  attachments: ApprovalAttachment[]
  reviewResults: ReviewResult[]
  createdAt: string
}

export interface ApprovalFormModel {
  applicantName: string
  applicantIdCard: string
  enterpriseName?: string
  enterpriseLicenseNo?: string
  waterIntakeLocation: string
  waterIntakePurpose: string
  waterIntakeAmount: number
}

export interface ApprovalQuery {
  page?: number
  pageSize?: number
  keyword?: string
  status?: string
}
