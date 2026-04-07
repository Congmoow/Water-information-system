import json
from datetime import datetime
import urllib.error
import urllib.parse
import urllib.request

from playwright.sync_api import sync_playwright


API_BASE = "http://127.0.0.1:5000"
WEB_BASE = "http://127.0.0.1:5173"


def api_request(method: str, path: str, token: str | None = None, payload: dict | None = None, expected_status: int = 200):
    body = None
    headers = {}

    if payload is not None:
        body = json.dumps(payload).encode("utf-8")
        headers["Content-Type"] = "application/json"

    if token:
        headers["Authorization"] = f"Bearer {token}"

    request = urllib.request.Request(f"{API_BASE}{path}", data=body, headers=headers, method=method)

    try:
        with urllib.request.urlopen(request) as response:
            status = response.status
            content = response.read().decode("utf-8")
    except urllib.error.HTTPError as exc:
        status = exc.code
        content = exc.read().decode("utf-8")

    if status != expected_status:
        raise RuntimeError(f"{method} {path} expected {expected_status}, got {status}: {content}")

    if not content:
        return None

    return json.loads(content)


def login() -> str:
    response = api_request(
        "POST",
        "/api/auth/login",
        payload={"username": "admin", "password": "Admin@123"},
    )
    token = response["data"]["token"]
    if not token:
        raise RuntimeError("login token is empty")
    return token


def verify_backend_flows(token: str):
    api_request("GET", "/openapi/v1.json")
    overview = api_request("GET", "/api/dashboard/overview", token=token)["data"]
    if overview["reservoirCount"] < 1 or overview["stationCount"] < 1:
        raise RuntimeError(f"dashboard overview looks empty: {overview}")

    map_points = api_request("GET", "/api/map/points", token=token)["data"]["items"]
    if len(map_points) < 3:
        raise RuntimeError(f"map points too few: {len(map_points)}")

    station_list = api_request("GET", "/api/station?page=1&pageSize=50", token=token)["data"]["items"]
    station = next((item for item in station_list if item["type"] == "WaterLevel"), None) or station_list[0]
    station_detail = api_request("GET", f"/api/station/{station['id']}", token=token)["data"]
    collected_at = datetime.now().strftime("%Y-%m-%dT%H:%M:%S")
    monitoring_payload = {
        "stationId": station["id"],
        "dataType": station_detail["type"],
        "value": float(station_detail["criticalThreshold"]) + 0.6,
        "collectedAt": collected_at,
        "remark": "stage456 verification"
    }

    monitoring_result = api_request("POST", "/api/monitoring", token=token, payload=monitoring_payload)["data"]
    if not monitoring_result["triggeredAlarm"] or not monitoring_result["alarmId"]:
        raise RuntimeError(f"monitoring create did not trigger alarm: {monitoring_result}")

    alarm_detail = api_request("GET", f"/api/alarm/{monitoring_result['alarmId']}", token=token)["data"]
    if alarm_detail["status"] != "Pending":
        raise RuntimeError(f"new alarm status invalid: {alarm_detail}")

    handled_alarm = api_request(
        "PUT",
        f"/api/alarm/{monitoring_result['alarmId']}/handle",
        token=token,
        payload={"status": "Resolved", "handleRemark": "stage456 verification handled"},
    )["data"]
    if handled_alarm["status"] != "Resolved":
        raise RuntimeError(f"alarm handle failed: {handled_alarm}")

    monitoring_list = api_request(
        "GET",
        f"/api/monitoring?{urllib.parse.urlencode({'stationId': station['id'], 'page': 1, 'pageSize': 10})}",
        token=token,
    )["data"]
    if monitoring_list["total"] < 1:
        raise RuntimeError("monitoring list returned no records")

    history = api_request(
        "GET",
        f"/api/monitoring/history?{urllib.parse.urlencode({'stationId': station['id'], 'dataType': station_detail['type']})}",
        token=token,
    )["data"]
    if len(history) < 1:
        raise RuntimeError("monitoring history returned no points")

    alarm_list = api_request(
        "GET",
        f"/api/alarm?{urllib.parse.urlencode({'stationId': station['id'], 'page': 1, 'pageSize': 10})}",
        token=token,
    )["data"]
    if alarm_list["total"] < 1:
        raise RuntimeError("alarm list returned no records")

    api_request("POST", "/api/auth/logout", token=token)
    print(f"BACKEND_STAGE456_OK|{station['id']}|{monitoring_result['alarmId']}")


def verify_frontend_pages():
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(headless=True)
        page = browser.new_page()
        page.goto(f"{WEB_BASE}/login", wait_until="networkidle")
        page.locator(".login-card input").nth(0).fill("admin")
        page.locator(".login-card input").nth(1).fill("Admin@123")
        page.locator(".login-card button").first.click()
        page.wait_for_url("**/dashboard", timeout=10000)

        page.goto(f"{WEB_BASE}/dashboard", wait_until="networkidle")
        page.wait_for_selector("canvas", timeout=10000)

        page.goto(f"{WEB_BASE}/monitoring", wait_until="networkidle")
        page.wait_for_selector(".el-table", timeout=10000)

        page.goto(f"{WEB_BASE}/alarms", wait_until="networkidle")
        page.wait_for_selector(".el-table", timeout=10000)

        page.goto(f"{WEB_BASE}/map", wait_until="networkidle")
        page.wait_for_selector(".leaflet-interactive", timeout=10000)
        page.locator(".leaflet-interactive").first.click()
        page.wait_for_selector(".map-sidebar__detail h3", timeout=10000)

        page.locator(".app-header__action").click()
        page.locator(".el-dropdown-menu__item").last.click()
        page.wait_for_url("**/login", timeout=10000)

        print("FRONTEND_STAGE456_OK|/dashboard|/monitoring|/alarms|/map|logout")
        browser.close()


if __name__ == "__main__":
    auth_token = login()
    verify_backend_flows(auth_token)
    verify_frontend_pages()
