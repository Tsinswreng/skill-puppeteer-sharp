#r "nuget: PuppeteerSharp"
#nullable enable
using PuppeteerSharp;
using CT = System.Threading.CancellationToken;
CT Ct = default;

var OutDir = @"C:\_\Code\Tswg-Nicp\_Doc\Problems\assets";
var EdgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

var browser = await Puppeteer.LaunchAsync(new LaunchOptions {
	Headless = true,
	ExecutablePath = EdgePath,
	DefaultViewport = new ViewPortOptions { Width = 1600, Height = 900 },
	Args = ["--no-sandbox", "--disable-gpu"],
});
var page = await browser.NewPageAsync();

// 登入
await page.GoToAsync("http://localhost:8888/", new NavigationOptions {
	WaitUntil = [WaitUntilNavigation.Networkidle2],
});
await page.WaitForSelectorAsync("input[name=username]");
await page.TypeAsync("input[name=username]", "super_admin");
await page.TypeAsync("input[name=password]", "=R@3b<mX14_o<bvd63U5");
await page.EvaluateFunctionAsync(@"() => {
	var t = [...document.querySelectorAll('button')].find(x => x.innerText.trim() === '登录');
	if (t) t.click();
}");
await Task.Delay(5000, Ct);
Console.WriteLine("after login: " + page.Url);

// 截圖工具
async Task Shot(string hash, string name) {
	await page.EvaluateFunctionAsync("h => { location.hash = h; }", hash);
	await Task.Delay(3500, Ct);
	await page.ScreenshotAsync(System.IO.Path.Combine(OutDir, name));
	Console.WriteLine($"saved {name}  hash={await page.EvaluateFunctionAsync<string>("() => location.hash")}");
}

await Shot("#/", "shot-dashboard.png");
await Shot("#/projects", "shot-projects.png");
await Shot("#/platform/project", "shot-platform-project.png");
await Shot("#/node/nodes/global", "shot-nodes.png");

// 摸一下項目列表頁裏的元素
await page.EvaluateFunctionAsync("() => { location.hash = '#/projects'; }");
await Task.Delay(3000, Ct);
var info = await page.EvaluateFunctionAsync<string>(@"() => {
	var rows = [...document.querySelectorAll('tbody tr')].map(r => r.innerText.replace(/\s+/g,' ').trim()).slice(0,5);
	var btns = [...document.querySelectorAll('button')].map(b => (b.innerText.trim()||b.getAttribute('aria-label')||'') + (b.disabled ? '[disabled]' : '')).filter(Boolean).slice(0,20);
	var links = [...document.querySelectorAll('a[href]')].map(a => a.getAttribute('href')).slice(0,10);
	return JSON.stringify({rows, btns, links}, null, 1);
}");
Console.WriteLine(info);

await browser.CloseAsync();
