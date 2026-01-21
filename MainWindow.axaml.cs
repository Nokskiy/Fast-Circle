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
            Background = Brushes.Red,
            CornerRadius = new CornerRadius(50)
        };
        var btn2 = CloneBtn(btn1);
        var btn3 = CloneBtn(btn1);
        var btn4 = CloneBtn(btn1);
        var btn5 = CloneBtn(btn1);

        btns.AddRange(new List<Button> { btn1, btn2, btn3, btn4, btn5 });
        btns.ForEach(x => MainCanvas.Children.Add(x));
        btns.ForEach(x => SetBtnPos(x));
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

        public static POINT operator +(POINT point1, POINT point2)
        {
            return new POINT { X = point1.X + point2.X, Y = point1.Y + point2.Y };
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
        Canvas.SetLeft(control, point.X);
        Canvas.SetTop(control, point.Y);
    }

    private Button CloneBtn(Button btn)
    {
        return new Button
        {
            Width = btn.Width,
            Height = btn.Height,
            Background = btn.Background,
            CornerRadius = btn.CornerRadius,
        };
    }

    private void SetBtnPos(Button btn)
    {
        var index = MainCanvas.Children.IndexOf(btn);
        var c = MainCanvas.Children.Count;

        var offset = 125;

        if (index == 0)
        {
        }

        switch (index)
        {
            case 0:
                SetPos(btn, GetCursorPosition() - new Point(btn.Width / 2, offset));
                break;
            case 1:
                SetPos(btn, GetCursorPosition() + new Point(offset / 2, -offset / 2));
                break;
            case 2:
                SetPos(btn, GetCursorPosition() + new Point(offset / 4, offset / 2));
                break;
            case 3:
                SetPos(btn, GetCursorPosition() + new Point(-offset, offset/2));
                break;
            case 4:
                SetPos(btn, GetCursorPosition() + new Point(-offset*1.33, -offset / 2));
                break;
        }

        //SetPos(btn,GetCursorPosition() - new Point(btn.Width / 2, btn.Height / 2) - new Point(0, offset));
    }
}