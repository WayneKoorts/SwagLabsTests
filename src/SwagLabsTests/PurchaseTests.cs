namespace SwagLabsTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PurchaseTests : AuthenticatedPageTest
{
    // This is the core test scenario requested for the interview task.
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

    [Test]
    public async Task CheckoutRequiresBillingDetails()
    {
        await LoginAsync(TestUsers.StandardUser, TestUsers.Password);

        // Add an item and navigate to checkout.
        var items = Page.Locator("[data-test='inventory-item']");
        await items.Nth(0).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        await Page.Locator("[data-test='checkout']").ClickAsync();

        // Try to continue without filling in any fields.
        await Page.Locator("[data-test='continue']").ClickAsync();

        var error = Page.Locator("[data-test='error']");
        await Expect(error).ToBeVisibleAsync();
        await Expect(error).ToContainTextAsync("First Name is required");

        // Fill first name only, verify last name is required.
        await Page.Locator("[data-test='firstName']").FillAsync("John");
        await Page.Locator("[data-test='continue']").ClickAsync();
        await Expect(error).ToContainTextAsync("Last Name is required");

        // Fill last name too, verify postal code is required.
        await Page.Locator("[data-test='lastName']").FillAsync("Lennon");
        await Page.Locator("[data-test='continue']").ClickAsync();
        await Expect(error).ToContainTextAsync("Postal Code is required");
    }

    [Test]
    public async Task RemoveItemFromCart()
    {
        await LoginAsync(TestUsers.StandardUser, TestUsers.Password);

        // Add the first two items and note the name of the second one.
        var items = Page.Locator("[data-test='inventory-item']");
        await items.Nth(0).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await items.Nth(1).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        var secondItemName = await items.Nth(1).Locator("[data-test='inventory-item-name']").TextContentAsync();

        // Go to cart and remove the first item.
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        var cartItems = Page.Locator("[data-test='inventory-item']");
        await Expect(cartItems).ToHaveCountAsync(2);
        await cartItems.Nth(0).Locator("button", new() { HasTextString = "Remove" }).ClickAsync();

        // Verify only the second item remains.
        await Expect(cartItems).ToHaveCountAsync(1);
        await Expect(cartItems.Nth(0).Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(secondItemName!);
        await Expect(Page.Locator("[data-test='shopping-cart-badge']")).ToHaveTextAsync("1");
    }

    [Test]
    public async Task ItemDetailsCarryThroughCheckout()
    {
        await LoginAsync(TestUsers.StandardUser, TestUsers.Password);

        // Capture item names and prices from the inventory page.
        var items = Page.Locator("[data-test='inventory-item']");
        var firstItemName = await items.Nth(0).Locator("[data-test='inventory-item-name']").TextContentAsync();
        var firstItemPrice = await items.Nth(0).Locator("[data-test='inventory-item-price']").TextContentAsync();
        var secondItemName = await items.Nth(1).Locator("[data-test='inventory-item-name']").TextContentAsync();
        var secondItemPrice = await items.Nth(1).Locator("[data-test='inventory-item-price']").TextContentAsync();

        // Add both items and go to cart.
        await items.Nth(0).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await items.Nth(1).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();

        // Verify names and prices match in the cart.
        var cartItems = Page.Locator("[data-test='inventory-item']");
        await Expect(cartItems.Nth(0).Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(firstItemName!);
        await Expect(cartItems.Nth(0).Locator("[data-test='inventory-item-price']")).ToHaveTextAsync(firstItemPrice!);
        await Expect(cartItems.Nth(1).Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(secondItemName!);
        await Expect(cartItems.Nth(1).Locator("[data-test='inventory-item-price']")).ToHaveTextAsync(secondItemPrice!);

        // Proceed to checkout overview and verify again.
        await Page.Locator("[data-test='checkout']").ClickAsync();
        await Page.Locator("[data-test='firstName']").FillAsync("John");
        await Page.Locator("[data-test='lastName']").FillAsync("Lennon");
        await Page.Locator("[data-test='postalCode']").FillAsync("90210");
        await Page.Locator("[data-test='continue']").ClickAsync();

        var overviewItems = Page.Locator("[data-test='inventory-item']");
        await Expect(overviewItems.Nth(0).Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(firstItemName!);
        await Expect(overviewItems.Nth(0).Locator("[data-test='inventory-item-price']")).ToHaveTextAsync(firstItemPrice!);
        await Expect(overviewItems.Nth(1).Locator("[data-test='inventory-item-name']")).ToHaveTextAsync(secondItemName!);
        await Expect(overviewItems.Nth(1).Locator("[data-test='inventory-item-price']")).ToHaveTextAsync(secondItemPrice!);
    }

    [Test]
    public async Task CancelCheckoutReturnsToCart()
    {
        await LoginAsync(TestUsers.StandardUser, TestUsers.Password);

        // Add an item and go to cart.
        var items = Page.Locator("[data-test='inventory-item']");
        await items.Nth(0).Locator("button", new() { HasTextString = "Add to cart" }).ClickAsync();
        await Page.Locator("[data-test='shopping-cart-link']").ClickAsync();
        await Expect(Page.Locator("[data-test='inventory-item']")).ToHaveCountAsync(1);

        // Start checkout, then cancel.
        await Page.Locator("[data-test='checkout']").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*checkout-step-one\\.html"));
        await Page.Locator("[data-test='cancel']").ClickAsync();

        // Verify we're back in the cart with the item still present.
        await Expect(Page).ToHaveURLAsync(new Regex(".*cart\\.html"));
        await Expect(Page.Locator("[data-test='inventory-item']")).ToHaveCountAsync(1);
    }
}
