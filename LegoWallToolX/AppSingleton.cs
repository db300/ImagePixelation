using Avalonia.Media;
using LegoWallToolX.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoWallToolX
{
    internal static class AppSingleton
    {
        internal static FileItem? FileItem;

        internal static Color CurrentPenColor = Colors.LightGray;
    }
}
