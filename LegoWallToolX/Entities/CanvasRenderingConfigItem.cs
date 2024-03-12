using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallToolX.Entities
{
    /// <summary>
    /// 画布渲染配置实体
    /// </summary>
    internal class CanvasRenderingConfigItem
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
}
