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
    public void AddModule(UserControl module)
    {
        if (this.Content is TabControl tabControl)
        {
            var dockPanel = new DockPanel();
            dockPanel.Children.Add(module);
            var textBlock = new TextBlock
            {
                Text = module.GetType().Name,
                FontSize = 16,
                MinHeight = 20
            };
            var tabItem = new TabItem
            {
                Header = textBlock,
                Content = dockPanel,
                MinHeight = 25
            };
            tabControl.Items.Add(tabItem);
        }
    }

    public void GetModule()
    {
        if(this.Content is TabControl tabControl)
        {
            System.Diagnostics.Debug.WriteLine(tabControl.SelectedContent);            
        }
    }
    #endregion
}