using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallToolX.Entities
{
    internal class ColorPaletteItem
    {
        /// <summary>
        /// 颜色名称
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public required Color Color { get; set; }
    }
}
