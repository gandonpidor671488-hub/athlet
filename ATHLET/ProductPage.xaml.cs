namespace Athlet;
public partial class ProductPage : ContentPage, IQueryAttributable
{
    int id;
    public ProductPage() { InitializeComponent(); }
    public async void ApplyQueryAttributes(IDictionary<string, object> q)
    {
        id = int.Parse(q["id"].ToString()!);
        if (await Store.Get(id) is not Product p) return;
        Title = p.Name; Emoji.Text = p.Emoji; Brand.Text = p.Brand; NameL.Text = p.Name;
        Weight.Text = "Масса упаковки: " + p.Weight; Price.Text = p.PriceText; Desc.Text = p.Description; Comp.Text = p.Composition;
    }
    async void OnAdd(object? s, EventArgs e)
    {
        await Store.AddToCart(id);
        Add.Text = "✓ Добавлено в корзину"; await Task.Delay(1200); Add.Text = "В корзину";
    }
}
