#!/usr/bin/env bash
set -euo pipefail

browsers=("chromium" "firefox" "webkit")

for browser in "${browsers[@]}"; do
    echo "Running tests on $browser..."
    dotnet test src/SwagLabsTests.sln -- Playwright.BrowserName="$browser"
done
