namespace Athlet;
public partial class CatalogPage : ContentPage
{
    string cat = "Все";
    public CatalogPage() { InitializeComponent(); Cats.ItemsSource = new[] { "Все" }.Concat(Store.Categories).ToList(); }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Store.PendingCategory != null) cat = Store.PendingCategory;
        if (Store.PendingQuery != null) Search.Text = Store.PendingQuery;
        Store.PendingCategory = Store.PendingQuery = null;
        Cats.SelectedItem = cat;
        await Load();
    }
    async Task Load() => List.ItemsSource = await Store.Products(cat, Search.Text);
    async void OnText(object? s, TextChangedEventArgs e) => await Load();
    async void OnCat(object? s, SelectionChangedEventArgs e)
    {
        cat = e.CurrentSelection.FirstOrDefault() as string ?? "Все"; await Load();
    }
    async void OnProduct(object? s, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Product p) return;
        List.SelectedItem = null; await Shell.Current.GoToAsync($"product?id={p.Id}");
    }
}
