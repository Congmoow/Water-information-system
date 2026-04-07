from playwright.sync_api import sync_playwright


with sync_playwright() as playwright:
    browser = playwright.chromium.launch(headless=True)
    page = browser.new_page()
    page.goto("http://127.0.0.1:5173/login", wait_until="networkidle")
    page.get_by_placeholder("admin / viewer").fill("admin")
    page.get_by_placeholder("请输入密码").fill("Admin@123")
    page.get_by_role("button", name="登录系统").click()
    page.wait_for_url("**/dashboard", timeout=10000)
    page.wait_for_load_state("networkidle")
    heading = page.locator("h2").first.text_content() or ""
    print(f"LOGIN_OK|{page.url}|{heading}")
    browser.close()
