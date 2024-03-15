using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LegoWallToolX.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LegoWallToolX
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();

            Title = $"乐高墙工具 {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        #endregion

        #region event handler
        private async void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var result = await new NewWindow().ShowDialog<FileItem>(this);
            if (result is null) return;
            for (var r = 0; r < result.RowCount; r++)
            {
                for (var c = 0; c < result.ColCount; c++)
                {
                    result.CanvasPixelColorItems.Add(new CanvasPixelColorItem { RowNum = r, ColNum = c, Color = result.BasePlateColor });
                }
            }
            _mainCanvas.InitCanvas(result);
        }

        private async void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel is null) return;
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("legox文件") { Patterns = new List<string> { "*.legox" } },
                    new("所有文件") { Patterns = new List<string> { "*.*" } }
                }
            });
            if (!(files?.Count > 0)) return;
            var json = System.IO.File.ReadAllText(files[0].Path.LocalPath);
            var fileItem = JsonConvert.DeserializeObject<FileItem>(json);
            if (fileItem is null) return;
            _mainCanvas.InitCanvas(fileItem);
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel is null) return;
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                DefaultExtension = "legox",
                FileTypeChoices = new List<FilePickerFileType>
                 {
                     new("legox文件") { Patterns = new List<string> { "*.legox" } },
                     new("所有文件") { Patterns = new List<string> { "*.*" } }
                 }
            });
            if (file is null) return;
            var fileItem = _mainCanvas.GetFileItem();
            var json = JsonConvert.SerializeObject(fileItem);
            System.IO.File.WriteAllText(file.Path.LocalPath, json);
        }
        #endregion
    }
}