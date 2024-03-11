using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using LegoWallToolX.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LegoWallToolX;

/// <summary>
/// 调色板
/// </summary>
public partial class ColorPalette : UserControl
{
    #region constructor
    public ColorPalette()
    {
        InitializeComponent();

        InitUi();
    }
    #endregion

    #region property
    private static readonly List<ColorPaletteItem> _availableColors = new()
    {
        new ColorPaletteItem { Color = Color.FromRgb(255, 0, 0), Name = "红色" },
        new ColorPaletteItem { Color = Color.FromRgb(0, 0, 255), Name = "蓝色" },
        new ColorPaletteItem { Color = Color.FromRgb(0, 255, 0), Name = "绿色" },
        new ColorPaletteItem { Color = Color.FromRgb(255, 255, 0), Name = "黄色" },
        new ColorPaletteItem { Color = Color.FromRgb(255, 255, 255), Name = "白色" },
        new ColorPaletteItem { Color = Color.FromRgb(0, 0, 0), Name = "黑色" },
        new ColorPaletteItem { Color = Color.FromRgb(255, 182, 193), Name = "粉色" },
        new ColorPaletteItem { Color = Color.FromRgb(255, 165, 0), Name = "橙色" },
        new ColorPaletteItem { Color = Color.FromRgb(165, 42, 42), Name = "棕色" },
        new ColorPaletteItem { Color = Color.FromRgb(128, 128, 128), Name = "灰色" },
        new ColorPaletteItem { Color = Color.FromRgb(128, 0, 128), Name = "紫色" },
        new ColorPaletteItem { Color = Color.FromRgb(218, 165, 32), Name = "金色" }
    };
    #endregion

    #region ui
    private void InitUi()
    {
        _grid.Rows = 4;
        _grid.Columns = 3;
        var borderedCanvases = _availableColors.Select(x =>
        {
            var canvas = new Canvas
            {
                Background = new SolidColorBrush(x.Color),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            };
            canvas.PointerPressed += Canvas_PointerPressed;
            var border = new Border
            {
                Background = new SolidColorBrush(x.Color),
                Child = canvas,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            };
            // Create a new style
            ///canvas.Styles.Add())
            return border;
        });
        _grid.Children.AddRange(borderedCanvases);
        /*
        var buttons = _availableColors.Select(x =>
        {
            var button = new ToggleButton
            {
                Background = new SolidColorBrush(x.Color),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            };
            button.SetValue(ToolTip.TipProperty, x.Name);
            return button;
        });
        _grid.Children.AddRange(buttons);
        */
        /*
        _availableColors.ForEach(x =>
        {
            var button = new ToggleButton { Background = new SolidColorBrush(x.Color) };
            button.SetValue(ToolTip.TipProperty, x.Name);
        });
        */
    }

    private void Canvas_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var canvas = sender as Canvas;
        var border = canvas.Parent as Border;
        border.BorderThickness = new Thickness(2);
        border.BorderBrush = Brushes.Black;
    }
    #endregion
}