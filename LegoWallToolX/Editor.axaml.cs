using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LegoWallToolX;

/// <summary>
/// 编辑器
/// </summary>
public partial class Editor : UserControl
{
    #region constructor
    public Editor(Entities.FileItem result)
    {
        InitializeComponent();

        _mainCanvas.InitCanvas(result);
    }
    #endregion
}