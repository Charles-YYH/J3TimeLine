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
        private CaptureManager(){}
        private Point position;
        private int avgRGB = -2;

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
                return String.Format("X:{0}, Y:{1} {2}", position.X, position.Y, avgRGB);
            }
        }

    }
}
