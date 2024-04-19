using Avalonia.Media;
using System.Collections.Generic;

namespace LegoWallToolX.Entities
{
    /// <summary>
    /// 文件实体
    /// </summary>
    public class FileItem
    {
        /// <summary>
        /// 行数
        /// </summary>
        public required int RowCount { get; set; }
        /// <summary>
        /// 列数
        /// </summary>
        public required int ColCount { get; set; }
        /// <summary>
        /// 底板颜色
        /// </summary>
        public required Color BasePlateColor { get; set; }
        /// <summary>
        /// 画布像素颜色列表
        /// </summary>
        public required List<CanvasPixelColorItem> CanvasPixelColorItems { get; set; }
    }
}
