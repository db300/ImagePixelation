using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LegoWallToolX.Entities;
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

        _canvas.PointerPressed += Canvas_PointerPressed;
    }
    #endregion

    #region property
    private readonly CanvasRenderingConfigItem _crConfig = new()
    {
        PixelPerRect = 15,
        RowCount = 48,
        ColCount = 48,
        OffsetX = 100,
        OffsetY = 100
    };
    private readonly List<CanvasRenderingColorItem> _crColorList = [];
    private readonly List<Rectangle> _rectList = [];
    private readonly Color _basePlateColor = Colors.LightGray;//底板颜色
    #endregion

    #region method
    internal void InitCanvas(FileItem fileItem)
    {
        _crConfig.RowCount = fileItem.RowCount;
        _crConfig.ColCount = fileItem.ColCount;

        _crColorList.Clear();
        _rectList.Clear();
        _canvas.Children.Clear();
        for (var r = 0; r < _crConfig.RowCount; r++)
        {
            for (var c = 0; c < _crConfig.ColCount; c++)
            {
                var crColor = new CanvasRenderingColorItem { ColNum = c, RowNum = r, Color = _basePlateColor };
                _crColorList.Add(crColor);

                var rect = new Rectangle
                {
                    Fill = new SolidColorBrush(crColor.Color),
                    Width = _crConfig.PixelPerRect,
                    Height = _crConfig.PixelPerRect,
                    Stroke = new SolidColorBrush(Colors.DarkGray),
                    StrokeThickness = 1
                };
                var offsetX = _crConfig.OffsetX + crColor.ColNum * _crConfig.PixelPerRect;
                var offsetY = _crConfig.OffsetY + crColor.RowNum * _crConfig.PixelPerRect;
                Canvas.SetLeft(rect, offsetX);
                Canvas.SetTop(rect, offsetY);
                _rectList.Add(rect);
                _canvas.Children.Add(rect);
            }
        }
    }
    #endregion

    #region event handler
    private void Canvas_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var pos = e.GetPosition(_canvas);
        var offsetX = pos.X - _crConfig.OffsetX;
        var offsetY = pos.Y - _crConfig.OffsetY;
        if (offsetX < 0 || offsetY < 0) return;
        var c = (int)(offsetX / _crConfig.PixelPerRect);
        var r = (int)(offsetY / _crConfig.PixelPerRect);
        var idx = _crColorList.FindIndex(x => x.ColNum == c && x.RowNum == r);//单元格序列号
        //var idx1 = r * _crConfig.ColCount + c;//单元格序列号
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"当前鼠标点击 第 {r} 行 第 {c} 列...单元格序列号 {idx}");
#endif
        if (idx < 0) return;
        var color = AppSingleton.CurrentPenColor;
        if (e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed)
        {
        }
        else if (e.GetCurrentPoint(_canvas).Properties.IsRightButtonPressed)
        {
            color = _basePlateColor;
        }
        _crColorList[idx].Color = color;
        _rectList[idx].Fill = new SolidColorBrush(color);
    }
    #endregion
}