namespace Athlet;
public partial class App : Application
{
    public App() { InitializeComponent(); }
    protected override Window CreateWindow(IActivationState? s) => new Window(new AppShell());
}
