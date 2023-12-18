using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageToPixel
{
    public partial class MainForm : Form
    {
        #region constructor
        public MainForm()
        {
            InitializeComponent();

            InitUi();
        }
        #endregion

        #region property
        private const int ControlMargin = 20;
        private const int ControlPadding = 12;
        private PictureBox _picInput;
        private PictureBox _picOutput;
        private NumericUpDown _numPixelSpacing;
        private TextBox _txtAvailableHtmlColors;
        private Bitmap _imgInput;
        private Bitmap _imgOutput;

        private static readonly List<string> AvailableHtmlColors = new List<string>
        {
            //"#FFFFFF",
            //"#000000"
            ColorTranslator.ToHtml(Color.FromArgb(255, 0, 0)),//红色：255, 0, 0
            ColorTranslator.ToHtml(Color.FromArgb(0, 0, 255)),//蓝色：0, 0, 255
            ColorTranslator.ToHtml(Color.FromArgb(0, 255, 0)),//绿色：0, 255, 0
            ColorTranslator.ToHtml(Color.FromArgb(255, 255, 0)),//黄色：255, 255, 0
            ColorTranslator.ToHtml(Color.FromArgb(255, 255, 255)),//白色：255, 255, 255
            ColorTranslator.ToHtml(Color.FromArgb(0, 0, 0)),//黑色：0, 0, 0
            ColorTranslator.ToHtml(Color.FromArgb(165, 42, 42)),//棕色：165, 42, 42
        };
        #endregion

        #region method
        
        #endregion

        #region event handler
        private void BtnOpen_Click(object? sender, EventArgs e)
        {
            var openDlg = new OpenFileDialog
            {
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "打开图片",
            };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            _imgInput = new Bitmap(openDlg.FileName);
            _imgInput = ImageHelper.ResizeImage(_imgInput, _imgInput.Width * 4, _imgInput.Height * 4);
            _picInput.Image = _imgInput;
            _picOutput.Left = _picInput.Right + ControlPadding;
        }

        private void BtnConvert_Click(object? sender, EventArgs e)
        {
            var availableColors = AvailableHtmlColors.Select(a => ColorTranslator.FromHtml(a)).ToList();
            var pixelSpacing = (int)_numPixelSpacing.Value;
            _imgOutput = new Bitmap(_imgInput.Width, _imgInput.Height);
            using (var graphics = Graphics.FromImage(_imgOutput))
            {
                for (var offsetX = 0; offsetX < _imgInput.Width; offsetX += pixelSpacing)
                {
                    for (var offsetY = 0; offsetY < _imgInput.Height; offsetY += pixelSpacing)
                    {
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
                        var mergedColor = ColorHelper.MergeColor(colorBlock);
                        var closestColor = ColorHelper.FindClosestColor(availableColors, mergedColor);
                        graphics.FillRectangle(new SolidBrush(closestColor), offsetX, offsetY, pixelSpacing, pixelSpacing);
                    }
                }
            }
            _picOutput.Image = _imgOutput;
        }

        private void BtnSampleColor_Click(object? sender, EventArgs e)
        {
            if (_imgInput == null || _imgInput.Width <= 1) return;
            AvailableHtmlColors.Clear();
            AvailableHtmlColors.Add("#FFFFFF");
            AvailableHtmlColors.Add("#000000");
            var r = new Random(DateTime.Now.Millisecond);
            var step = _imgInput.Width / 16;
            for (var i = 0; i < 16; i++)
            {
                var x = r.Next(i * step, (i + 1) * step);
                var y = r.Next(0, _imgInput.Height);
                var color = _imgInput.GetPixel(x, y);
                AvailableHtmlColors.Add(ColorTranslator.ToHtml(color));
            }
            _txtAvailableHtmlColors.Text = string.Join("\r\n", AvailableHtmlColors);
        }
        #endregion

        #region ui
        private void InitUi()
        {
            ClientSize = new Size(1600, 1020);
            StartPosition = FormStartPosition.CenterScreen;
            Text = $"图片转像素 {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

            var btnOpen = new Button
            {
                AutoSize = true,
                Location = new Point(ControlMargin, ControlMargin),
                Parent = this,
                Text = "打开图片",
            };
            btnOpen.Click += BtnOpen_Click;

            var lbl = new Label
            {
                AutoSize = true,
                Location = new Point(btnOpen.Right + ControlPadding, btnOpen.Top + 3),
                Parent = this,
                Text = "像素间距："
            };
            _numPixelSpacing = new NumericUpDown
            {
                Location = new Point(lbl.Right, btnOpen.Top + 1),
                Minimum = 1,
                Parent = this,
                Value = 4
            };

            var btnConvert = new Button
            {
                AutoSize = true,
                Location = new Point(_numPixelSpacing.Right + ControlPadding, btnOpen.Top),
                Parent = this,
                Text = "转换",
            };
            btnConvert.Click += BtnConvert_Click;

            var btnSampleColor = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true,
                Parent = this,
                Text = "颜色取样"
            };
            btnSampleColor.Location = new Point(ClientSize.Width - ControlMargin - btnSampleColor.Width, ControlMargin);
            btnSampleColor.Click += BtnSampleColor_Click;
            _txtAvailableHtmlColors = new TextBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                Location = new Point(ClientSize.Width - 200 - ControlMargin, btnSampleColor.Bottom + ControlPadding),
                Multiline = true,
                Parent = this,
                Size = new Size(200, ClientSize.Height - ControlMargin - btnSampleColor.Bottom - ControlPadding),
                Text = string.Join("\r\n", AvailableHtmlColors)
            };

            _picInput = new PictureBox
            {
                BackColor = Color.White,
                Location = new Point(ControlMargin, btnOpen.Bottom + ControlPadding),
                Parent = this,
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            _picOutput = new PictureBox
            {
                BackColor = Color.White,
                Location = new Point(_picInput.Right + ControlPadding, _picInput.Top),
                Parent = this,
                SizeMode = PictureBoxSizeMode.AutoSize
            };
        }
        #endregion
    }
}