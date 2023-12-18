using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallDesignTool
{
    internal class CanvasRendingConfigItem
    {
        /// <summary>
        /// 每个方格像素数
        /// </summary>
        public int PixelPerRect { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// 列数
        /// </summary>
        public int ColCount { get; set; }
        /// <summary>
        /// 横向位移
        /// </summary>
        public int OffsetX { get; set; }
        /// <summary>
        /// 纵向位移
        /// </summary>
        public int OffsetY { get; set; }
    }

    internal class CanvasRendingColorItem
    {
        /// <summary>
        /// 单元格列号
        /// </summary>
        public int ColNum { get; set; }
        /// <summary>
        /// 单元格行号
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 单元格颜色
        /// </summary>
        public Color Color { get; set; }
    }
}
