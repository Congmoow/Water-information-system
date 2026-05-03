"""文档解析服务：支持 PDF 和 Word 文档的文本提取与分块。"""

from __future__ import annotations

import io
import re
from dataclasses import dataclass, field


@dataclass
class DocumentChunk:
    content: str
    source: str
    page: int | None = None
    section: str | None = None
    metadata: dict = field(default_factory=dict)


class DocumentParser:
    """解析 PDF / Word 文档，返回结构化文本块。"""

    def parse_bytes(self, content: bytes, suffix: str, filename: str) -> list[DocumentChunk]:
        if suffix == "pdf":
            return self._parse_pdf(content, filename)
        elif suffix in ("docx", "doc"):
            return self._parse_docx(content, filename)
        else:
            raise ValueError(f"不支持的文件格式: {suffix}")

    def _parse_pdf(self, content: bytes, filename: str) -> list[DocumentChunk]:
        import pdfplumber

        chunks: list[DocumentChunk] = []
        with pdfplumber.open(io.BytesIO(content)) as pdf:
            for page_num, page in enumerate(pdf.pages, start=1):
                text = page.extract_text() or ""
                text = self._clean_text(text)
                if not text.strip():
                    continue
                paragraphs = self._split_paragraphs(text)
                for para in paragraphs:
                    if para.strip():
                        chunks.append(DocumentChunk(
                            content=para.strip(),
                            source=filename,
                            page=page_num,
                            metadata={"page": page_num},
                        ))
        return chunks

    def _parse_docx(self, content: bytes, filename: str) -> list[DocumentChunk]:
        from docx import Document

        chunks: list[DocumentChunk] = []
        doc = Document(io.BytesIO(content))
        current_section = ""

        for para in doc.paragraphs:
            text = para.text.strip()
            if not text:
                continue

            # 检测标题作为章节
            if para.style and para.style.name.startswith("Heading"):
                current_section = text
                continue

            text = self._clean_text(text)
            if text:
                chunks.append(DocumentChunk(
                    content=text,
                    source=filename,
                    section=current_section or None,
                    metadata={"section": current_section} if current_section else {},
                ))

        return chunks

    @staticmethod
    def _clean_text(text: str) -> str:
        text = re.sub(r"\s+", " ", text)
        text = re.sub(r"[^\w\s一-鿿，。；：！？、（）《》""''【】\-\.\,\;\:\!\?\/\(\)]", "", text)
        return text.strip()

    @staticmethod
    def _split_paragraphs(text: str) -> list[str]:
        paragraphs = re.split(r"\n{2,}", text)
        result = []
        for p in paragraphs:
            p = p.strip()
            if len(p) > 200:
                sentences = re.split(r"([。！？])", p)
                buf = ""
                for s in sentences:
                    buf += s
                    if s in "。！？" and len(buf) > 50:
                        result.append(buf.strip())
                        buf = ""
                if buf.strip():
                    result.append(buf.strip())
            elif p:
                result.append(p)
        return result


document_parser = DocumentParser()
