namespace Athlet;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("product", typeof(ProductPage));
        Routing.RegisterRoute("checkout", typeof(CheckoutPage));
    }
}
