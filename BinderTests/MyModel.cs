using System.ComponentModel;

namespace BinderTests
{
    class MyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double mValue;
        private int[] mData = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public int[] Data {
            get {
                return mData;
            }
        }

        public double Value {
            get { return mValue; }
            set {
                if (value == mValue) return;
                mValue = value;
            }
        }

        private void OnPropertyChanged(string propertyChanged)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}

