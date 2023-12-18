using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallDesignTool
{
    /// <summary>
    /// 图像配置实体
    /// </summary>
    internal class ImageConfigItem
    {
        /// <summary>
        /// 横向截取位移
        /// </summary>
        public int OffsetX { get; set; }
        /// <summary>
        /// 纵向截取位移
        /// </summary>
        public int OffsetY { get; set; }
        /// <summary>
        /// 缩放比例
        /// </summary>
        public int ScaleRatio { get; set; }
        /// <summary>
        /// 像素化间隔
        /// </summary>
        public int Spacing { get; set; }
        /// <summary>
        /// 截取宽度
        /// </summary>
        public int CutOffWidth { get; set; }
        /// <summary>
        /// 截取高度
        /// </summary>
        public int CutOffHeight { get; set; }
    }
}
