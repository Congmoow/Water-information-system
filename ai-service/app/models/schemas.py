from pydantic import BaseModel, Field
from typing import Optional
from datetime import datetime
from enum import Enum


class ApprovalStatus(str, Enum):
    PENDING = "Pending"
    REVIEWING = "Reviewing"
    APPROVED = "Approved"
    REJECTED = "Rejected"


class Severity(str, Enum):
    ERROR = "Error"
    WARNING = "Warning"
    INFO = "Info"


class FindingCategory(str, Enum):
    FORM_CHECK = "形式审查"
    CONTENT规范 = "内容规范"
    SUBSTANTIVE = "实质合规"
    RELEVANCE = "关联性"


# --- Request Models ---

class KnowledgeUploadRequest(BaseModel):
    file_path: str = Field(..., description="知识库文档路径")


class KnowledgeSearchRequest(BaseModel):
    query: str = Field(..., description="检索查询语句")
    top_k: int = Field(default=5, ge=1, le=20, description="返回结果数量")


class MaterialItem(BaseModel):
    name: str = Field(..., description="材料名称")
    file_type: str = Field(..., description="文件类型")
    file_path: str = Field(..., description="文件路径")
    attachment_type: str = Field(..., description="附件类型")


class ReviewSubmitRequest(BaseModel):
    application_id: str = Field(..., description="审批申请 ID")
    applicant_name: str = Field(..., description="申请人姓名")
    applicant_id_card: str = Field(..., description="证件号")
    enterprise_name: Optional[str] = Field(None, description="企业名称")
    enterprise_license_no: Optional[str] = Field(None, description="营业执照号")
    water_intake_location: str = Field(..., description="取水地点")
    water_intake_purpose: str = Field(..., description="取水用途")
    water_intake_amount: float = Field(..., description="申请取水量")
    materials: list[MaterialItem] = Field(default_factory=list, description="申请材料列表")


class CheckCompletenessRequest(BaseModel):
    materials: list[str] = Field(..., description="材料名称列表")


# --- Response Models ---

class SearchHit(BaseModel):
    content: str = Field(..., description="文档片段内容")
    source: str = Field(..., description="来源文档")
    score: float = Field(..., description="相似度分数")
    metadata: dict = Field(default_factory=dict, description="额外元数据")


class KnowledgeSearchResponse(BaseModel):
    query: str
    results: list[SearchHit]
    total: int


class KnowledgeStatsResponse(BaseModel):
    total_documents: int
    total_chunks: int
    collection_name: str


class ReviewFindingResult(BaseModel):
    category: str = Field(..., description="问题类别")
    severity: str = Field(..., description="严重程度")
    description: str = Field(..., description="问题描述")
    suggestion: Optional[str] = Field(None, description="修改建议")


class ReviewResultResponse(BaseModel):
    application_id: str
    is_passed: bool
    summary: str
    findings: list[ReviewFindingResult]
    reviewed_at: str


class CheckCompletenessResponse(BaseModel):
    required_materials: list[str]
    provided_materials: list[str]
    missing_materials: list[str]
    is_complete: bool


class HealthResponse(BaseModel):
    status: str = "ok"
    app_name: str
    version: str
