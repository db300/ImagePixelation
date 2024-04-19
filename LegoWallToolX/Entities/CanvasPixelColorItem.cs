using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallToolX.Entities
{
    /// <summary>
    /// 画布像素颜色实体
    /// </summary>
    public class CanvasPixelColorItem
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
