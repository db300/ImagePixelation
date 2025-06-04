using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LegoWallToolX.Enums;

namespace LegoWallToolX;

public partial class ViewPalette : UserControl
{
    #region constructor
    public ViewPalette()
    {
        InitializeComponent();

        _ckbRowColNumVisible.IsCheckedChanged += CkbRowColNumVisible_IsCheckedChanged;
        _txtPixelPerRect.TextChanged += TxtPixelPerRect_TextChanged;
    }
    #endregion

    #region event handler
    private void CkbRowColNumVisible_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var isVisible = _ckbRowColNumVisible.IsChecked ?? false;
        ViewChanged?.Invoke(this, ViewMode.RowColNumberVisible, isVisible);
    }

    private void TxtPixelPerRect_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(_txtPixelPerRect.Text, out var pixelPerRect) && pixelPerRect > 0)
        {
            ViewChanged?.Invoke(this, ViewMode.PixelPerRect, pixelPerRect);
        }
        else
        {
            _txtPixelPerRect.Text = "1"; // Reset to default if invalid input
        }
    }
    #endregion

    #region custom event
    internal delegate void ViewChangedEventHandler(object? sender, ViewMode viewMode, object? value);
    internal event ViewChangedEventHandler? ViewChanged;
    #endregion
}