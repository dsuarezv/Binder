using System;
using System.ComponentModel;
using System.Reflection;

namespace UI.Data
{
    public class Binding: IDisposable
    {
        private object mSourceObject;
        private object mTargetObject;
        private string mSourcePath;
        private string mTargetPath;

        private INotifyPropertyChanged mNotificationObject;
        private PropertyChangedEventHandler mNotificationDelegate;


        public BindingMode Type { get; set; }


        public Binding(object sourceObject, string sourcePath, object targetObject, string targetPath, BindingMode type, bool delaySetup = false)
        {
            Type = type;
            mSourcePath = sourcePath;
            mSourceObject = sourceObject;
            mTargetPath = targetPath;
            mTargetObject = targetObject;

            if (!delaySetup) Setup();
        }

        public void Setup()
        {
            switch (Type)
            {
            case BindingMode.SourceToTarget:
                Setup(mSourceObject, mSourcePath, mTargetObject, mTargetPath);
                break;
            case BindingMode.TargetToSource:
                Setup(mTargetObject, mTargetPath, mSourceObject, mSourcePath);
                break;
            case BindingMode.TwoWay:
                Setup(mSourceObject, mSourcePath, mTargetObject, mTargetPath);
                Setup(mTargetObject, mTargetPath, mSourceObject, mSourcePath);
                break;
            }
        }

        public void Dispose()
        {
            if (mNotificationObject == null) return;

            mNotificationObject.PropertyChanged -= mNotificationDelegate;
        }

        private void Setup(object source, string sourcePath, object dest, string destPath)
        {
            if (source == null || dest == null || sourcePath == null || destPath == null) return;

            // Register PropertyChanged event. This can only be objects, so the
            // indexed cases are ruled out (if the last element of a binding is 
            // an indexed prop like Names[2], the content will be an object and there should
            // be a property name follwing. Otherwise, we can't register the notifychanged
            // event.

            BindingPathParser.TraversePath(source, sourcePath,
                (t, p) => {
                    var notif = t as INotifyPropertyChanged;
                    SetupNotification(notif, p, dest, destPath);
                    UpdateValue(notif, p, dest, destPath);
                    return null;
                });
        }


        private void SetupNotification(INotifyPropertyChanged notif, PropertyInfo p, object dest, string destPath)
        {
            if (notif == null) return;

            mNotificationObject = notif;
            mNotificationDelegate = (sender, e) => {
                if (e.PropertyName != p.Name) return;
                UpdateValue(notif, p, dest, destPath);
            };

            notif.PropertyChanged += mNotificationDelegate;


            // check also if it is an observablecollection.
        }

        static void UpdateValue(INotifyPropertyChanged notif, PropertyInfo p, object dest, string destPath)
        {
            if (notif == null) return;

            var newVal = p.GetValue(notif);

            // DAVE: a traversal is done here and another below. May be possible to make just 
            // one to get and set the value. Note that the 3 callbacks need to be implemented
            // in this case. 
            var oldVal = BindingPathParser.GetTargetValue(dest, destPath);
            if (oldVal == newVal) return;

            BindingPathParser.SetTargetValue(dest, destPath, newVal);
        }
    }



    public enum BindingMode
    { 
        SourceToTarget,
        TargetToSource,  // Check if it implements propertyChanged/collectionChanged
        TwoWay
    }
}

