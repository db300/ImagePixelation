using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LegoWallToolX.Entities;
using LegoWallToolX.Enums;
using System;
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
    internal FileItem? FileItem => _fileItem;
    private FileItem? _fileItem;
    private readonly CanvasRenderingConfigItem _crConfig = new()
    {
        BasePlateColor = Colors.LightGray,
        PixelPerRect = 15,
        RowColNumberVisible = true,
        RowCount = 48,
        ColCount = 48,
        OffsetX = 100,
        OffsetY = 100
    };
    private readonly List<Rectangle> _rectList = [];
    private readonly List<TextBlock> _rowTextList4Left = [];
    private readonly List<TextBlock> _rowTextList4Right = [];
    private readonly List<TextBlock> _colTextList4Top = [];
    private readonly List<TextBlock> _colTextList4Bottom = [];
    private Image _backImage;
    private Point _pointerStartPosition;
    private Point _pointerEndPostion;
    private const double NumRatio = 0.6; // 行列号字体大小与像素格大小的比例
    #endregion

    #region method
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
                Fill = new SolidColorBrush(x.Color, x.IsBase ? 0.5 : 1),
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
        _rectList.AddRange(rectList);

        for (var i = 0; i < _crConfig.RowCount; i++)
        {
            //左侧行号
            var textBlockLeft = CreateRowColNumTextBlock((i + 1).ToString(), _crConfig.OffsetX - _crConfig.PixelPerRect, _crConfig.OffsetY + i * _crConfig.PixelPerRect, TextAlignment.Right, _crConfig.PixelPerRect);
            _rowTextList4Left.Add(textBlockLeft);
            //右侧行号
            var textBlockRight = CreateRowColNumTextBlock((i + 1).ToString(), _crConfig.OffsetX + _crConfig.ColCount * _crConfig.PixelPerRect, _crConfig.OffsetY + i * _crConfig.PixelPerRect, TextAlignment.Left, _crConfig.PixelPerRect);
            _rowTextList4Right.Add(textBlockRight);
        }

        for (var i = 0; i < _crConfig.ColCount; i++)
        {
            //上方列号
            var textBlockTop = CreateRowColNumTextBlock((i + 1).ToString(), _crConfig.OffsetX + i * _crConfig.PixelPerRect, _crConfig.OffsetY - _crConfig.PixelPerRect, TextAlignment.Center, _crConfig.PixelPerRect);
            _colTextList4Top.Add(textBlockTop);
            //下方列号
            var textBlockBottom = CreateRowColNumTextBlock((i + 1).ToString(), _crConfig.OffsetX + i * _crConfig.PixelPerRect, _crConfig.OffsetY + _crConfig.RowCount * _crConfig.PixelPerRect, TextAlignment.Center, _crConfig.PixelPerRect);
            _colTextList4Bottom.Add(textBlockBottom);
        }

        _canvas.Children.Clear();
        _canvas.Children.AddRange(rectList);
        _canvas.Children.AddRange(_rowTextList4Left);
        _canvas.Children.AddRange(_rowTextList4Right);
        _canvas.Children.AddRange(_colTextList4Top);
        _canvas.Children.AddRange(_colTextList4Bottom);
    }

    internal void ImportBack(string localPath)
    {
        if (_fileItem is null) return;
        _fileItem.BackImageItem = new BackImageConfigItem { FileName = localPath, ScaleRatio = 1, OffsetX = 0, OffsetY = 0 };
        _backImage = new Image
        {
            Source = new Bitmap(localPath),
            RenderTransform = new ScaleTransform(1, 1)
        };
        var offsetX = _crConfig.OffsetX + 0;
        var offsetY = _crConfig.OffsetY + 0;
        Canvas.SetLeft(_backImage, offsetX);
        Canvas.SetTop(_backImage, offsetY);
        _canvas.Children.Insert(0, _backImage);
    }

    internal void UpdateBackLayout(int offsetX, int offsetY, double scaleRatio)
    {
        if (_backImage is null) return;
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(new ScaleTransform(scaleRatio, scaleRatio));
        transformGroup.Children.Add(new TranslateTransform(offsetX, offsetY));
        _backImage.RenderTransform = transformGroup;
    }

    internal void UpdateView4RowColNumVisible(bool visible)
    {
        _crConfig.RowColNumberVisible = visible;

        _rowTextList4Left.ForEach(a => a.IsVisible = visible);
        _rowTextList4Right.ForEach(a => a.IsVisible = visible);
        _colTextList4Top.ForEach(a => a.IsVisible = visible);
        _colTextList4Bottom.ForEach(a => a.IsVisible = visible);
    }

    internal void UpdateView4PixelPerRect(int pixelPerRect)
    {
        if (pixelPerRect <= 0) return;
        // 更新像素大小
        _crConfig.PixelPerRect = pixelPerRect;
        foreach (var rect in _rectList)
        {
            rect.Width = pixelPerRect;
            rect.Height = pixelPerRect;
        }
        SetRowColNumBlockSize(_rowTextList4Left, pixelPerRect);
        SetRowColNumBlockSize(_rowTextList4Right, pixelPerRect);
        SetRowColNumBlockSize(_colTextList4Top, pixelPerRect);
        SetRowColNumBlockSize(_colTextList4Bottom, pixelPerRect);
        // 重新排列位置
        for (int i = 0; i < _rectList.Count; i++)
        {
            var rect = _rectList[i];
            var pixelItem = _fileItem?.CanvasPixelColorItems[i];
            if (pixelItem != null)
            {
                var offsetX = _crConfig.OffsetX + pixelItem.ColNum * pixelPerRect;
                var offsetY = _crConfig.OffsetY + pixelItem.RowNum * pixelPerRect;
                Canvas.SetLeft(rect, offsetX);
                Canvas.SetTop(rect, offsetY);
            }
        }
        UpdateRowColNumBlockPosition(_rowTextList4Left, i => _crConfig.OffsetX - pixelPerRect, i => _crConfig.OffsetY + i * pixelPerRect);
        UpdateRowColNumBlockPosition(_rowTextList4Right, i => _crConfig.OffsetX + _crConfig.ColCount * pixelPerRect, i => _crConfig.OffsetY + i * pixelPerRect);
        UpdateRowColNumBlockPosition(_colTextList4Top, i => _crConfig.OffsetX + i * pixelPerRect, i => _crConfig.OffsetY - pixelPerRect);
        UpdateRowColNumBlockPosition(_colTextList4Bottom, i => _crConfig.OffsetX + i * pixelPerRect, i => _crConfig.OffsetY + _crConfig.RowCount * pixelPerRect);
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
        UpdateRowColNumBlockPosition(_rowTextList4Left, offsetX, offsetY);
        UpdateRowColNumBlockPosition(_rowTextList4Right, offsetX, offsetY);
        UpdateRowColNumBlockPosition(_colTextList4Top, offsetX, offsetY);
        UpdateRowColNumBlockPosition(_colTextList4Bottom, offsetX, offsetY);
        if (_backImage != null)
        {
            var backLeft = Canvas.GetLeft(_backImage);
            var backTop = Canvas.GetTop(_backImage);
            Canvas.SetLeft(_backImage, backLeft + offsetX);
            Canvas.SetTop(_backImage, backTop + offsetY);
        }
    }

    private void MoveBackImage(double offsetX, double offsetY)
    {
        if (_backImage is null) return;
        _fileItem.BackImageItem.OffsetX += offsetX;
        _fileItem.BackImageItem.OffsetY += offsetY;

        var backLeft = Canvas.GetLeft(_backImage);
        var backTop = Canvas.GetTop(_backImage);
        Canvas.SetLeft(_backImage, backLeft + offsetX);
        Canvas.SetTop(_backImage, backTop + offsetY);
    }

    /// <summary>
    /// 绘制像素格
    /// </summary>
    /// <param name="idx">像素格序号</param>
    private void DrawPixel(int idx)
    {
        if (idx < 0) return;
        var color = AppSingleton.CurrentPenColor;
        _rectList[idx].Fill = new SolidColorBrush(color);
        if (_fileItem != null)
        {
            _fileItem.CanvasPixelColorItems[idx].Color = color;
            _fileItem.CanvasPixelColorItems[idx].IsBase = false;
        }

        CanvasPixelColorChanged?.Invoke(this, _fileItem);
    }

    /// <summary>
    /// 擦除像素格
    /// </summary>
    /// <param name="idx">像素格序号</param>
    private void ErasePixel(int idx)
    {
        if (idx < 0) return;
        var color = _crConfig.BasePlateColor;
        _rectList[idx].Fill = new SolidColorBrush(color, 0.5);
        if (_fileItem != null)
        {
            _fileItem.CanvasPixelColorItems[idx].Color = color;
            _fileItem.CanvasPixelColorItems[idx].IsBase = true;
        }

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

    private static TextBlock CreateRowColNumTextBlock(string text, double left, double top, TextAlignment alignment, int pixelPerRect)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = pixelPerRect * NumRatio,
            TextAlignment = alignment,
            Width = pixelPerRect,
            Height = pixelPerRect
        };
        Canvas.SetLeft(textBlock, left);
        Canvas.SetTop(textBlock, top);
        return textBlock;
    }

    /// <summary>
    /// 设置行列号显示的块大小
    /// </summary>
    private static void SetRowColNumBlockSize(List<TextBlock> textBlocks, int pixelPerRect)
    {
        foreach (var textBlock in textBlocks)
        {
            textBlock.Width = pixelPerRect;
            textBlock.Height = pixelPerRect;
            textBlock.FontSize = pixelPerRect * NumRatio;
        }
    }

    private static void UpdateRowColNumBlockPosition(List<TextBlock> textBlocks, double offsetX, double offsetY)
    {
        foreach (var textBlock in textBlocks)
        {
            var left = Canvas.GetLeft(textBlock);
            var top = Canvas.GetTop(textBlock);
            Canvas.SetLeft(textBlock, left + offsetX);
            Canvas.SetTop(textBlock, top + offsetY);
        }
    }

    private static void UpdateRowColNumBlockPosition(List<TextBlock> textBlocks, Func<int, double> calcLeft, Func<int, double> calcTop)
    {
        for (int i = 0; i < textBlocks.Count; i++)
        {
            var textBlock = textBlocks[i];
            Canvas.SetLeft(textBlock, calcLeft(i));
            Canvas.SetTop(textBlock, calcTop(i));
        }
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
    internal delegate void CanvasPixelColorChangedHandler(object sender, FileItem? fileItem);
    internal event CanvasPixelColorChangedHandler? CanvasPixelColorChanged;
    #endregion
}