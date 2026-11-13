namespace Athlet;
public partial class HomePage : ContentPage
{
    public HomePage() { InitializeComponent(); Cats.ItemsSource = Store.Categories; }
    protected override async void OnAppearing() { base.OnAppearing(); Top.ItemsSource = (await Store.Products()).Take(4).ToList(); }
    async void OnSearch(object? s, EventArgs e)
    {
        Store.PendingQuery = Search.Text; Store.PendingCategory = "Все"; Search.Text = "";
        await Shell.Current.GoToAsync("//catalog");
    }
    async void OnCat(object? s, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not string c) return;
        Cats.SelectedItem = null; Store.PendingCategory = c; Store.PendingQuery = null;
        await Shell.Current.GoToAsync("//catalog");
    }
    async void OnProduct(object? s, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product p) return;
        Top.SelectedItem = null; await Shell.Current.GoToAsync($"product?id={p.Id}");
    }
}
