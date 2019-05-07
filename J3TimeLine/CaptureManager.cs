using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using System.ComponentModel;

namespace J3TimeLine
{
    class CaptureManager : INotifyPropertyChanged
    {
        public static CaptureManager shared = new CaptureManager();
        public event PropertyChangedEventHandler PropertyChanged;
        public CaptureManagerDelegate dl;

        private CaptureManager(){}
        private Point position;
        private int avgRGB = -2;
        private bool didSetCursor = false;
        public bool isBattelStarted = false;

        private System.Windows.Threading.DispatcherTimer captureTimer;

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if(PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Position"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PositionFormat"));
                }
            }
        }

        public int AvgRGB
        {
            get
            {
                return avgRGB;
            }

            set
            {
                avgRGB = value;
                if(PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PositionFormat"));
                }
            }
        }

        public String PositionFormat
        {
            get
            {
                return String.Format("调试信息：X:{0}, Y:{1}  RGB平均值：{2}", position.X, position.Y, avgRGB);
            }
        }

        private void initTimer()
        {
            captureTimer = new System.Windows.Threading.DispatcherTimer();
            captureTimer.Tick += new EventHandler(captureTimer_Tick);
            captureTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
        }

        public void startCapture()
        {
            didSetCursor = false;
            if (captureTimer == null) initTimer();
            captureTimer.Start();
        }

        public void startMonitor()
        {
            didSetCursor = true;
            if (isBattelStarted) return;
            if (captureTimer == null) initTimer();
            captureTimer.Start();
        }

        public void stop()
        {
            if (captureTimer != null) captureTimer.Stop();
            captureTimer = null;
            isBattelStarted = false;
            dl.BattelDidStop();
        }

        private void captureTimer_Tick(object sender, EventArgs e)
        {
            var width = 1;
            var height = 1;
            var image = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                g.CopyFromScreen(position.X, position.Y, 0, 0, new System.Drawing.Size(width, height));
                int sum = 0;
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixelColor = image.GetPixel(x, y);
                        sum += pixelColor.R + pixelColor.G + pixelColor.B;
                    }
                }
                avgRGB = sum / 3;
                if (!didSetCursor)
                    Position = System.Windows.Forms.Control.MousePosition;
                Console.WriteLine(avgRGB);
                if (avgRGB >= 210)
                {
                    if (!didSetCursor)
                    {
                        didSetCursor = true;
                        SettingDAO.updateValueByKey("DEFAULTX", Position.X.ToString());
                        SettingDAO.updateValueByKey("DEFAULTY", Position.Y.ToString());
                        captureTimer.Stop();
                        System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("进战图标位置重设完成，若是不小心移动到其他白色区域而触发此对话框，可以再" +
                            "点击刚才的按钮继续重新设定，请确保最终位置是在进入战斗图标（两柄交叉小剑）的交叉处或者刀刃处（最白的地方）",
                                          "重设确认",
                                          System.Windows.MessageBoxButton.OK);
                    }
                    else
                    {
                        Console.WriteLine("Battel Start");
                        dl.BattelDidStart();
                        isBattelStarted = true;
                        captureTimer.Stop();
                    }
                }
            }
        }
    }
    interface CaptureManagerDelegate
    {
        void BattelDidStart();
        void BattelDidStop();
    }
}
