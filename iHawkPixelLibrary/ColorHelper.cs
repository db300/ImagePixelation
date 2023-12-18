using System;
using System.Collections.Generic;
using System.Drawing;

namespace iHawkPixelLibrary
{
    public static class ColorHelper
    {
        /// <summary>
        /// 合并颜色
        /// </summary>
        public static Color MergeColor(List<Color> colors)
        {
            int r = 0, g = 0, b = 0;
            foreach (var color in colors)
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }
            var count = colors.Count;
            return Color.FromArgb(r / count, g / count, b / count);
        }

        /// <summary>
        /// 获取 colors 中，与 targetColor 最接近的颜色
        /// </summary>
        public static Color FindClosestColor(List<Color> colors, Color targetColor)
        {
            Color closestColor = colors[0];
            double minDistance = ColorDistance(colors[0], targetColor);
            foreach (Color color in colors)
            {
                double distance = ColorDistance(color, targetColor);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = color;
                }
            }
            return closestColor;
        }

        [Obsolete("建议使用FindClosestColor", true)]
        public static Color GetClosestColor(List<Color> colors, Color targetColor)
        {
            // 初始化最接近的颜色为列表中的第一个颜色
            Color closestColor = colors[0];
            // 计算每个颜色与目标颜色之间的距离
            foreach (Color color in colors)
            {
                double distance = ColorDistance(color, targetColor);
                if (distance < ColorDistance(closestColor, targetColor))
                {
                    closestColor = color;
                }
            }
            return closestColor;
        }

        private static double ColorDistance(Color c1, Color c2)
        {
            double redDiff = c1.R - c2.R;
            double greenDiff = c1.G - c2.G;
            double blueDiff = c1.B - c2.B;
            return Math.Sqrt(redDiff * redDiff + greenDiff * greenDiff + blueDiff * blueDiff);
        }
    }
}
