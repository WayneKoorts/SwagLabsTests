namespace SwagLabsTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HomePageTests : PageTest
{
    [Test]
    public async Task HomePageShowsLoginForm()
    {
        await Page.GotoAsync(Constants.BaseUrl);

        var loginContainer = Page.Locator("div.login_container");
        await Expect(loginContainer).ToBeVisibleAsync();
    }
}
