# LoginApp
Playwright Login Demo – Blazor (C#)
📌 Overview

This project demonstrates how to use Playwright for .NET to automate a login flow in a Blazor application.

It covers:

Playwright setup in C#
UI automation for login
Assertions for validation
Test execution using NUnit/Mstest
🛠️ Tech Stack
Blazor (C#)
Playwright for .NET
NUnit Test Framework
Visual Studio 2026
⚙️ Setup Instructions
1. Create Test Project

Add a new NUnit/MsTest test project to your solution:

Right Click Solution → Add → New Project → NUnit/MStest Test Project
2. Install Playwright

Run the following command:

dotnet add package Microsoft.Playwright
3. Install Browsers
playwright install
4. Project Structure
LoginApp/
│
├── Pages/
│   └── Login.razor
│
├── LoginApp.Tests/
│   └── LoginTests.cs
│
└── README.md
🔐 Demo Login Page (Blazor)

Example Login.razor:

@page "/login"

<h3>Login</h3>

<input id="username" placeholder="Username" />
<input id="password" type="password" placeholder="Password" />
<button id="loginBtn">Login</button>

<p id="message"></p>

@code {
    private string message = "";

    private void HandleLogin()
    {
        if (username == "admin" && password == "1234")
        {
            message = "Login Successful";
        }
        else
        {
            message = "Invalid Credentials";
        }
    }

    string username;
    string password;
}
🧪 Playwright Login Test (C#)

Create LoginTests.cs:

using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BlazorApp.Tests
{
    public class LoginTests
    {
        [Test]
        public async Task Login_Should_Succeed_With_Valid_Credentials()
        {
            using var playwright = await Playwright.CreateAsync();

            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            var page = await browser.NewPageAsync();

            // Navigate to login page
            await page.GotoAsync("https://localhost:5001/login");

            // Enter credentials
            await page.FillAsync("#username", "admin");
            await page.FillAsync("#password", "1234");

            // Click login button
            await page.ClickAsync("#loginBtn");

            // Validate result
            var message = await page.InnerTextAsync("#message");

            Assert.AreEqual("Login Successful", message);

            await browser.CloseAsync();
        }
    }
}
▶️ Running the Test
Using Visual Studio
Open Test Explorer
Click Run All Tests
Using CLI
dotnet test
✅ Expected Output
Browser launches
Navigates to /login
Fills username & password
Clicks login
Displays: "Login Successful"
Test passes ✅
❌ Negative Test Example
[Test]
public async Task Login_Should_Fail_With_Invalid_Credentials()
{
    using var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync();

    var page = await browser.NewPageAsync();

    await page.GotoAsync("https://localhost:5001/login");

    await page.FillAsync("#username", "wrong");
    await page.FillAsync("#password", "wrong");

    await page.ClickAsync("#loginBtn");

    var message = await page.InnerTextAsync("#message");

    Assert.AreEqual("Invalid Credentials", message);

    await browser.CloseAsync();
}
