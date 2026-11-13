using Android.App;
using Android.Runtime;
namespace Athlet;
[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr h, JniHandleOwnership o) : base(h, o) { }
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
