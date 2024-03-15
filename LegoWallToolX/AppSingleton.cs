using Avalonia.Media;
using LegoWallToolX.Entities;
using LegoWallToolX.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallToolX
{
    internal static class AppSingleton
    {
        internal static Color CurrentPenColor = Colors.LightGray;//当前选中的颜色
        internal static EditMode CurrentEditMode = EditMode.Pen;//当前编辑模式
        internal static EditMode PreviousEditMode = EditMode.Pen;//上一个编辑模式
    }
}
