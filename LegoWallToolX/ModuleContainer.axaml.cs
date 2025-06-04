using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LegoWallToolX;

/// <summary>
/// 模块容器
/// </summary>
public partial class ModuleContainer : UserControl
{
    #region constructor
    public ModuleContainer()
    {
        InitializeComponent();
    }
    #endregion

    #region method
    public void AddModule(UserControl module, string title, string tip)
    {
        if (Content is TabControl tabControl)
        {
            var dockPanel = new DockPanel();
            dockPanel.Children.Add(module);
            var textBlock = new TextBlock
            {
                Text = title,
                FontSize = 16,
                MinHeight = 20
            };
            textBlock.SetValue(ToolTip.TipProperty, tip);
            var tabItem = new TabItem
            {
                Header = textBlock,
                Content = dockPanel,
                MinHeight = 25
            };
            tabControl.Items.Add(tabItem);

            tabControl.SelectedItem = tabItem;
        }
    }

    public void CloseModule()
    {
        if (Content is TabControl tabControl && tabControl.SelectedItem is TabItem tabItem)
        {
            tabControl.Items.Remove(tabItem);
        }
    }

    public T? GetModule<T>() where T : class
    {
        if (this.Content is TabControl tabControl && tabControl.SelectedContent is DockPanel dockPanel)
        {
            return dockPanel.Children[0] as T;
        }
        return null;
    }

    public void UpdateModuleTitle(string title, string tip)
    {
        if (Content is TabControl tabControl && tabControl.SelectedItem is TabItem tabItem && tabItem.Header is TextBlock textBlock)
        {
            textBlock.Text = title;
            textBlock.SetValue(ToolTip.TipProperty, tip);
        }
    }
    #endregion
}