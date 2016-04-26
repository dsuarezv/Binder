using System.ComponentModel;

namespace BinderTests
{
    class MyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int mItemValue = 3;
        private double mValue;
        private int[] mData = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private MyModel mChild;

        public string Name { get; set; }

        public MyModel Child {
            get { return mChild; }
            set { mChild = value; }
        }

        public int this[int index] { 
            get { return mItemValue; }
            set { mItemValue = value; }
        }

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

