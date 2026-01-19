using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace FastCircle;

public partial class MainWindow : Window
{
    private bool _shown;

    public MainWindow()
    {
        InitializeComponent();
        SetWindowVisible(!_shown);
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
        Width = Screens.Primary!.Bounds.Width;
        Height = Screens.Primary.Bounds.Height;
    }

    private void UserTryToChangePosition(object? sender, PixelPointEventArgs e) =>
        Position = new PixelPoint(0, 0);
    #endregion
}