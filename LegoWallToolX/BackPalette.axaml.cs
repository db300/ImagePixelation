using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LegoWallToolX;

public partial class BackPalette : UserControl
{
    #region constructor
    public BackPalette()
    {
        InitializeComponent();

        _txtOffsetX.TextChanged += TxtLayout_TextChanged;
        _txtOffsetY.TextChanged += TxtLayout_TextChanged;
        _txtScaleRatio.TextChanged += TxtLayout_TextChanged;
    }
    #endregion

    #region event handler
    private void TxtLayout_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(_txtOffsetX.Text, out var offsetX) && int.TryParse(_txtOffsetY.Text, out var offsetY) && double.TryParse(_txtScaleRatio.Text, out var scaleRatio)) LayoutChanged?.Invoke(this, offsetX, offsetY, scaleRatio);
    }
    #endregion

    #region custom event
    public delegate void LayoutChangedEventHandler(object? sender, int offsetX, int offsetY, double scaleRatio);
    public event LayoutChangedEventHandler? LayoutChanged;
    #endregion
}