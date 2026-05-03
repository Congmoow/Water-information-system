"""MCP 工具定义：knowledge_search 和 check_completeness。"""

from __future__ import annotations

from mcp.server import Server
from mcp.types import Tool, TextContent

from app.services.review_agent import review_agent
from app.services.knowledge_store import knowledge_store

server = Server("water-approval-mcp")


@server.list_tools()
async def list_tools() -> list[Tool]:
    return [
        Tool(
            name="knowledge_search",
            description="检索水利法规知识库，返回与查询相关的法规片段。用于合规性审查时查找相关法律依据。",
            inputSchema={
                "type": "object",
                "properties": {
                    "query": {
                        "type": "string",
                        "description": "检索查询语句，如 '取水许可证申请材料要求' 或 '水源保护区禁止事项'",
                    },
                    "top_k": {
                        "type": "integer",
                        "description": "返回结果数量，默认 5",
                        "default": 5,
                    },
                },
                "required": ["query"],
            },
        ),
        Tool(
            name="check_completeness",
            description="对照检查清单，判断申请材料是否完整。接收材料名称列表，返回缺失的必备材料。",
            inputSchema={
                "type": "object",
                "properties": {
                    "materials": {
                        "type": "array",
                        "items": {"type": "string"},
                        "description": "已提交的材料名称列表",
                    },
                },
                "required": ["materials"],
            },
        ),
    ]


@server.call_tool()
async def call_tool(name: str, arguments: dict) -> list[TextContent]:
    import json

    if name == "knowledge_search":
        query = arguments.get("query", "")
        top_k = arguments.get("top_k", 5)
        results = knowledge_store.search(query, top_k=top_k)
        return [TextContent(type="text", text=json.dumps(results, ensure_ascii=False, indent=2))]

    elif name == "check_completeness":
        materials = arguments.get("materials", [])
        result = review_agent.check_materials_completeness(materials)
        return [TextContent(type="text", text=json.dumps(result.model_dump(), ensure_ascii=False, indent=2))]

    else:
        return [TextContent(type="text", text=f"未知工具: {name}")]
