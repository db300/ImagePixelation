using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using LegoWallToolX.Entities;
using System.Collections.Generic;

namespace LegoWallToolX;

/// <summary>
/// 新建窗口
/// </summary>
public partial class NewWindow : Window
{
    #region constructor
    public NewWindow()
    {
        InitializeComponent();

        _cmbRowCount.Items.Add(48);
        _cmbRowCount.Items.Add(96);
        _cmbRowCount.Items.Add(144);
        _cmbRowCount.SelectedIndex = 0;
        _cmbColCount.Items.Add(48);
        _cmbColCount.Items.Add(96);
        _cmbColCount.Items.Add(144);
        _cmbColCount.SelectedIndex = 0;
    }
    #endregion

    #region property
    #endregion

    #region event handler
    private void BtnCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void BtnOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var fileItem = new FileItem
        {
            RowCount = 48,
            ColCount = 48,
            BasePlateColor = Colors.LightGray,
            CanvasPixelColorItems = new List<CanvasPixelColorItem>()
        };
        if (_cmbRowCount.SelectedItem is int rowCount) fileItem.RowCount = rowCount;
        if (_cmbColCount.SelectedItem is int colCount) fileItem.ColCount = colCount;
        if (_cmbBaseColor.SelectedItem is ComboBoxItem baseColorItem &&
            baseColorItem.Content is Panel panel &&
            panel.Children[0] is Rectangle rect &&
            rect.Fill is IImmutableSolidColorBrush brush) fileItem.BasePlateColor = brush.Color;
        Close(fileItem);
    }
    #endregion
}