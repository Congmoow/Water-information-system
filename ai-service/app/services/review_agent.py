"""合规审查 Agent：基于 LangChain 的涉水审批材料合规性审查。"""

from __future__ import annotations

import json
from datetime import datetime, timezone

from app.models.schemas import (
    ReviewSubmitRequest,
    ReviewResultResponse,
    ReviewFindingResult,
    CheckCompletenessResponse,
)
from app.services.knowledge_store import knowledge_store
from app.services.document_parser import document_parser

# 必备材料清单
REQUIRED_MATERIALS = [
    "取水许可申请表",
    "身份证",
    "营业执照",
    "水资源论证报告",
    "承诺书",
]


class ReviewAgent:
    """涉水审批合规审查 Agent。"""

    _cache: dict[str, ReviewResultResponse] = {}

    async def review(self, request: ReviewSubmitRequest) -> ReviewResultResponse:
        """执行完整的合规审查流程。"""
        findings: list[ReviewFindingResult] = []

        # 1. 形式审查 - 材料完整性
        material_names = [m.name for m in request.materials]
        completeness = self.check_materials_completeness(material_names)
        if not completeness.is_complete:
            for missing in completeness.missing_materials:
                findings.append(ReviewFindingResult(
                    category="形式审查",
                    severity="Error",
                    description=f"缺少必备材料：{missing}",
                    suggestion=f"请补充提交 {missing}",
                ))

        # 2. 内容规范审查 - 字段检查
        content_findings = self._check_content规范(request)
        findings.extend(content_findings)

        # 3. 实质合规审查 - 知识库检索
        substantive_findings = await self._check_substantive合规(request)
        findings.extend(substantive_findings)

        # 4. 附件类型检查
        attachment_findings = self._check附件类型(request)
        findings.extend(attachment_findings)

        is_passed = not any(f.severity == "Error" for f in findings)
        summary = self._generate_summary(is_passed, findings)

        result = ReviewResultResponse(
            application_id=request.application_id,
            is_passed=is_passed,
            summary=summary,
            findings=findings,
            reviewed_at=datetime.now(timezone.utc).isoformat(),
        )

        # 缓存结果
        self._cache[request.application_id] = result
        return result

    def get_cached_result(self, application_id: str) -> ReviewResultResponse | None:
        return self._cache.get(application_id)

    def check_materials_completeness(self, materials: list[str]) -> CheckCompletenessResponse:
        """检查材料完整性（MCP 工具：check_completeness）。"""
        provided = []
        missing = []

        for required in REQUIRED_MATERIALS:
            found = False
            for m in materials:
                if required in m or m in required:
                    found = True
                    provided.append(required)
                    break
            if not found:
                missing.append(required)

        return CheckCompletenessResponse(
            required_materials=REQUIRED_MATERIALS,
            provided_materials=provided,
            missing_materials=missing,
            is_complete=len(missing) == 0,
        )

    def _check_content规范(self, request: ReviewSubmitRequest) -> list[ReviewFindingResult]:
        """内容规范审查。"""
        findings = []

        if not request.applicant_name or not request.applicant_name.strip():
            findings.append(ReviewFindingResult(
                category="内容规范",
                severity="Error",
                description="申请人姓名为空",
                suggestion="请填写申请人姓名",
            ))

        if not request.applicant_id_card or not request.applicant_id_card.strip():
            findings.append(ReviewFindingResult(
                category="内容规范",
                severity="Error",
                description="证件号为空",
                suggestion="请填写有效证件号码",
            ))

        if not request.water_intake_location or not request.water_intake_location.strip():
            findings.append(ReviewFindingResult(
                category="内容规范",
                severity="Error",
                description="取水地点为空",
                suggestion="请填写取水地点",
            ))

        if not request.water_intake_purpose or not request.water_intake_purpose.strip():
            findings.append(ReviewFindingResult(
                category="内容规范",
                severity="Error",
                description="取水用途为空",
                suggestion="请填写取水用途",
            ))

        if request.water_intake_purpose == "其他":
            findings.append(ReviewFindingResult(
                category="内容规范",
                severity="Warning",
                description='取水用途选择了"其他"但未补充说明',
                suggestion="请在备注中补充说明具体取水用途",
            ))

        if request.water_intake_amount <= 0:
            findings.append(ReviewFindingResult(
                category="内容规范",
                severity="Error",
                description="申请取水量不合法",
                suggestion="取水量必须大于 0",
            ))

        return findings

    async def _check_substantive合规(self, request: ReviewSubmitRequest) -> list[ReviewFindingResult]:
        """实质合规审查 - 检索知识库中的法规。"""
        findings = []

        # 检索取水地点相关法规
        query = f"取水地点 {request.water_intake_location} 禁止 限制 保护区"
        results = knowledge_store.search(query, top_k=3)

        for r in results:
            content = r["content"]
            score = r["score"]
            if score > 0.6 and any(kw in content for kw in ["禁止", "限制", "保护区", "水源地"]):
                findings.append(ReviewFindingResult(
                    category="实质合规",
                    severity="Warning",
                    description=f"取水地点可能涉及限制区域（相关法规片段，相似度 {score:.2f}）",
                    suggestion=f"请核实取水地点是否位于禁止或限制区域。参考：{content[:200]}",
                ))

        # 检索取水量合理性
        query_amount = f"取水量 {request.water_intake_amount} 许可 限额 标准"
        amount_results = knowledge_store.search(query_amount, top_k=2)

        for r in amount_results:
            if r["score"] > 0.6:
                findings.append(ReviewFindingResult(
                    category="实质合规",
                    severity="Info",
                    description=f"检索到相关取水量规定（相似度 {r['score']:.2f}）",
                    suggestion=f"请参照：{r['content'][:200]}",
                ))

        return findings

    def _check附件类型(self, request: ReviewSubmitRequest) -> list[ReviewFindingResult]:
        """检查附件类型是否匹配。"""
        findings = []

        for material in request.materials:
            if material.attachment_type == "id_card" and material.file_type not in ("jpg", "jpeg", "png", "pdf"):
                findings.append(ReviewFindingResult(
                    category="形式审查",
                    severity="Error",
                    description=f"身份证附件格式不正确：{material.file_type}",
                    suggestion="请上传 JPG/PNG/PDF 格式的身份证件",
                ))

            if material.attachment_type == "id_card":
                if "驾驶" in material.name:
                    findings.append(ReviewFindingResult(
                        category="形式审查",
                        severity="Error",
                        description="上传的身份证附件实际为驾驶证",
                        suggestion="请上传正确的身份证件，而非驾驶证",
                    ))

            if material.attachment_type == "business_license":
                if "过期" in material.name or "expired" in material.name.lower():
                    findings.append(ReviewFindingResult(
                        category="关联性",
                        severity="Error",
                        description="营业执照已过期",
                        suggestion="请提供在有效期内的营业执照",
                    ))

        return findings

    @staticmethod
    def _generate_summary(is_passed: bool, findings: list[ReviewFindingResult]) -> str:
        if is_passed and not findings:
            return "审查通过，未发现不合规问题"

        error_count = sum(1 for f in findings if f.severity == "Error")
        warning_count = sum(1 for f in findings if f.severity == "Warning")
        info_count = sum(1 for f in findings if f.severity == "Info")

        parts = []
        if error_count:
            parts.append(f"{error_count} 项错误")
        if warning_count:
            parts.append(f"{warning_count} 项警告")
        if info_count:
            parts.append(f"{info_count} 项提示")

        status = "审查未通过" if not is_passed else "审查通过（有提示）"
        return f"{status}，共发现 {', '.join(parts)}"

    def knowledge_search(self, query: str, top_k: int = 5) -> list[dict]:
        """MCP 工具：knowledge_search。"""
        return knowledge_store.search(query, top_k=top_k)


review_agent = ReviewAgent()
