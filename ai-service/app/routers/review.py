from fastapi import APIRouter, HTTPException

from app.models.schemas import (
    ReviewSubmitRequest,
    ReviewResultResponse,
    ReviewFindingResult,
    CheckCompletenessRequest,
    CheckCompletenessResponse,
)
from app.services.review_agent import review_agent

router = APIRouter()


@router.post("/submit", response_model=ReviewResultResponse)
async def submit_review(request: ReviewSubmitRequest):
    """提交审批申请进行 Agent 合规审查。"""
    try:
        result = await review_agent.review(request)
        return result
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"审查失败: {str(e)}")


@router.get("/result/{application_id}", response_model=ReviewResultResponse)
async def get_review_result(application_id: str):
    """获取审查结果（从缓存或重新审查）。"""
    result = review_agent.get_cached_result(application_id)
    if not result:
        raise HTTPException(status_code=404, detail="未找到该申请的审查结果")
    return result


@router.post("/check-completeness", response_model=CheckCompletenessResponse)
async def check_completeness(request: CheckCompletenessRequest):
    """检查材料完整性。"""
    result = review_agent.check_materials_completeness(request.materials)
    return result
