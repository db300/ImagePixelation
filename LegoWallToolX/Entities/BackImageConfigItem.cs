using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallToolX.Entities
{
    /// <summary>
    /// 背景图配置实体
    /// </summary>
    public class BackImageConfigItem
    {
        /// <summary>
        /// 图片文件名
        /// </summary>
        public required string FileName { get; set; }

        /// <summary>
        /// 缩放比例
        /// </summary>
        public double ScaleRatio { get; set; }

        /// <summary>
        /// 横向位移(相对乐高图)
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 纵向位移(相对乐高图)
        /// </summary>
        public double OffsetY { get; set; }
    }
}
