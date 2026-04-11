namespace LoginApp.Tests
{
    using Microsoft.Playwright;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class LoginTest
    {
        [TestMethod]
        public async Task Login_Should_Work()
        {
            using var playwright = await Playwright.CreateAsync();

            var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false // browser visible
            });

            var context = await browser.NewContextAsync(new()
            {
                IgnoreHTTPSErrors = true
            });

            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:5001/login");

            await page.FillAsync("[data-testid='username']", "admin");
            await page.FillAsync("[data-testid='password']", "1234");

            await page.ClickAsync("[data-testid='login-btn']");

            await page.WaitForSelectorAsync("text=Login Success");

            Assert.IsTrue(await page.IsVisibleAsync("text=Login Success"));

            await browser.CloseAsync();
        }
    }
}
