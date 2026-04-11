namespace LoginApp.Tests
{
    using Microsoft.Playwright;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// End-to-End Integration Tests for Login functionality.
    /// 
    /// IMPORTANT: To run this test successfully, the LoginApp application must be running.
    /// 
    /// Steps to run:
    /// 1. In one terminal, start the application:
    ///    dotnet run --project LoginApp/LoginApp.csproj
    ///
    /// 2. In another terminal, run the tests:
    ///    dotnet test LoginApp.Tests/LoginApp.Tests.csproj
    /// </summary>
    [TestClass]
    public class LoginTest
    {
        private readonly string _baseUrl = "http://localhost:5176";
        private IBrowser _browser;
        private IPage _page;

        [TestInitialize]
        public async Task TestInitialize()
        {
            // Verify server is running before starting test
            using var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
            using var client = new HttpClient(handler);

            try
            {
                var response = await client.GetAsync($"{_baseUrl}/");
                if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new InvalidOperationException(
                        $"Application server not responding at {_baseUrl}. " +
                        $"Please start it with: dotnet run --project LoginApp/LoginApp.csproj");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"Application server not running at {_baseUrl}. " +
                    $"Please start it with: dotnet run --project LoginApp/LoginApp.csproj", ex);
            }

            // Initialize Playwright browser
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,
                SlowMo = 300
            });

            // Create browser context and page
            var context = await _browser.NewContextAsync(new()
            {
                IgnoreHTTPSErrors = true
            });

            _page = await context.NewPageAsync();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            if (_page != null)
            {
                await _page.CloseAsync();
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }

        [TestMethod]
        public async Task Login_Should_Navigate_To_Dashboard()
        {
            // Navigate to login page
            await _page.GotoAsync($"{_baseUrl}/login");

            // Fill in login credentials
            await _page.FillAsync("[data-testid='username']", "admin");
            await _page.FillAsync("[data-testid='password']", "1234");

            // Click login button
            await _page.ClickAsync("[data-testid='login-btn']");

            // Verify navigation to dashboard
            await _page.WaitForURLAsync("**/dashboard");

            // Assert that the URL contains "dashboard"
            Assert.IsTrue(_page.Url != null && _page.Url.Contains("dashboard"), 
                $"Expected URL to contain 'dashboard', but got: {_page.Url}");
        }
    }
}
