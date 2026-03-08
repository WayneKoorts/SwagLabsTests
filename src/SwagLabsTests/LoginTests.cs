namespace SwagLabsTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginTests : PageTest
{
    private async Task LoginAsync(string username, string password)
    {
        await Page.GotoAsync(Constants.BaseUrl);
        await Page.Locator("#user-name").FillAsync(username);
        await Page.Locator("#password").FillAsync(password);
        await Page.Locator("#login-button").ClickAsync();
    }

    [Test]
    public async Task ValidLoginNavigatesToInventoryPage()
    {
        await LoginAsync(TestUsers.StandardUser, TestUsers.Password);

        await Expect(Page).ToHaveURLAsync(new Regex(".*inventory\\.html"));
        await Expect(Page.Locator("div.inventory_list")).ToBeVisibleAsync();
    }

    [Test]
    public async Task LockedOutUserShowsError()
    {
        await LoginAsync(TestUsers.LockedOutUser, TestUsers.Password);

        var error = Page.Locator("h3[data-test='error']");
        await Expect(error).ToBeVisibleAsync();
        await Expect(error).ToContainTextAsync("locked out");
    }

    [Test]
    public async Task InvalidCredentialsShowsError()
    {
        await LoginAsync("invalid_user", "wrong_password");

        var error = Page.Locator("h3[data-test='error']");
        await Expect(error).ToBeVisibleAsync();
        await Expect(error).ToContainTextAsync("do not match");
    }

    [Test]
    public async Task EmptyUsernameShowsError()
    {
        await LoginAsync("", TestUsers.Password);

        var error = Page.Locator("h3[data-test='error']");
        await Expect(error).ToBeVisibleAsync();
        await Expect(error).ToContainTextAsync("Username is required");
    }

    [Test]
    public async Task EmptyPasswordShowsError()
    {
        await LoginAsync(TestUsers.StandardUser, "");

        var error = Page.Locator("h3[data-test='error']");
        await Expect(error).ToBeVisibleAsync();
        await Expect(error).ToContainTextAsync("Password is required");
    }

    [Test]
    public async Task LoginErrorCanBeDismissed()
    {
        await LoginAsync("", "");

        var error = Page.Locator("h3[data-test='error']");
        await Expect(error).ToBeVisibleAsync();

        await Page.Locator("button.error-button").ClickAsync();
        await Expect(error).Not.ToBeVisibleAsync();
    }
}
