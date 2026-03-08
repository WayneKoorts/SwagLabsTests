$browsers = @("chromium", "firefox", "webkit")

foreach ($browser in $browsers) {
    Write-Host "Running tests on $browser..."
    dotnet test src/SwagLabsTests.sln -- Playwright.BrowserName=$browser
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
