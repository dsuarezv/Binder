using System;
using System.ComponentModel;

namespace BinderTests
{
    public class MyControl: INotifyPropertyChanged
    {
        private int mValue; 

        public int Value {
            get { return mValue; }
            set {
                mValue = value;
                OnPropertyChanged("Value");
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MyControl()
        {
            
        }

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}

