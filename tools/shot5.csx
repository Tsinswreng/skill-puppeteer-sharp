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
var loginBtn = await page.QuerySelectorAsync("button.el-button--primary");
if (loginBtn != null) { await loginBtn.ClickAsync(); }
else {
	await page.EvaluateFunctionAsync(@"() => {
		var t = [...document.querySelectorAll('button')].find(x => x.innerText.trim() === '登录');
		if (t) t.click();
	}");
}
await Task.Delay(5000, Ct);

await page.EvaluateFunctionAsync("() => { location.hash = '#/projects'; }");
await Task.Delay(2500, Ct);
await page.EvaluateFunctionAsync(@"() => {
	var rows = [...document.querySelectorAll('tbody tr')];
	var target = rows.find(r => r.innerText.includes('2026_1102_153443'));
	var btn = [...target.querySelectorAll('button')].find(b => b.innerText.includes('切换项目'));
	btn && btn.click();
}");
await Task.Delay(4000, Ct);

await page.EvaluateFunctionAsync("() => { location.hash = '#/task/tasks'; }");
await Task.Delay(3500, Ct);

await page.EvaluateFunctionAsync(@"() => {
	var b = [...document.querySelectorAll('button')].find(x=>x.innerText.includes('创建任务'));
	if (b) b.id = '__create_btn';
}");
var createBtn = await page.QuerySelectorAsync("#__create_btn");
Console.WriteLine("create btn found: " + (createBtn != null));
if (createBtn != null) {
	await createBtn.ClickAsync();
	await Task.Delay(5000, Ct);
}
Console.WriteLine("url: " + page.Url);
var txt = await page.EvaluateFunctionAsync<string>(@"() => document.body.innerText.replace(/\s+/g,' ').slice(0,1200)");
Console.WriteLine(txt);
await page.ScreenshotAsync(System.IO.Path.Combine(OutDir, "shot-task-create.png"));
Console.WriteLine("saved");

await browser.CloseAsync();
