using System;
using System.Collections;
using System.ComponentModel;

namespace J3TimeLine
{
    class TimeManager : INotifyPropertyChanged, CaptureManagerDelegate
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int secondSinceStart = 0;
        public DateTime startTime;
        private System.Windows.Threading.DispatcherTimer timer;
        public ArrayList timelines = new ArrayList();

        public int SecondSinceStart
        {
            get { return secondSinceStart; }
            set
            {
                if (value >= 5941) value = 0; // 99 分钟以上自动归零
                secondSinceStart = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SecondSinceStart"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TimeFormatted"));
                }
                TimeDidChanged();
            }
        }

        private void TimeDidChanged()
        {
            foreach(string line in timelines)
            {
                int min = Convert.ToInt32(line.Substring(0, 2));
                int second = Convert.ToInt32(line.Substring(3, 2));
                Console.WriteLine(min);
                Console.WriteLine(second);
            }
        }

        public string TimeFormatted
        {
            get
            {
                int s = 0;
                int m = 0;
                s = secondSinceStart % 60;
                m = secondSinceStart / 60;
                return String.Format("{0,2}:{1,2}", m, s);
            }
        }

        public void BattelDidStart()
        {
            System.Media.SystemSounds.Beep.Play();
            startTime = DateTime.Now;
            Console.WriteLine(startTime);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            timer.Start();
        }

        public void BattelDidStop()
        {
            if (timer != null) timer.Stop();
            timer = null;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime current = DateTime.Now;
            TimeSpan interval = current - startTime;
            SecondSinceStart = (int)Math.Truncate(interval.TotalSeconds);
        }
    }
}
