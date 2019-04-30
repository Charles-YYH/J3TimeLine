using System;
using System.ComponentModel;

namespace J3TimeLine
{
    class TimeManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int secondSinceStart = 0;
        public DateTime startTime;
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


    }
}
