using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tesseract;

namespace J3TimeLine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimeManager t;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public MainWindow()
        {
            InitializeComponent();

            t = new TimeManager();
            Binding binding = new Binding();
            binding.Source = t;
            binding.Path = new PropertyPath("TimeFormatted");
            BindingOperations.SetBinding(this.timeLabel, Label.ContentProperty, binding);

            string message = "00:19 | 火圈";
            lstMessage.Items.Add(message);
            lstMessage.Items.Add(message);
            lstMessage.Items.Add(message);
            lstMessage.Items.Add(message);
        }

        private bool isStarted = false;
        private void startButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!isStarted)
            {
                t.startTime = DateTime.Now;
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
                dispatcherTimer.Start();

                isStarted = true;
                this.startButton.Content = "stop";
            }
            else
            {
                isStarted = false;
                this.startButton.Content = "start";
                dispatcherTimer.Stop();
                dispatcherTimer = null;
            }
        }

        private void captureButtonClicked(object sender, RoutedEventArgs e)
        {
            var testImagePath = "C:\\Users\\qyxx0\\Desktop\\sample.PNG";
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Console.WriteLine(text);
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2.ToString());
            }

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime current = DateTime.Now;
            TimeSpan interval = current - t.startTime;
            t.SecondSinceStart = (int) Math.Truncate(interval.TotalSeconds);
        }
    }
}
