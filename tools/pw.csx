#r "nuget: Microsoft.Playwright"
#nullable enable
using Microsoft.Playwright;
using CT = System.Threading.CancellationToken;
CT Ct = default;

var OutDir = @"C:\_\Code\Tswg-Nicp\_Doc\Problems\assets";

var pw = await Playwright.CreateAsync();
var browser = await pw.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = true });
var page = await browser.NewPageAsync(new() { ViewportSize = new() { Width = 1600, Height = 900 } });
page.SetDefaultTimeout(15000);

// 登入：用文字定位，不需要手寫 evaluate
await page.GotoAsync("http://localhost:8888/", new() { WaitUntil = WaitUntilState.NetworkIdle });
await page.Locator("input[name=username]").FillAsync("super_admin");
await page.Locator("input[name=password]").FillAsync("=R@3b<mX14_o<bvd63U5");
await page.GetByRole(AriaRole.Button, new() { Name = "登录" }).ClickAsync();
await page.WaitForURLAsync("**/#/", new() { Timeout = 15000 });
Console.WriteLine("login ok: " + page.Url);

async Task Shot(string hash, string name) {
	await page.EvaluateAsync("h => { location.hash = h; }", hash);
	await page.WaitForTimeoutAsync(3000);
	await page.ScreenshotAsync(new() { Path = System.IO.Path.Combine(OutDir, name) });
	Console.WriteLine("saved " + name);
}

await Shot("#/", "pw-dashboard.png");
await Shot("#/projects", "pw-projects.png");
await Shot("#/platform/project", "pw-platform-project.png");
await Shot("#/node/nodes/global", "pw-nodes.png");

// 切換項目
await page.EvaluateAsync("() => { location.hash = '#/projects'; }");
await page.WaitForTimeoutAsync(2500);
var row = page.Locator("tbody tr", new() { HasTextString = "2026_1102_153443" });
await row.GetByRole(AriaRole.Button, new() { Name = "切换项目" }).ClickAsync();
await page.WaitForTimeoutAsync(3500);
Console.WriteLine("switched: " + (await page.Locator("body").InnerTextAsync()).Replace("\n"," ").Substring(0, 120));

// 進原子任務頁
await page.EvaluateAsync("() => { location.hash = '#/task/tasks'; }");
await page.WaitForTimeoutAsync(3000);
await page.ScreenshotAsync(new() { Path = System.IO.Path.Combine(OutDir, "pw-task-list.png") });
Console.WriteLine("saved pw-task-list.png");

// 點創建任務（PuppeteerSharp 需要 evaluate 抓 id 再點，這裏直接按文字點）
await page.GetByRole(AriaRole.Button, new() { Name = "创建任务" }).ClickAsync();
await page.WaitForTimeoutAsync(2500);
await page.ScreenshotAsync(new() { Path = System.IO.Path.Combine(OutDir, "pw-task-create.png") });
Console.WriteLine("saved pw-task-create.png");

// 點 servicescan —— PuppeteerSharp 的 ClickAsync 在這裏卡死過
await page.GetByText("servicescan@1.0.0", new() { Exact = true }).Last.ClickAsync();
await page.WaitForTimeoutAsync(3000);
Console.WriteLine("after pick: " + page.Url);
await page.ScreenshotAsync(new() { Path = System.IO.Path.Combine(OutDir, "pw-task-form.png") });
Console.WriteLine("saved pw-task-form.png");

await browser.CloseAsync();
