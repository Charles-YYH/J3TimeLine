using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Tesseract;

namespace J3TimeLine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, TimeManagerDelegate
    {
        private TimeManager t;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private System.Windows.Threading.DispatcherTimer captureTimer;
        private CaptureManager c = CaptureManager.shared;

        public MainWindow()
        {
            InitializeComponent();

            this.Topmost = true;

            t = new TimeManager();
            Binding binding = new Binding();
            binding.Source = t;
            binding.Path = new PropertyPath("TimeFormatted");
            BindingOperations.SetBinding(this.timeLabel, Label.ContentProperty, binding);

            binding = new Binding();
            binding.Source = c;
            binding.Path = new PropertyPath("PositionFormat");
            BindingOperations.SetBinding(this.positionLabel, Label.ContentProperty, binding);
        }

        private bool isStarted = false;
        private void startButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!isBattelStarted)
            {
            }
            else
            {
                isBattelStarted = false;
                this.startButton.Content = "请点击监控开始监控战斗开始";
                dispatcherTimer.Stop();
                dispatcherTimer = null;
            }
        }

        private void captureButtonClicked(object sender, RoutedEventArgs e)
        {
            didSetCursor = false;
            captureTimer = new System.Windows.Threading.DispatcherTimer();
            captureTimer.Tick += new EventHandler(captureTimer_Tick);
            captureTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            captureTimer.Start();
        }

        private void monitorButtonClicked(object sender, RoutedEventArgs e)
        {
            captureTimer.Start();
        }

        private bool isBattelStarted = false;
        private void startBattel()
        {
            if (isBattelStarted) return;
            System.Media.SystemSounds.Beep.Play();
            t.startTime = DateTime.Now;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Start();

            this.startButton.Content = "停止";
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime current = DateTime.Now;
            TimeSpan interval = current - t.startTime;
            t.SecondSinceStart = (int)Math.Truncate(interval.TotalSeconds);
        }

        private bool didSetCursor = false;
        private void captureTimer_Tick(object sender, EventArgs e)
        {
            var width = 1;
            var height = 1;
            var image = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                g.CopyFromScreen(c.Position.X, c.Position.Y, 0, 0, new System.Drawing.Size(width, height));
                this.captureImage.Source = BitmapToImageSource(image);
                int sum = 0;
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixelColor = image.GetPixel(x, y);
                        sum += pixelColor.R + pixelColor.G + pixelColor.B;
                        string pixelColorStringValue =
                        pixelColor.R.ToString("D3") + " " +
                        pixelColor.G.ToString("D3") + " " +
                        pixelColor.B.ToString("D3") + ", ";
                        if (sum > 540)
                            Console.WriteLine(pixelColorStringValue);
                    }
                }
                c.AvgRGB = sum / 3;
                if (!didSetCursor)
                    c.Position = System.Windows.Forms.Control.MousePosition;
                if (c.AvgRGB >= 220)
                {
                    if (!didSetCursor)
                    {
                        didSetCursor = true;
                        captureTimer.Stop();
                    }
                    else
                    {
                        startBattel();
                        isBattelStarted = true;
                    }
                }
            }
        }

        public void TimeDidChange(int secondSinceStart)
        {
            throw new NotImplementedException();
        }

        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
