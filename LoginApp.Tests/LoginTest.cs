using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LoginApp.Tests
{
    [TestClass]
    public class LoginTest
    {
        private string _baseUrl = "";
        private string _loginPath = "";
        private string _username = "";
        private string _password = "";

        private IBrowser _browser = null!;
        private IPage _page = null!;

        [TestInitialize]
        public async Task TestInitialize()
        {
            // 🔹 Load appsettings.json
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _baseUrl = config["TestSettings:BaseUrl"]!;
            _loginPath = config["TestSettings:LoginPath"]!;
            _username = config["TestSettings:Username"]!;
            _password = config["TestSettings:Password"]!;

            // 🔹 Start Playwright
            var playwright = await Playwright.CreateAsync();

            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500
            });

            // 🔹 Browser context
            var context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true
            });

            _page = await context.NewPageAsync();

            // Increase default timeouts to reduce flaky timeout issues on slower networks/pages
            _page.SetDefaultTimeout(60000);
            _page.SetDefaultNavigationTimeout(60000);
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            if (_page != null)
                await _page.CloseAsync();

            if (_browser != null)
                await _browser.CloseAsync();
        }

        [TestMethod]
        public async Task Login_Should_Work()
        {
            // Open login page
            await _page.GotoAsync($"{_baseUrl}{_loginPath}");

            // Wait page load
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // 🔹 Enter Email
            await _page.GetByPlaceholder("Email Address")
                .FillAsync(_username);

            // 🔹 Click Continue
            await _page.GetByRole(AriaRole.Button, new()
            {
                Name = "Continue"
            }).ClickAsync();

            // 🔹 Wait for password page
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            // 🔹 Fill Password
            await _page.GetByPlaceholder("Password")
                .FillAsync(_password);

            // 🔹 Click Login
            await _page.GetByRole(AriaRole.Button, new()
            {
                Name = "Login"
            }).ClickAsync();

            // 🔹 Wait redirect
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // 🔹 Verify login success
            Assert.IsFalse(_page.Url.Contains("/Login"),
                $"Login failed. Current URL: {_page.Url}");

            Console.WriteLine($"Final URL: {_page.Url}");
        }
    }
}