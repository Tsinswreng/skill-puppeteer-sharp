#r "nuget: Microsoft.Playwright"
#nullable enable
using Microsoft.Playwright;
using CT = System.Threading.CancellationToken;
CT Ct = default;

Console.WriteLine("creating playwright");
var pw = await Playwright.CreateAsync();
Console.WriteLine("driver ok: " + pw.GetType().Name);

var browser = await pw.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = true });
var page = await browser.NewPageAsync(new() { ViewportSize = new() { Width = 1600, Height = 900 } });
await page.GotoAsync("http://localhost:8888/", new() { WaitUntil = WaitUntilState.NetworkIdle });
Console.WriteLine("title: " + await page.TitleAsync());
Console.WriteLine("url: " + page.Url);
await page.ScreenshotAsync(new() { Path = @"C:\_\Code\Tswg-Nicp\_Doc\Problems\assets\pw-login.png" });
Console.WriteLine("saved pw-login.png");
await browser.CloseAsync();
