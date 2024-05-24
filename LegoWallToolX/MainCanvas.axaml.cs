using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LegoWallToolX.Entities;
using LegoWallToolX.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LegoWallToolX;

/// <summary>
/// 主画布
/// </summary>
public partial class MainCanvas : UserControl
{
    #region constructor
    public MainCanvas()
    {
        InitializeComponent();

        _canvas.Focusable = true;
        _canvas.PointerPressed += Canvas_PointerPressed;
        _canvas.PointerMoved += Canvas_PointerMoved;
        _canvas.KeyDown += Canvas_KeyDown;
        _canvas.KeyUp += Canvas_KeyUp;
    }
    #endregion

    #region property
    private FileItem? _fileItem;
    private readonly CanvasRenderingConfigItem _crConfig = new()
    {
        BasePlateColor = Colors.LightGray,
        PixelPerRect = 15,
        RowCount = 48,
        ColCount = 48,
        OffsetX = 100,
        OffsetY = 100
    };
    private readonly List<Rectangle> _rectList = [];
    private Point _pointerStartPosition;
    private Point _pointerEndPostion;
    #endregion

    #region method
    internal FileItem? GetFileItem()
    {
        if (_fileItem is null) return null;
        for (var i = 0; i < _rectList.Count; i++)
        {
            if (_rectList[i].Fill is SolidColorBrush brush) _fileItem.CanvasPixelColorItems[i].Color = brush.Color;
        }
        return _fileItem;
    }

    internal void InitCanvas(FileItem fileItem)
    {
        _fileItem = fileItem;
        _crConfig.BasePlateColor = _fileItem.BasePlateColor;
        _crConfig.RowCount = _fileItem.RowCount;
        _crConfig.ColCount = _fileItem.ColCount;
        AppSingleton.CurrentPenColor = _crConfig.BasePlateColor;

        var rectList = _fileItem.CanvasPixelColorItems.Select(x =>
        {
            var rect = new Rectangle
            {
                Fill = new SolidColorBrush(x.Color),
                Width = _crConfig.PixelPerRect,
                Height = _crConfig.PixelPerRect,
                Stroke = new SolidColorBrush(Colors.DarkGray),
                StrokeThickness = 1
            };
            var offsetX = _crConfig.OffsetX + x.ColNum * _crConfig.PixelPerRect;
            var offsetY = _crConfig.OffsetY + x.RowNum * _crConfig.PixelPerRect;
            Canvas.SetLeft(rect, offsetX);
            Canvas.SetTop(rect, offsetY);
            return rect;
        }).ToList();
        _rectList.Clear();
        _canvas.Children.Clear();
        _rectList.AddRange(rectList);
        _canvas.Children.AddRange(rectList);
    }

    private void MoveCanvas(double offsetX, double offsetY)
    {
        _crConfig.OffsetX += offsetX;
        _crConfig.OffsetY += offsetY;
        foreach (var rect in _rectList)
        {
            var left = Canvas.GetLeft(rect);
            var top = Canvas.GetTop(rect);
            Canvas.SetLeft(rect, left + offsetX);
            Canvas.SetTop(rect, top + offsetY);
        }
    }

    /// <summary>
    /// 绘制像素格
    /// </summary>
    /// <param name="idx">像素格序号</param>
    private void DrawPixel(int idx)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"当前绘制像素格 序列号 {idx}");
#endif
        if (idx < 0) return;
        var color = AppSingleton.CurrentPenColor;
        _rectList[idx].Fill = new SolidColorBrush(color);
        if (_fileItem != null) _fileItem.CanvasPixelColorItems[idx].Color = color;

        RectColorChanged?.Invoke(this, idx, color);
        CanvasPixelColorChanged?.Invoke(this, _fileItem);
    }

    /// <summary>
    /// 擦除像素格
    /// </summary>
    /// <param name="idx">像素格序号</param>
    private void ErasePixel(int idx)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"当前擦除像素格 序列号 {idx}");
#endif
        if (idx < 0) return;
        var color = _crConfig.BasePlateColor;
        _rectList[idx].Fill = new SolidColorBrush(color);
        if (_fileItem != null) _fileItem.CanvasPixelColorItems[idx].Color = color;

        RectColorChanged?.Invoke(this, idx, color);
        CanvasPixelColorChanged?.Invoke(this, _fileItem);
    }

    private int GetActivatedPixelIndex(double x, double y)
    {
        var offsetX = x - _crConfig.OffsetX;
        var offsetY = y - _crConfig.OffsetY;
        if (offsetX < 0 || offsetY < 0) return -1;
        var c = (int)(offsetX / _crConfig.PixelPerRect);
        var r = (int)(offsetY / _crConfig.PixelPerRect);
        if (c >= _crConfig.ColCount || r >= _crConfig.RowCount) return -1;
        //var idx = _fileItem.CanvasPixelColorItems.FindIndex(x => x.ColNum == c && x.RowNum == r);//单元格序列号
        var idx = r * _crConfig.ColCount + c;//单元格序列号
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"当前鼠标点击 第 {r} 行 第 {c} 列...单元格序列号 {idx}");
#endif
        return idx;
    }
    #endregion

    #region event handler
    private void Canvas_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var pos = e.GetPosition(_canvas);
        _pointerStartPosition = pos;
        _pointerEndPostion = pos;

        var pointerProperties = e.GetCurrentPoint(_canvas).Properties;
        var isLeftButtonPressed = pointerProperties.IsLeftButtonPressed;
        var isRightButtonPressed = pointerProperties.IsRightButtonPressed;
        switch (AppSingleton.CurrentEditMode)
        {
            case EditMode.Pen:
                if (!isLeftButtonPressed && !isRightButtonPressed) break;
                var idx = GetActivatedPixelIndex(pos.X, pos.Y);
                if (idx < 0) break;
                if (isLeftButtonPressed) DrawPixel(idx);
                else if (isRightButtonPressed) ErasePixel(idx);
                break;
        }
    }

    private void Canvas_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        _canvas.Focus();

        var pos = e.GetPosition(_canvas);
        _pointerEndPostion = pos;

        var pointerProperties = e.GetCurrentPoint(_canvas).Properties;
        var isLeftButtonPressed = pointerProperties.IsLeftButtonPressed;
        var isRightButtonPressed = pointerProperties.IsRightButtonPressed;
        switch (AppSingleton.CurrentEditMode)
        {
            case EditMode.None:
                break;
            case EditMode.Pen:
                if (!isLeftButtonPressed && !isRightButtonPressed) break;
                var idx = GetActivatedPixelIndex(pos.X, pos.Y);
                if (idx < 0) break;
                if (isLeftButtonPressed) DrawPixel(idx);
                else if (isRightButtonPressed) ErasePixel(idx);
                break;
            case EditMode.Eraser:
                break;
            case EditMode.MoveCanvas:
                if (!isLeftButtonPressed) break;
                var offsetX = _pointerEndPostion.X - _pointerStartPosition.X;
                var offsetY = _pointerEndPostion.Y - _pointerStartPosition.Y;
                MoveCanvas(offsetX, offsetY);
                _pointerStartPosition = _pointerEndPostion;
                break;
            default:
                break;
        }
    }

    private void Canvas_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"当前键盘按下 {e.Key}");
#endif
        switch (e.Key)
        {
            case Avalonia.Input.Key.Space:
                if (AppSingleton.CurrentEditMode == EditMode.MoveCanvas) break;
                AppSingleton.PreviousEditMode = AppSingleton.CurrentEditMode;
                AppSingleton.CurrentEditMode = EditMode.MoveCanvas;
                break;
        }
    }

    private void Canvas_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"当前键盘抬起 {e.Key}");
#endif
        switch (e.Key)
        {
            case Avalonia.Input.Key.Space:
                AppSingleton.CurrentEditMode = AppSingleton.PreviousEditMode;
                break;
        }
    }
    #endregion

    #region custom event
    internal delegate void RectColorChangedHandler(object sender, int idx, Color color);
    internal event RectColorChangedHandler? RectColorChanged;

    internal delegate void CanvasPixelColorChangedHandler(object sender, FileItem? fileItem);
    internal event CanvasPixelColorChangedHandler? CanvasPixelColorChanged;
    #endregion
}