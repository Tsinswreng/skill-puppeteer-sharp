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

await page.EvaluateFunctionAsync("() => { location.hash = '#/projects'; }");
await Task.Delay(3000, Ct);
await page.EvaluateFunctionAsync(@"() => {
	var rows = [...document.querySelectorAll('tbody tr')];
	var target = rows.find(r => r.innerText.includes('2026_1102_153443'));
	var btn = [...target.querySelectorAll('button')].find(b => b.innerText.includes('切换项目'));
	btn && btn.click();
}");
await Task.Delay(4000, Ct);

// 展開任務管理
var taskMenu = await page.EvaluateFunctionAsync<bool>(@"() => {
	var els = [...document.querySelectorAll('*')].filter(e => e.children.length === 0 && e.innerText && e.innerText.trim() === '任务管理');
	if (!els.length) return false;
	els[0].click();
	return true;
}");
Console.WriteLine("click task menu: " + taskMenu);
await Task.Delay(2500, Ct);

var sub = await page.EvaluateFunctionAsync<string>(@"() => {
	var items = [...document.querySelectorAll('*')].filter(e => e.children.length === 0 && e.innerText && e.innerText.trim().length < 10);
	return JSON.stringify([...new Set(items.map(e=>e.innerText.trim()))].filter(t=>t.includes('任务')||t.includes('资产')||t.includes('工作流')).slice(0,10));
}");
Console.WriteLine(sub);

await Task.Delay(1500, Ct);

var clicked = await page.EvaluateFunctionAsync<bool>(@"() => {
	var els = [...document.querySelectorAll('*')].filter(e => e.children.length === 0 && e.innerText && e.innerText.trim() === '原子任务');
	if (!els.length) return false;
	els[0].click();
	return true;
}");
Console.WriteLine("click 原子任务: " + clicked);
await Task.Delay(3500, Ct);

Console.WriteLine("url: " + page.Url);
await page.ScreenshotAsync(System.IO.Path.Combine(OutDir, "shot-task-list.png"));
Console.WriteLine("saved shot-task-list.png");

await browser.CloseAsync();
