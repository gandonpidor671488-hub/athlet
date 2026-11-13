namespace Athlet;
public partial class CartPage : ContentPage
{
    public CartPage() { InitializeComponent(); }
    protected override async void OnAppearing() { base.OnAppearing(); await Reload(); }
    async Task Reload()
    {
        var l = await Store.Cart(); List.ItemsSource = l;
        TotalL.Text = $"Итого: {l.Sum(x => x.Sum):N0} ₽"; Checkout.IsEnabled = l.Count > 0;
    }
    static CartLine? Line(object? s) => (s as BindableObject)?.BindingContext as CartLine;
    async void OnPlus(object? s, EventArgs e) { if (Line(s) is CartLine l) { await Store.SetQty(l.P.Id, l.Qty + 1); await Reload(); } }
    async void OnMinus(object? s, EventArgs e) { if (Line(s) is CartLine l) { await Store.SetQty(l.P.Id, l.Qty - 1); await Reload(); } }
    async void OnDel(object? s, EventArgs e) { if (Line(s) is CartLine l) { await Store.SetQty(l.P.Id, 0); await Reload(); } }
    async void OnCheckout(object? s, EventArgs e) => await Shell.Current.GoToAsync("checkout");
}
