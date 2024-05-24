using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LegoWallToolX.Entities;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace LegoWallToolX;

/// <summary>
/// 编辑器
/// </summary>
public partial class Editor : UserControl
{
    #region constructor
    public Editor(FileItem fileItem)
    {
        InitializeComponent();

        _mainCanvas.InitCanvas(fileItem);
        _mainCanvas.CanvasPixelColorChanged += MainCanvas_CanvasPixelColorChanged;

        DrawPreviewCanvas(fileItem);
    }
    #endregion

    #region property
    public FileItem? FileItem => _mainCanvas?.GetFileItem();
    #endregion

    #region method
    private void DrawPreviewCanvas(FileItem? fileItem)
    {
        if (fileItem == null) return;

        const int unitSize = 1;
        var width = fileItem.ColCount * unitSize;
        var height = fileItem.RowCount * unitSize;
        using (var img = new SKBitmap(width, height))
        {
            using (var canvas = new SKCanvas(img))
            {
                canvas.Clear(SKColors.White);

                fileItem.CanvasPixelColorItems.ForEach(x =>
                {
                    using (var paint = new SKPaint { Color = new SKColor(x.Color.R, x.Color.G, x.Color.B, x.Color.A), IsAntialias = true })
                    {
                        var offsetX = x.ColNum * unitSize;
                        var offsetY = x.RowNum * unitSize;
                        canvas.DrawRect(offsetX, offsetY, unitSize, unitSize, paint);
                    }
                });
            }
            using (var stream = new MemoryStream())
            {
                img.Encode(stream, SKEncodedImageFormat.Png, 100);
                stream.Position = 0;
                var bitmap = new Bitmap(stream);
                _previewImage.Source = bitmap;
            }
        }
    }
    #endregion

    #region event handler
    private void MainCanvas_CanvasPixelColorChanged(object sender, FileItem? fileItem)
    {
        DrawPreviewCanvas(fileItem);
    }
    #endregion
}