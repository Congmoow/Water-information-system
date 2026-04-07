import json
import time
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
        raise RuntimeError("Login succeeded but token is empty")
    return token


def verify_backend_crud(token: str):
    suffix = str(int(time.time()))

    reservoir_payload = {
        "name": f"Stage3 Reservoir {suffix}",
        "location": "Wuhan Test Zone",
        "capacity": 82.5,
        "managementUnit": "Hydrology Demo Center",
        "latitude": 30.612345,
        "longitude": 114.345678,
        "description": "stage3 api smoke"
    }
    created_reservoir = api_request("POST", "/api/reservoir", token=token, payload=reservoir_payload)["data"]
    reservoir_id = created_reservoir["id"]

    river_payload = {
        "name": f"Stage3 River {suffix}",
        "basin": "South Test Basin",
        "length": 36.8,
        "latitude": 30.623456,
        "longitude": 114.356789,
        "description": "stage3 api smoke"
    }
    created_river = api_request("POST", "/api/river", token=token, payload=river_payload)["data"]
    river_id = created_river["id"]

    station_payload = {
        "name": f"Stage3 Station {suffix}",
        "type": "WaterLevel",
        "longitude": 114.366789,
        "latitude": 30.634567,
        "status": "Online",
        "warningThreshold": 18.5,
        "criticalThreshold": 20.0,
        "description": "stage3 api smoke",
        "lastActiveAt": "2026-04-07T10:00:00",
        "reservoirId": reservoir_id,
        "riverId": river_id
    }
    created_station = api_request("POST", "/api/station", token=token, payload=station_payload)["data"]
    station_id = created_station["id"]

    search_reservoir = api_request(
        "GET",
        f"/api/reservoir?{urllib.parse.urlencode({'page': 1, 'pageSize': 10, 'keyword': suffix})}",
        token=token,
    )["data"]
    if not search_reservoir["items"]:
        raise RuntimeError("Reservoir search returned no items")

    search_river = api_request(
        "GET",
        f"/api/river?{urllib.parse.urlencode({'page': 1, 'pageSize': 10, 'keyword': suffix})}",
        token=token,
    )["data"]
    if not search_river["items"]:
        raise RuntimeError("River search returned no items")

    search_station = api_request(
        "GET",
        f"/api/station?{urllib.parse.urlencode({'page': 1, 'pageSize': 10, 'keyword': suffix})}",
        token=token,
    )["data"]
    if not search_station["items"]:
        raise RuntimeError("Station search returned no items")

    updated_reservoir = dict(reservoir_payload)
    updated_reservoir["name"] = f"Stage3 Reservoir Updated {suffix}"
    updated_reservoir["capacity"] = 88.1
    api_request("PUT", f"/api/reservoir/{reservoir_id}", token=token, payload=updated_reservoir)

    updated_river = dict(river_payload)
    updated_river["name"] = f"Stage3 River Updated {suffix}"
    updated_river["length"] = 42.3
    api_request("PUT", f"/api/river/{river_id}", token=token, payload=updated_river)

    updated_station = dict(station_payload)
    updated_station["name"] = f"Stage3 Station Updated {suffix}"
    updated_station["status"] = "Warning"
    updated_station["warningThreshold"] = 19.2
    api_request("PUT", f"/api/station/{station_id}", token=token, payload=updated_station)

    detail_station = api_request("GET", f"/api/station/{station_id}", token=token)["data"]
    if detail_station["status"] != "Warning":
        raise RuntimeError(f"Station update not persisted: {detail_station}")

    api_request("DELETE", f"/api/station/{station_id}", token=token)
    api_request("DELETE", f"/api/river/{river_id}", token=token)
    api_request("DELETE", f"/api/reservoir/{reservoir_id}", token=token)

    api_request("GET", f"/api/station/{station_id}", token=token, expected_status=404)
    api_request("GET", f"/api/river/{river_id}", token=token, expected_status=404)
    api_request("GET", f"/api/reservoir/{reservoir_id}", token=token, expected_status=404)

    print(f"API_CRUD_OK|{reservoir_id}|{river_id}|{station_id}")


def verify_frontend_pages():
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(headless=True)
        page = browser.new_page()
        page.goto(f"{WEB_BASE}/login", wait_until="networkidle")
        page.locator(".login-card input").nth(0).fill("admin")
        page.locator(".login-card input").nth(1).fill("Admin@123")
        page.locator(".login-card button").first.click()
        page.wait_for_url("**/dashboard", timeout=10000)

        checked_routes = []
        for route in ["/dashboard", "/reservoirs", "/rivers", "/stations"]:
            page.goto(f"{WEB_BASE}{route}", wait_until="networkidle")
            if route == "/dashboard":
                page.wait_for_selector(".page-shell", timeout=10000)
            else:
                page.wait_for_selector(".el-table", timeout=10000)
                row_count = page.locator("tbody tr").count()
                if row_count < 1:
                    raise RuntimeError(f"{route} table has no rows")
            checked_routes.append(route)

        print(f"FRONTEND_PAGES_OK|{'|'.join(checked_routes)}")
        browser.close()


if __name__ == "__main__":
    auth_token = login()
    verify_backend_crud(auth_token)
    verify_frontend_pages()
