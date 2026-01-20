using System;
using Avalonia;
using Avalonia.Controls;
using Ikst.KeyboardHook;

namespace FastCircle;

public partial class MainWindow : Window
{
    private bool _shown;

    private KeyboardHook kh = new();
    private double _firstButtonPressedTime;

    public MainWindow()
    {
        InitializeComponent();

        kh.KeyDown += (_, e) =>
        {
            var t = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (e.Key == VirtualKeys.LCONTROL)
                _firstButtonPressedTime = t;
            else if (e.Key == VirtualKeys.LMENU)
                if (t - _firstButtonPressedTime < 200)
                {
                    _shown = !_shown;
                    SetWindowVisible(_shown);
                }
        };

        kh.Start();

        SetWindowVisible(_shown);
    }

    private void SetWindowVisible(bool visible)
    {
        WindowState = visible ? WindowState.Maximized : WindowState.Minimized;
        if (!visible) return;

        Width = Screens.Primary!.Bounds.Width;
        Height = Screens.Primary.Bounds.Height;
    }

    #region cheating by Windows system

    private void UserTryToResize(object? sender, SizeChangedEventArgs e)
    {
        if(!_shown) return;
        Width = Screens.Primary!.Bounds.Width;
        Height = Screens.Primary.Bounds.Height;
    }

    private void UserTryToChangePosition(object? sender, PixelPointEventArgs e)
    {
        if(_shown)
            Position = new PixelPoint(0, 0);
    }

    #endregion
}