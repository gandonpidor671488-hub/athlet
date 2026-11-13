namespace Athlet;
public partial class InfoPage : ContentPage
{
    public InfoPage() { InitializeComponent(); }
    void OnCall(object? s, EventArgs e) { if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open("+78001234567"); }
}
