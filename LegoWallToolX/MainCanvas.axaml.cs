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
/// ������
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
    private readonly Color _basePlateColor = Colors.LightGray;//�װ���ɫ
    #endregion

    #region method
    internal void InitCanvas(FileItem fileItem)
    {
        _crConfig.RowCount = fileItem.RowCount;
        _crConfig.ColCount = fileItem.ColCount;

        _crColorList.Clear();
        for (var r = 0; r < _crConfig.RowCount; r++)
        {
            for (var c = 0; c < _crConfig.ColCount; c++)
            {
                _crColorList.Add(new CanvasRenderingColorItem { ColNum = c, RowNum = r, Color = _basePlateColor });
            }
        }

        PaintCanvas();
    }

    private void PaintCanvas()
    {
        _canvas.Children.Clear();
        _crColorList.ForEach(x =>
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
            _canvas.Children.Add(rect);
        });
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
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"��ǰ����� �� {r} �� �� {c} ��...");
#endif
        var idx = _crColorList.FindIndex(x => x.ColNum == c && x.RowNum == r);
        if (idx >= 0)
        {
            _crColorList[idx].Color = AppSingleton.CurrentPenColor;
            if (_canvas.Children[idx] is Rectangle rect)
            {
                rect.Fill = new SolidColorBrush(AppSingleton.CurrentPenColor);
            }
        }
    }
    #endregion
}