using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LegoWallToolX
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();

            FontFamily = new("Microsoft YaHei UI,Simsun,苹方-简,宋体-简");
            Title = $"乐高墙工具 {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        #endregion

        #region event handler
        private void New_Click(object sender, RoutedEventArgs e)
        {
            AppSingleton.FileItem = new Entities.FileItem { ColCount = 48, RowCount = 48 };
            _mainCanvas.InitCanvas(AppSingleton.FileItem);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Event handling logic goes here
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Event handling logic goes here
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            // Event handling logic goes here
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AppSingleton.FileItem = new Entities.FileItem { ColCount = 48, RowCount = 48 };
            _mainCanvas.InitCanvas(AppSingleton.FileItem);
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion
    }
}