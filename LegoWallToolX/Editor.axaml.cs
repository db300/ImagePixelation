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
    public Editor(Entities.FileItem fileItem)
    {
        InitializeComponent();

        _mainCanvas.InitCanvas(fileItem);
    }
    #endregion

    #region property
    public Entities.FileItem? FileItem => _mainCanvas?.GetFileItem();
    #endregion
}