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
    private static readonly List<ColorPaletteItem> _availableColors =
    [
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
    ];
    #endregion

    #region event handler
    private void Border_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        _grid.Children.OfType<Border>().ToList().ForEach(x =>
        {
            x.BorderBrush = Brushes.Transparent;
        });
        if (sender is Border border)
        {
            border.BorderBrush = Brushes.DarkSlateGray;
            AppSingleton.CurrentPenColor = _availableColors[_grid.Children.IndexOf(border)].Color;
        }
    }
    #endregion

    #region ui
    private void InitUi()
    {
        _grid.Rows = 4;
        _grid.Columns = 3;
        var borderedCanvases = _availableColors.Select(x =>
        {
            var border = new Border
            {
                Background = new SolidColorBrush(x.Color),
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(5),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            };
            border.PointerPressed += Border_PointerPressed;
            return border;
        });
        _grid.Children.AddRange(borderedCanvases);
    }
    #endregion
}