using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoWallDesignTool
{
    public partial class MainForm : Form
    {
        #region constructor
        public MainForm()
        {
            InitializeComponent();

            InitUi();

            Load += MainForm_Load;
        }
        #endregion

        #region property
        private PictureBox _picCanvas;
        private NumericUpDown _numRowCount;
        private NumericUpDown _numColCount;
        private Color _currentPenColor = Color.LightGray;
        private readonly Color BasePlateColor = Color.LightGray;
        private const int ControlMargin = 20;
        private const int ControlPadding = 12;
        private Bitmap _imgInput;

        private readonly CanvasRendingConfigItem _crc = new CanvasRendingConfigItem
        {
            PixelPerRect = 15,
            RowCount = 96,
            ColCount = 48,
            OffsetX = 100,
            OffsetY = 100
        };

        private static readonly List<CanvasRendingColorItem> _canvasRendingColorList = new List<CanvasRendingColorItem>();

        private static readonly List<ColorPaletteItem> _availableColors = new List<ColorPaletteItem>
        {
            new ColorPaletteItem { Color = Color.FromArgb(255, 0, 0), Name = "红色" },
            new ColorPaletteItem { Color = Color.FromArgb(0, 0, 255), Name = "蓝色" },
            new ColorPaletteItem { Color = Color.FromArgb(0, 255, 0), Name = "绿色" },
            new ColorPaletteItem { Color = Color.FromArgb(255, 255, 0), Name = "黄色" },
            new ColorPaletteItem { Color = Color.FromArgb(255, 255, 255), Name = "白色" },
            new ColorPaletteItem { Color = Color.FromArgb(0, 0, 0), Name = "黑色" },
            new ColorPaletteItem { Color = Color.FromArgb(255, 182, 193), Name = "粉色" },
            new ColorPaletteItem { Color = Color.FromArgb(255, 165, 0), Name = "橙色" },
            new ColorPaletteItem { Color = Color.FromArgb(165, 42, 42), Name = "棕色" },
            new ColorPaletteItem { Color = Color.FromArgb(128, 128, 128), Name = "灰色" },
            new ColorPaletteItem { Color = Color.FromArgb(128, 0, 128), Name = "紫色" },
            new ColorPaletteItem { Color = Color.FromArgb(218, 165, 32), Name = "金色" }
        };
        #endregion

        #region method
        private void ResetCanvas()
        {
            _canvasRendingColorList.Clear();
            for (var r = 0; r < _crc.RowCount; r++)
            {
                for (var c = 0; c < _crc.ColCount; c++)
                {
                    _canvasRendingColorList.Add(new CanvasRendingColorItem { ColNum = c, RowNum = r, Color = BasePlateColor });
                }
            }
            _picCanvas.Invalidate();
        }
        #endregion

        #region event handler
        private void MainForm_Load(object sender, EventArgs e)
        {
            _numRowCount.Value = _crc.RowCount;
            _numColCount.Value = _crc.ColCount;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            _crc.RowCount = (int)_numRowCount.Value;
            _crc.ColCount = (int)_numColCount.Value;
            ResetCanvas();
        }

        private void PicCanvas_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.Clear(Color.White);

            if (_canvasRendingColorList.Count == 0) return;

            foreach (var item in _canvasRendingColorList)
            {
                var offsetX = _crc.OffsetX + item.ColNum * _crc.PixelPerRect;
                var offsetY = _crc.OffsetY + item.RowNum * _crc.PixelPerRect;
                graphics.FillRectangle(new SolidBrush(item.Color), offsetX, offsetY, _crc.PixelPerRect, _crc.PixelPerRect);
                graphics.DrawRectangle(Pens.DarkGray, offsetX, offsetY, _crc.PixelPerRect, _crc.PixelPerRect);
            }
        }

        private void PicCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            var offsetX = e.X - _crc.OffsetX;
            var offsetY = e.Y - _crc.OffsetY;
            if (offsetX < 0 || offsetY < 0) return;
            var c = offsetX / _crc.PixelPerRect;
            var r = offsetY / _crc.PixelPerRect;
            var color = _currentPenColor;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;
                case MouseButtons.Right:
                    color = BasePlateColor;
                    break;
            }
            var item = _canvasRendingColorList.FirstOrDefault(a => a.ColNum == c && a.RowNum == r);
            if (item == null) return;
            item.Color = color;
            _picCanvas.Invalidate();
        }

        private void PicCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None) return;
            var offsetX = e.X - _crc.OffsetX;
            var offsetY = e.Y - _crc.OffsetY;
            if (offsetX < 0 || offsetY < 0) return;
            var c = offsetX / _crc.PixelPerRect;
            var r = offsetY / _crc.PixelPerRect;
            var color = _currentPenColor;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;
                case MouseButtons.Right:
                    color = BasePlateColor;
                    break;
            }
            var item = _canvasRendingColorList.FirstOrDefault(a => a.ColNum == c && a.RowNum == r);
            if (item == null) return;
            item.Color = color;
            _picCanvas.Invalidate();
        }

        private void PicCanvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void PicColor_Click(object sender, EventArgs e)
        {
            if (!(sender is PictureBox pic)) return;
            var color = pic.BackColor;
            _currentPenColor = color;
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            var openDlg = new OpenFileDialog
            {
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "打开图片",
            };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            _imgInput = new Bitmap(openDlg.FileName);

            var availableColors = _availableColors.Select(a => a.Color).ToList();
            var pixelSpacing = 4;
            for (var r = 0; r < _crc.RowCount; r++)
            {
                var offsetY = r * pixelSpacing;
                for (var c = 0; c < _crc.ColCount; c++)
                {
                    var offsetX = c * pixelSpacing;
                    var colorBlock = new List<Color>();
                    for (var i = 0; i < pixelSpacing; i++)
                    {
                        var x = offsetX + i;
                        var y = offsetY + i;
                        if (x >= _imgInput.Width || y >= _imgInput.Height) continue;
                        var color = _imgInput.GetPixel(x, y);
                        colorBlock.Add(color);
                    }
                    if (colorBlock.Count == 0) continue;
                    var mergedColor = iHawkPixelLibrary.ColorHelper.MergeColor(colorBlock);
                    var closestColor = iHawkPixelLibrary.ColorHelper.FindClosestColor(availableColors, mergedColor);
                    var item = _canvasRendingColorList.FirstOrDefault(a => a.ColNum == c && a.RowNum == r);
                    item.Color = closestColor;
                }
            }
            _picCanvas.Invalidate();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ResetCanvas();
        }
        #endregion

        #region ui
        private void InitUi()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Text = $"乐高墙设计工具 {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

            var panel = new Panel
            {
                Dock = DockStyle.Left,
                Parent = this,
                Width = 300
            };
            InitUi4Config(panel);

            panel = new Panel
            {
                Dock = DockStyle.Right,
                Parent = this,
                Width = 300
            };
            InitUi4ColorPalette(panel);

            panel = new Panel
            {
                Dock = DockStyle.Fill,
                Parent = this
            };
            panel.BringToFront();
            InitUi4Canvas(panel);
        }

        private void InitUi4Config(Panel panel)
        {
            var lbl = new Label
            {
                AutoSize = true,
                Location = new Point(ControlMargin, ControlMargin),
                Parent = panel,
                Text = "乐高行数："
            };
            _numRowCount = new NumericUpDown
            {
                Location = new Point(lbl.Right, lbl.Top - 3),
                Maximum = 10000,
                Minimum = 10,
                Parent = panel,
                TextAlign = HorizontalAlignment.Right,
                Width = 100
            };
            lbl = new Label
            {
                AutoSize = true,
                Location = new Point(lbl.Left, lbl.Bottom + ControlPadding),
                Parent = panel,
                Text = "乐高列数："
            };
            _numColCount = new NumericUpDown
            {
                Location = new Point(lbl.Right, lbl.Top - 3),
                Maximum = 10000,
                Minimum = 10,
                Parent = panel,
                TextAlign = HorizontalAlignment.Right,
                Width = 100
            };

            var btnApply = new Button
            {
                AutoSize = true,
                Location = new Point(_numColCount.Left, _numColCount.Bottom + ControlPadding),
                Parent = panel,
                Text = "确认调整"
            };
            btnApply.Click += BtnApply_Click;

            var btnOpen = new Button
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                AutoSize = true,
                Parent = panel,
                Text = "打开图片"
            };
            btnOpen.Location = new Point(ControlMargin, panel.ClientSize.Height - ControlMargin - btnOpen.Height);
            btnOpen.Click += BtnOpen_Click;
        }

        private void InitUi4ColorPalette(Panel panel)
        {
            var count = _availableColors.Count;
            for (var i = 0; i < count; i++)
            {
                var item = _availableColors[i];
                var lbl = new Label
                {
                    AutoSize = true,
                    Location = new Point(ControlMargin, ControlMargin + i * (25 + ControlPadding)),
                    Parent = panel,
                    Text = item.Name
                };
                var pic = new PictureBox
                {
                    BackColor = item.Color,
                    Location = new Point(lbl.Right + ControlPadding, lbl.Top + (lbl.Height - 25) / 2),
                    Parent = panel,
                    Size = new Size(50, 25)
                };
                pic.Click += PicColor_Click;
            }
        }

        private void InitUi4Canvas(Panel panel)
        {
            var panelTool = new Panel
            {
                Dock = DockStyle.Top,
                Parent = panel
            };

            var btnClear = new Button
            {
                AutoSize = true,
                Location = new Point(ControlMargin, ControlMargin),
                Parent = panelTool,
                Text = "清除"
            };
            btnClear.Click += BtnClear_Click;

            _picCanvas = new PictureBox
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Parent = panel
            };
            _picCanvas.BringToFront();
            _picCanvas.Paint += PicCanvas_Paint;
            _picCanvas.MouseDown += PicCanvas_MouseDown;
            _picCanvas.MouseMove += PicCanvas_MouseMove;
            _picCanvas.MouseUp += PicCanvas_MouseUp;
        }
        #endregion
    }
}
