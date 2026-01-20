using System;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Ikst.KeyboardHook;

namespace FastCircle;

public partial class MainWindow : Window
{
    private bool _shown => IsVisible;

    private KeyboardHook kh = new();
    private double _firstButtonPressedTime;

    public MainWindow()
    {
        InitializeComponent();
    }

    #region cheating by Windows system

    private void UserTryToResize(object? sender, SizeChangedEventArgs e)
    {
        if (!_shown) return;
        Width = Screens.Primary!.Bounds.Width;
        Height = Screens.Primary.Bounds.Height;
    }

    private void UserTryToChangePosition(object? sender, PixelPointEventArgs e)
    {
        if (_shown)
            Position = new PixelPoint(0, 0);
    }

    #endregion
}