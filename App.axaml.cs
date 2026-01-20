using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Ikst.KeyboardHook;

namespace FastCircle;

public partial class App : Application
{
    private KeyboardHook kh = new();
    private double _firstButtonPressedTime;
    
    private MainWindow _mainWindow;
    private bool _userUsedIt;
    
    public override void Initialize()
    {
        kh.KeyDown += (_, e) =>
        {
            var t = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (e.Key == VirtualKeys.LCONTROL)
                _firstButtonPressedTime = t;
            else if (e.Key == VirtualKeys.LMENU)
                if (t - _firstButtonPressedTime < 200)
                {
                    if (_mainWindow == null)
                    {
                        _userUsedIt = true;
                        OnFrameworkInitializationCompleted();
                    }
                    SetWindowVisible(!_mainWindow.IsVisible);
                }
        };
        kh.Start();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if(!_userUsedIt) return;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            _mainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void SetWindowVisible(bool visible)
    {
        if (!visible)
        {
            _mainWindow.Hide();
            _mainWindow.Topmost = false;
        }
        else
        {
            _mainWindow.Show();
            _mainWindow.Topmost = true;
            _mainWindow.Width = _mainWindow.Screens.Primary!.Bounds.Width;
            _mainWindow.Height = _mainWindow.Screens.Primary.Bounds.Height;
            _mainWindow.Position = new PixelPoint(0, 0);
        }
    }
}