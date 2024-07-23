using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LegoWallToolX.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LegoWallToolX
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();

            InitUi();
        }
        #endregion

        #region property
        private int _docIndex;
        #endregion

        #region method
        private async void NewFile()
        {
            var result = await new NewWindow().ShowDialog<FileItem>(this);
            if (result is null) return;
            for (var r = 0; r < result.RowCount; r++)
            {
                for (var c = 0; c < result.ColCount; c++)
                {
                    result.CanvasPixelColorItems.Add(new CanvasPixelColorItem { RowNum = r, ColNum = c, Color = result.BasePlateColor, IsBase = true });
                }
            }
            var editor = new Editor(result);
            var title = $"未命名文档 - {++_docIndex}";
            _moduleContainer.AddModule(editor, title, title);
        }

        private async void OpenFile()
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
            var json = File.ReadAllText(files[0].Path.LocalPath);
            var fileItem = JsonConvert.DeserializeObject<FileItem>(json);
            if (fileItem is null) return;
            var editor = new Editor(fileItem);
            _moduleContainer.AddModule(editor, files[0].Name, files[0].Path.LocalPath);
        }

        private async void SaveFile()
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
            var editor = _moduleContainer.GetModule<Editor>();
            var fileItem = editor?.FileItem;
            var json = JsonConvert.SerializeObject(fileItem);
            File.WriteAllText(file.Path.LocalPath, json);
            _moduleContainer.UpdateModuleTitle(file.Name, file.Path.LocalPath);
        }

        private async void ImportBack()
        {
            var editor = _moduleContainer.GetModule<Editor>();
            if (editor is null) return;
            var topLevel = GetTopLevel(this);
            if (topLevel is null) return;
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("图片文件") { Patterns = new List<string> { "*.png", "*.jpg", "*.jpeg", "*.bmp" } },
                    new("所有文件") { Patterns = new List<string> { "*.*" } }
                }
            });
            if (!(files?.Count > 0)) return;
            editor.ImportBack(files[0].Path.LocalPath);
        }
        #endregion

        #region event handler
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            NewFile();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void MenuNew_Click(object? sender, RoutedEventArgs e)
        {
            NewFile();
        }

        private void MenuOpen_Click(object? sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void MenuSave_Click(object? sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void MenuImportBack_Click(object? sender, RoutedEventArgs e)
        {
            ImportBack();
        }

        private void NativeMenuNew_Click(object? sender, System.EventArgs e)
        {
            NewFile();
        }

        private void NativeMenuOpen_Click(object? sender, System.EventArgs e)
        {
            OpenFile();
        }

        private void NativeMenuSave_Click(object? sender, System.EventArgs e)
        {
            SaveFile();
        }

        private void NativeMenuImportBack_Click(object? sender, System.EventArgs e)
        {
            ImportBack();
        }
        #endregion

        #region ui
        private void InitUi()
        {
            Title = $"乐高墙工具 {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("当前操作系统是Windows");
#endif
                //文件菜单
                var menuFile = new MenuItem { Header = "文件" };
                var menuNew = new MenuItem { Header = "新建..." };
                menuNew.Click += MenuNew_Click;
                var menuOpen = new MenuItem { Header = "打开..." };
                menuOpen.Click += MenuOpen_Click;
                var menuSave = new MenuItem { Header = "保存..." };
                menuSave.Click += MenuSave_Click;
                menuFile.Items.Add(menuNew);
                menuFile.Items.Add(menuOpen);
                menuFile.Items.Add(menuSave);
                //编辑菜单
                var menuEdit = new MenuItem { Header = "编辑" };
                var menuImportBack = new MenuItem { Header = "导入背景图片..." };
                menuImportBack.Click += MenuImportBack_Click;
                menuEdit.Items.Add(menuImportBack);

                var menu = new Menu();
                menu.Items.Add(menuFile);
                menu.Items.Add(menuEdit);
                if (this.Content is DockPanel dockPanel)
                {
                    DockPanel.SetDock(menu, Dock.Top);
                    dockPanel.Children.Insert(0, menu);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("当前操作系统是OSX");
#endif
            }
        }
        #endregion
    }
}