namespace SwagLabsTests;

public class AuthenticatedPageTest : PageTest
{
    protected async Task LoginAsync(string username, string password)
    {
        await Page.GotoAsync(Constants.BaseUrl);

        // Skip filling empty fields to avoid triggering browser autofill overlays
        // that can obscure the login button and cause timeouts.
        if (username.Length > 0)
            await Page.Locator("#user-name").FillAsync(username);

        if (password.Length > 0)
            await Page.Locator("#password").FillAsync(password);

        await Page.Locator("#login-button").ClickAsync();
    }
}
