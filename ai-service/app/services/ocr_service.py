"""OCR 服务：识别扫描件中的文字内容。"""

from __future__ import annotations

import io
import logging

logger = logging.getLogger(__name__)


class OcrService:
    """基于 PaddleOCR 的文字识别服务。"""

    _ocr = None

    def _get_ocr(self):
        if self._ocr is None:
            try:
                from paddleocr import PaddleOCR
                self._ocr = PaddleOCR(use_angle_cls=True, lang="ch", show_log=False)
            except ImportError:
                logger.warning("PaddleOCR 未安装，OCR 功能不可用")
                return None
        return self._ocr

    def recognize(self, image_bytes: bytes) -> str:
        """识别图片中的文字。"""
        ocr = self._get_ocr()
        if ocr is None:
            return "[OCR 服务不可用，请安装 PaddleOCR]"

        result = ocr.ocr(image_bytes, cls=True)
        if not result or not result[0]:
            return ""

        lines = []
        for line in result[0]:
            if line and len(line) >= 2:
                text = line[1][0] if isinstance(line[1], (list, tuple)) else str(line[1])
                lines.append(text)

        return "\n".join(lines)

    def recognize_from_file(self, file_path: str) -> str:
        """从文件路径识别文字。"""
        ocr = self._get_ocr()
        if ocr is None:
            return "[OCR 服务不可用]"

        result = ocr.ocr(file_path, cls=True)
        if not result or not result[0]:
            return ""

        lines = []
        for line in result[0]:
            if line and len(line) >= 2:
                text = line[1][0] if isinstance(line[1], (list, tuple)) else str(line[1])
                lines.append(text)

        return "\n".join(lines)


ocr_service = OcrService()
