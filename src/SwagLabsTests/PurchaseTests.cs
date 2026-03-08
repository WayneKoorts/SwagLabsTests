namespace SwagLabsTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PurchaseTests : AuthenticatedPageTest
{
    [Test]
    public async Task CompletePurchaseOfFirstTwoItems()
    {
        await LoginAsync(TestUsers.StandardUser, TestUsers.Password);
        await Expect(Page).ToHaveURLAsync(new Regex(".*inventory\\.html"));

        // Add items to cart.  Get first two items by list index.
        var items = Page.Locator("[data-test='inventory-item']");
        await items.Nth(0).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await items.Nth(1).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await Expect(Page.Locator("[data-test='shopping-cart-badge']")).ToHaveTextAsync("2");

        // Go to the shopping cart.
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*cart\\.html"));
        await Expect(Page.Locator("[data-test='inventory-item']")).ToHaveCountAsync(2);

        // Go to checkout and fill in "billing" details.
        await Page.Locator("[data-test='checkout']").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*checkout-step-one\\.html"));
        await Page.Locator("[data-test='firstName']").FillAsync("John");
        await Page.Locator("[data-test='lastName']").FillAsync("Lennon");
        await Page.Locator("[data-test='postalCode']").FillAsync("90210");
        await Page.Locator("[data-test='continue']").ClickAsync();

        // Verify we made it to the final checkout step.
        await Expect(Page).ToHaveURLAsync(new Regex(".*checkout-step-two\\.html"));
        await Expect(Page.Locator("[data-test='inventory-item']")).ToHaveCountAsync(2);
        await Page.Locator("[data-test='finish']").ClickAsync();

        // Verify we're being shown the success page.
        await Expect(Page).ToHaveURLAsync(new Regex(".*checkout-complete\\.html"));
        await Expect(Page.Locator("[data-test='complete-header']")).ToHaveTextAsync("Thank you for your order!");
    }
}
