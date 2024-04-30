using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using RiffViewer.UI.ViewModels;
using RiffViewer.UI.Views;
using static PrettyLogSharp.PrettyLogger;

namespace RiffViewer.UI;

public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        Log("App.OnFrameworkInitializationCompleted()");
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Log("ApplicationLifetime is IClassicDesktopStyleApplicationLifetime");
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            Log("ApplicationLifetime is ISingleViewApplicationLifetime");
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}