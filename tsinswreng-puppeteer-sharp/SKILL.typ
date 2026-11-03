#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading

\-\-\-

name: tsinswreng-puppeteer-sharp

description: PuppeteerSharp庫使用教程, 用于操作瀏覽器

\-\-\-

#H[在dotnet-script(csx)中用][
	
	先看 cssh skill。
	
	#H[腳本開頭寫法][
	```cs
#r "nuget: Tsinswreng.CsSh, 0.3.0-alpha"
#r "nuget: PuppeteerSharp"
#nullable enable
using PuppeteerSharp;
using Tsinswreng.CsSh;
using static Tsinswreng.CsSh.ShGlobal;
using CT = System.Threading.CancellationToken;
CT Ct = default;
	```
	開頭寫法一般比較固定,
	無何好講的。
	]
	
	#H[用edge啓動新頁面][
````cs
var EdgePath = @"C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe";

var browser = await Puppeteer.LaunchAsync(new LaunchOptions {
	Headless = true,
	ExecutablePath = EdgePath,
	DefaultViewport = new ViewPortOptions { Width = 1600, Height = 900 },
	Args = ["--no-sandbox", "--disable-gpu"],
});
var page = await browser.NewPageAsync();

````
	]
	
	#H[導航並等待就緒][
	```cs
	await page.GoToAsync("https://example.com", WaitUntilNavigation.Networkidle0);
	```
	`WaitUntilNavigation` 常用值：
	- `Load`（默認，DOM加載完成）、
	- `Networkidle0`（500ms內無網絡請求，適合SPA）、
	- `Networkidle2`（允許殘留少量輪詢請求）。
	單頁應用往往 `Load` 之後內容纔開始渲染，
	與其猜固定延時，
	不如直接等一個只在渲染完成後纔出現的選擇器（見下節）。
	統一調超時：
	```cs
	page.DefaultTimeout = 10_000;
	page.DefaultNavigationTimeout = 20_000;
	```
	]
	
	#H[等元素出現或消失][
	```cs
	// 等出現（須可見）
	await page.WaitForSelectorAsync("#result", new WaitForSelectorOptions {
		Visible = true,
		Timeout = 10_000,
	});
	
	// 等消失（如加載遮罩退場）
	await page.WaitForSelectorAsync(".loading", new WaitForSelectorOptions {
		Hidden = true,
	});
	```
	這是替代固定延時的首選寫法，
	又快又穩。
	另：舊版的 `page.WaitForTimeoutAsync` 在新版已移除，
	確實要固定延時就 `await Task.Delay(500);`。
	]
	
	#H[取文本與屬性][
	```cs
	var el = await page.QuerySelectorAsync("#title");
	var Text = await el!.EvaluateFunctionAsync<string>("e => e.innerText");

	// 多個元素逐個取
	foreach (var Item in await page.QuerySelectorAllAsync(".item")) {
		var S = (await Item.EvaluateFunctionAsync<string>("e => e.innerText")).Trim();
		Console.WriteLine(S);
	}

	// 取屬性
	var Href = await el!.EvaluateFunctionAsync<string>("e => e.getAttribute('href')");
	```
	`EvaluateFunctionAsync` 傳的是*函數*，
	首個參數綁到 `e`；
	`EvaluateExpressionAsync` 傳的是*表達式*，
	不帶參數。
	兩者搞混是常見報錯來源。
	]
	
	#H[點擊與輸入][
	```cs
	await page.ClickAsync("#submit");

	// 輸入框：先清空舊值，再聚焦打字
	var Input = await page.QuerySelectorAsync("input[name=q]");
	await Input!.EvaluateFunctionAsync("e => e.value = ''");
	await Input!.FocusAsync();
	await page.Keyboard.TypeAsync("關鍵詞");
	await page.Keyboard.PressAsync("Enter");
	```
	]
	
	#H[點擊後等跳轉][
	```cs
	await Task.WhenAll(
		page.WaitForNavigationAsync(),
		page.ClickAsync("a.next")
	);
	```
	等待與點擊並行發出；
	寫成先點後等，
	慢頁面上會漏接跳轉事件。
	]
	
	#H[截圖與PDF][
	```cs
	await page.ScreenshotAsync("shot.png", new ScreenshotOptions {
		FullPage = true,
	});
	await page.PdfAsync("out.pdf"); // 僅無頭模式可用
	```
	]
	
	#H[多頁面管理][
	```cs
	var Pages = await browser.PagesAsync();
	await Pages[0].BringToFrontAsync();
	var page2 = await browser.NewPageAsync();
	```
	]
	
	#H[攔截後端響應][
	```cs
	var ApiResponses = new List<Response>();
	page.Response += (_, resp) => {
		if (resp.Url.Contains("/api/items")) {
			ApiResponses.Add(resp);
		}
	};
	await page.GoToAsync("https://example.com", WaitUntilNavigation.Networkidle0);
	var Body = await ApiResponses[0].TextAsync();
	Console.WriteLine(Body);
	```
	事件要在導航*前*掛上；
	處理器裏不做異步，
	把 `Response` 對象存進列表、
	事後再取體。
	前端展示取不到的數據（分頁總數、隱藏字段）常靠這招從接口拿。
	]
	
	#H[收尾][
	```cs
	await browser.CloseAsync();
	```
	腳本中途崩潰會留下瀏覽器進程；
	重要腳本把主流程包進 `try`，
	在 `finally` 裏關。
	]

]

#H[關于此skill的反饋和改進][
	在使用此skill的過程中,
	若你發現哪裏有錯誤或 寫得不好 或 值得補充的地方,
	請不吝主動聯系用戶。
]