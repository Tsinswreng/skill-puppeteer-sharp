#r "nuget: PuppeteerSharp"
#nullable enable
using PuppeteerSharp;
using CT = System.Threading.CancellationToken;
CT Ct = default;

var OutDir = @"C:\_\Code\Tswg-Nicp\_Doc\Problems\assets";
var EdgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

Console.WriteLine("PuppeteerSharp loaded");

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
Console.WriteLine("login page ok: " + page.Url);

await page.ScreenshotAsync(System.IO.Path.Combine(OutDir, "shot-login.png"));
Console.WriteLine("saved shot-login.png");

await browser.CloseAsync();
