# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Playwright-based UI tests for [Swag Labs](https://www.saucedemo.com/) using NUnit 4 and .NET 10. Tests inherit from `PageTest` (Playwright's NUnit base class) which provides the `Page` property and `Expect` assertions.

## Commands

Build:
```bash
dotnet build src/SwagLabsTests.sln
```

Run all tests:
```bash
dotnet test src/SwagLabsTests.sln
```

Run a specific test by name:
```bash
dotnet test src/SwagLabsTests.sln --filter "HomePageShowsLoginForm"
```

Run tests with a specific browser:
```bash
dotnet test src/SwagLabsTests.sln -- Playwright.BrowserName=firefox
```

Run all supported browsers:
```bash
./run-tests.sh        # Bash
./run-tests.ps1       # PowerShell
```

Run tests with detailed output:
```bash
dotnet test src/SwagLabsTests.sln --logger "console;verbosity=detailed"
```

Install Playwright browsers (required once after build or Playwright version update):
```bash
pwsh src/SwagLabsTests/bin/Debug/net10.0/playwright.ps1 install
```

## Architecture

- **Solution file**: `src/SwagLabsTests.sln`
- **Test project**: `src/SwagLabsTests/` — single NUnit test project targeting `net10.0`
- Test classes extend `PageTest` (from `Microsoft.Playwright.NUnit`), which handles browser/page lifecycle
- Global usings are configured in the `.csproj` via `<Using>` items: `Microsoft.Playwright.NUnit`, `NUnit.Framework`, `System.Text.RegularExpressions`, `System.Threading.Tasks`
- Tests use `[Parallelizable(ParallelScope.Self)]` for parallel execution
- Browser configuration is in `src/SwagLabsTests/.runsettings` (default: Chromium). Override via CLI: `-- Playwright.BrowserName=firefox`
- Helper scripts `run-tests.sh` / `run-tests.ps1` run tests against all supported browsers (Chromium, Firefox, WebKit)

## Coding Guidelines

- Prioritise clean, readable code. Follow DRY and SOLID principles — extract shared logic, avoid duplication, and keep classes/methods focused on a single responsibility.
- Never commit code unless explicitly asked. When asked to commit, break changes into small, logical commits (e.g. one commit per feature, fix, or refactor). Larger commits are acceptable when the changes are inherently cohesive, but prefer smaller ones.

## Environment

- Copy `.env.example` to `.env` for local config
- Set `HEADED=1` in `.env` (or inline) to run tests with a visible browser
