"""MCP Server 入口：可独立启动的 MCP 服务。"""

from __future__ import annotations

import asyncio
import logging

from mcp.server.stdio import stdio_server

from app.mcp.tools import server
from app.services.embedding_service import embedding_service
from app.services.knowledge_store import knowledge_store

logger = logging.getLogger(__name__)


async def run_mcp_server():
    """启动 MCP Server（stdio 模式）。"""
    logger.info("初始化嵌入模型和知识库...")
    embedding_service.initialize()
    knowledge_store.initialize()

    async with stdio_server() as (read_stream, write_stream):
        logger.info("MCP Server 已启动，等待连接...")
        await server.run(read_stream, write_stream, server.create_initialization_options())


def main():
    logging.basicConfig(level=logging.INFO)
    asyncio.run(run_mcp_server())


if __name__ == "__main__":
    main()
