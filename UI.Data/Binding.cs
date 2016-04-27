﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace UI.Data
{
    public class Binding
    {
        private object mSourceObject;
        private object mTargetObject;
        private string mSourcePath;
        private string mTargetPath;

        private INotifyPropertyChanged mNotificationObject;
        private PropertyChangedEventHandler mNotificationDelegate;


        public BindingType Type { get; set; }


        public Binding(object sourceObject, string sourcePath, object targetObject, string targetPath, bool delaySetup = false)
        {
            Type = BindingType.TwoWays;
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
            case BindingType.SourceToTarget:
                Setup(mSourceObject, mSourcePath, mTargetObject, mTargetPath);
                break;
            case BindingType.TargetToSource:
                Setup(mTargetObject, mTargetPath, mSourceObject, mSourcePath);
                break;
            case BindingType.TwoWays:
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
            // an indexed prop like Names[2], the content will be an object, so there should
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
            var oldVal = BindingPathParser.GetTargetValue(dest, destPath);

            if (oldVal == newVal) return;

            BindingPathParser.SetTargetValue(dest, destPath, newVal);
        }
   }



    public enum BindingType
    { 
        SourceToTarget,
        TargetToSource,  // Check if it implements propertyChanged/collectionChanged
        TwoWays
    }
}

