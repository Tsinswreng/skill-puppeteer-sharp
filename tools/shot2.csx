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

// 進項目列表，切換到已審批的那個項目
await page.EvaluateFunctionAsync("() => { location.hash = '#/projects'; }");
await Task.Delay(3000, Ct);

var switched = await page.EvaluateFunctionAsync<bool>(@"() => {
	var rows = [...document.querySelectorAll('tbody tr')];
	var target = rows.find(r => r.innerText.includes('2026_1102_153443'));
	if (!target) return false;
	var btn = [...target.querySelectorAll('button')].find(b => b.innerText.includes('切换项目'));
	if (!btn) return false;
	btn.click();
	return true;
}");
Console.WriteLine("switched: " + switched);
await Task.Delay(4000, Ct);

var menu = await page.EvaluateFunctionAsync<string>(@"() => {
	var links = [...document.querySelectorAll('a[href]')].map(a => (a.innerText.trim().replace(/\s+/g,' ') || '') + ' -> ' + a.getAttribute('href'));
	return JSON.stringify(links, null, 1);
}");
Console.WriteLine(menu);

await page.ScreenshotAsync(System.IO.Path.Combine(OutDir, "shot-after-switch.png"));
Console.WriteLine("saved shot-after-switch.png");

await browser.CloseAsync();
