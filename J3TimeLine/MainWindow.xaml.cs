using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Configuration;
using System.IO;
using System.Data.SQLite;

namespace J3TimeLine
{
    public partial class MainWindow : Window
    {
        private TimeManager t;
        private CaptureManager c = CaptureManager.shared;

        public MainWindow()
        {
            InitializeComponent();

            this.Topmost = true;

            t = new TimeManager();
            c.dl = t;
            Binding binding = new Binding();
            binding.Source = t;
            binding.Path = new PropertyPath("TimeFormatted");
            BindingOperations.SetBinding(this.timeLabel, Label.ContentProperty, binding);

            binding = new Binding();
            binding.Source = c;
            binding.Path = new PropertyPath("PositionFormat");
            BindingOperations.SetBinding(this.positionLabel, Label.ContentProperty, binding);

            var defaultX = SettingDAO.getValueByKey("DEFAULTX");
            var defaultY = SettingDAO.getValueByKey("DEFAULTY");
            if(defaultX == "-1" || defaultY == "-1")
            {
                defaultX = SettingDAO.getValueByKey("19201080X");
                defaultY = SettingDAO.getValueByKey("19201080Y");
            }
            c.Position = new System.Drawing.Point(Convert.ToInt32(defaultX), Convert.ToInt32(defaultY));

            lstMessage.Items.Add("00:13 huoquan");
            lstMessage.Items.Add("00:16 huoquan");

            t.timelines.Add("00:13 huoquan");
            t.timelines.Add("00:16 huoquan");
        }

        private void captureButtonClicked(object sender, RoutedEventArgs e)
        {
            c.startCapture();
        }

        private bool didClickMonitor = false;
        private void monitorButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!didClickMonitor)
            {
                c.startMonitor();
                this.monitorButton.Content = "停止";
                didClickMonitor = !didClickMonitor;
            }
            else
            {
                c.stop();
                this.monitorButton.Content = "开始监控";
                didClickMonitor = !didClickMonitor;
            }
        }

        
    }
}
