using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Ikst.KeyboardHook;
using System.Runtime.InteropServices;
using System.Windows;

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

    public void SpawnCircle()
    {
        var btns = new List<Button>();
        var btn1 = new Button
        {
            Width = 100,
            Height = 100,
            Background = Brushes.Red
        };

        SetPos(btn1, GetCursorPosition());
        btns.Add(btn1);
        btns.ForEach(x => MainCanvas.Children.Add(x));
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
    #region get mouse position
    //from https://stackoverflow.com/questions/1316681/getting-mouse-position-in-c-sharp
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    private Point GetCursorPosition()
    {
        POINT lpPoint;
        GetCursorPos(out lpPoint);
        return lpPoint;
    }
    #endregion

    private void SetPos(Control control, Point point)
    {
        Canvas.SetLeft(control, GetCursorPosition().X - control.Width / 2);
        Canvas.SetTop(control, GetCursorPosition().Y - control.Height / 2);
    }
}