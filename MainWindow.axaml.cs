using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Ikst.KeyboardHook;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;


namespace FastCircle;

public partial class MainWindow : Window
{
    private bool _shown => IsVisible;

    private KeyboardHook kh = new();
    private double _firstButtonPressedTime;

    private string _savePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "fastcircle.json");

    private Save _curSave;

    public MainWindow()
    {
        InitializeComponent();
        _curSave = GetSave();
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
        btn1.Click += (_, _) =>
        {
            if (string.IsNullOrEmpty(_curSave.paths[0]))
            {
                OpenFiledialogAsync(this, 0).ConfigureAwait(false).GetAwaiter();
                return;
            }

            Process.Start(_curSave.paths[0]);
        };
        var btn2 = CloneBtn(btn1, 1);
        var btn3 = CloneBtn(btn1, 2);
        var btn4 = CloneBtn(btn1, 3);
        var btn5 = CloneBtn(btn1, 4);

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

    private Button CloneBtn(Button btn, int index)
    {
        var btn1 = new Button
        {
            Width = btn.Width,
            Height = btn.Height,
            Background = btn.Background,
            CornerRadius = btn.CornerRadius
        };
        btn1.Click += (_, _) =>
        {
            if (string.IsNullOrEmpty(_curSave.paths[index]))
                OpenFiledialogAsync(this, index).ConfigureAwait(false).GetAwaiter();
            return;
            Process.Start(_curSave.paths[index]);
        };

        Process.Start(_curSave.paths[0]);
        return btn1;
    }

    private void SetBtnPos(Button btn)
    {
        var index = MainCanvas.Children.IndexOf(btn);

        var offset = 125;

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
                SetPos(btn, GetCursorPosition() + new Point(-offset, offset / 2));
                break;
            case 4:
                SetPos(btn, GetCursorPosition() + new Point(-offset * 1.33, -offset / 2));
                break;
        }

        //SetPos(btn,GetCursorPosition() - new Point(btn.Width / 2, btn.Height / 2) - new Point(0, offset));
    }


    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private Save GetSave()
    {
        if (!File.Exists(_savePath))
        {
            File.Create(_savePath).Close();
            return new Save();
        }

        var text = File.ReadAllText(_savePath);
        try
        {
            return JsonSerializer.Deserialize<Save>(text, _jsonSerializerOptions) ?? new Save();
        }
        catch (Exception e)
        {
            return new Save();
        }
    }

    private void SaveSave()
    {
        var text = JsonSerializer.Serialize(_curSave, _jsonSerializerOptions);
        File.WriteAllText(_savePath, text);
    }

    private readonly FilePickerOpenOptions _filePickerOptions = new()
    {
        Title = "Select file",
        AllowMultiple = false,
        FileTypeFilter =
        [
            new FilePickerFileType("Audio Files")
            {
                Patterns = ["*.exe"]
            },
            FilePickerFileTypes.All
        ]
    };

    public async Task OpenFiledialogAsync(Window parent, int index)
    {
        try
        {
            var storageProvider = parent.StorageProvider;

            var files = await storageProvider.OpenFilePickerAsync(_filePickerOptions);

            if (files.Count.Equals(0))
            {
                return;
            }

            var filePaths = new string[files.Count];
            for (var i = 0; i < files.Count; i++)
                filePaths[i] = files[i].Path.LocalPath;
            _curSave.paths[index] = filePaths[0];
            SaveSave();
        }
        catch (Exception ex)
        {
        }
    }

    [DllImport("Shell32.dll")]
    private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
}