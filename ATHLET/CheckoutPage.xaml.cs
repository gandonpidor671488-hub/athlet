namespace Athlet;
public partial class CheckoutPage : ContentPage
{
    int total;
    public CheckoutPage() { InitializeComponent(); }
    protected override async void OnAppearing()
    {
        base.OnAppearing(); total = (await Store.Cart()).Sum(x => x.Sum); TotalL.Text = $"К оплате: {total:N0} ₽";
    }
    void OnMethod(object? s, CheckedChangedEventArgs e) => Address.IsVisible = Delivery.IsChecked;
    async void OnConfirm(object? s, EventArgs e)
    {
        var digits = new string((Phone.Text ?? "").Where(char.IsDigit).ToArray());
        if (total == 0) { Err.Text = "Корзина пуста"; return; }
        if (string.IsNullOrWhiteSpace(NameE.Text)) { Err.Text = "Введите имя"; return; }
        if (digits.Length < 10) { Err.Text = "Введите корректный телефон"; return; }
        if (Delivery.IsChecked && string.IsNullOrWhiteSpace(Address.Text)) { Err.Text = "Введите адрес доставки"; return; }
        var id = await Store.PlaceOrder(new Order
        {
            Name = NameE.Text.Trim(), Phone = Phone.Text!, Total = total, Created = DateTime.Now,
            Method = Delivery.IsChecked ? "Доставка" : "Самовывоз", Address = Delivery.IsChecked ? Address.Text!.Trim() : ""
        });
        Form.IsVisible = false; Done.IsVisible = true;
        DoneL.Text = $"Заказ №{id} оформлен!\nМы свяжемся с вами по телефону.";
    }
    async void OnHome(object? s, EventArgs e) => await Shell.Current.GoToAsync("//home");
}
