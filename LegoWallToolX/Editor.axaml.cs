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

        _backPalette.LayoutChanged += BackPalette_LayoutChanged;
        _viewPalette.ViewChanged += ViewPalette_ViewChanged;

        DrawPreviewCanvas(fileItem);
    }
    #endregion

    #region property
    public FileItem? FileItem => _mainCanvas?.FileItem;
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

    internal void ExportImage(string localPath)
    {
        var fileItem = FileItem;
        if (fileItem is null) return;
        const int unitSize = 15;
        const int padding = 15;
        var width = fileItem.ColCount * unitSize + 2 * padding;
        var height = fileItem.RowCount * unitSize + 2 * padding;
        using (var img = new SKBitmap(width, height))
        {
            using (var canvas = new SKCanvas(img))
            {
                canvas.Clear(SKColors.White);

                //绘制行号
                for (var i = 0; i < fileItem.RowCount; i++)
                {
                    var text = new SKPaint { Color = SKColors.Black, TextSize = 10, IsAntialias = true };
                    var rowNum = (i + 1).ToString();
                    canvas.DrawText(rowNum, 0, i * unitSize + padding + 10, text);
                    canvas.DrawText(rowNum, fileItem.ColCount * unitSize + padding, i * unitSize + padding + 10, text);
                }
                //绘制列号
                for (var i = 0; i < fileItem.ColCount; i++)
                {
                    var text = new SKPaint { Color = SKColors.Black, TextSize = 10, IsAntialias = true };
                    var colNum = (i + 1).ToString();
                    canvas.DrawText(colNum, i * unitSize + padding, padding - 5, text);
                    canvas.DrawText(colNum, i * unitSize + padding, fileItem.RowCount * unitSize + padding + 10, text);
                }

                fileItem.CanvasPixelColorItems.ForEach(x =>
                {
                    // 创建用于填充的画笔
                    using (var fillPaint = new SKPaint { Color = new SKColor(x.Color.R, x.Color.G, x.Color.B, x.Color.A), IsAntialias = true })
                    {
                        var offsetX = x.ColNum * unitSize + padding;
                        var offsetY = x.RowNum * unitSize + padding;
                        canvas.DrawRect(offsetX, offsetY, unitSize, unitSize, fillPaint);
                    }
                    // 创建用于绘制边框的画笔
                    using (var strokePaint = new SKPaint { Color = SKColors.DarkGray, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1 })
                    {
                        var offsetX = x.ColNum * unitSize + padding;
                        var offsetY = x.RowNum * unitSize + padding;
                        canvas.DrawRect(offsetX, offsetY, unitSize, unitSize, strokePaint);
                    }
                });

                //绘制中心线
                var linePaint = new SKPaint { Color = SKColors.OrangeRed, StrokeWidth = 1 };
                canvas.DrawLine(padding + fileItem.ColCount / 2 * unitSize, padding, padding + fileItem.ColCount / 2 * unitSize, padding + fileItem.RowCount * unitSize, linePaint);
                canvas.DrawLine(padding, padding + fileItem.RowCount / 2 * unitSize, padding + fileItem.ColCount * unitSize, padding + fileItem.RowCount / 2 * unitSize, linePaint);
            }
            using (var stream = new MemoryStream())
            {
                img.Encode(stream, SKEncodedImageFormat.Png, 100);
                stream.Position = 0;
                var bitmap = new Bitmap(stream);
                bitmap.Save(localPath);
            }
        }
    }

    internal void ImportBack(string localPath)
    {
        _mainCanvas.ImportBack(localPath);
    }
    #endregion

    #region event handler
    private void MainCanvas_CanvasPixelColorChanged(object sender, FileItem? fileItem)
    {
        DrawPreviewCanvas(fileItem);
    }

    private void BackPalette_LayoutChanged(object? sender, int offsetX, int offsetY, double scaleRatio)
    {
        _mainCanvas.UpdateBackLayout(offsetX, offsetY, scaleRatio);
    }

    private void ViewPalette_ViewChanged(object? sender, Enums.ViewMode viewMode, object? value)
    {
        switch (viewMode)
        {
            case Enums.ViewMode.RowColNumberVisible:
                if (value is bool visible) _mainCanvas.UpdateView4RowColNumVisible(visible);
                break;
            case Enums.ViewMode.PixelPerRect:
                if (value is int pixelPerRect && pixelPerRect > 0) _mainCanvas.UpdateView4PixelPerRect(pixelPerRect);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(viewMode), viewMode, null);
        }
    }
    #endregion
}