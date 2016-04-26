using System;
using System.Reflection;

namespace UI.Data
{
    public class PathParser
    {
        public static object GetTargetValue(object target, string path)
        {
            return TraversePath(target, path);
        }

        public static void SetTargetValue(object target, string path, object newValue)
        {
            TraversePath(target, path, true, newValue);
        }

        private static object TraversePath(object target, string path, bool isSetting = false, object newValue = null)
        {
            // Split path in .
            var components = path.Split('.');
            if (components.Length == 0) return null;

            object current = target;

            for (int i = 0; i < components.Length; ++i) 
            {
                var setting = (i == components.Length - 1 && isSetting);
                
                current = ParseProperty(components[i], current, setting, newValue);
            }

            return current;
        }

        private static object ParseProperty(string name, object target, bool isSetting, object newValue)
        {
            var indexer = name.IndexOf('[');

            if (indexer == -1)
            {
                return GetDirectPropertyValue(name, target, isSetting, newValue);
            }
            else
            {
                return GetIndexedPropertyValue(name, target, indexer, isSetting, newValue);
            }
        }

        static object GetDirectPropertyValue(string name, object target, bool isSetting, object newValue)
        {
            // regular property, simply retrieve value
            var p = GetPropertyInfo(target, name);
            if (p == null) throw new ArgumentException("Property not found in object: " + name);

            if (isSetting)
            {
                p.SetValue(target, newValue);
                return null;
            }
            else
            {
                return p.GetValue(target);
            }
        }

        static object GetIndexedPropertyValue(string name, object target, int indexer, bool isSetting, object newValue)
        {
            // indexed property, parse index and cast target to collection
            // Only one index supported. 
            string propName;
            int index;
            GetIndexAndProperty(name, indexer, out propName, out index);

            var p = GetPropertyInfo(target, propName);
            if (p == null) throw new ArgumentException("Property not found in object: " + propName);

            if (p.GetIndexParameters().Length > 0)
            {
                // Index parameter.
                if (isSetting)
                {
                    p.SetValue(target, newValue, new object[] { index });
                    return null;
                }
                else
                {
                    return p.GetValue(target, new object[] { index });
                }
            }
            else
            {
                // Try as Array
                var array = p.GetValue(target) as Array;
                if (array == null) throw new ArgumentException("Indexed array is null.");

                if (isSetting)
                {
                    array.SetValue(newValue, index);
                    return null;
                }
                else
                {
                    return array.GetValue(index);
                }
            }
        }

        static void GetIndexAndProperty(string name, int indexer, out string propName, out int index)
        {
            var indexerEnd = name.IndexOf(']');
            if (indexerEnd == -1) throw new ArgumentException("Indexed binding syntax error: " + name);

            propName = name.Substring(0, indexer);
            var indexString = name.Substring(indexer + 1, indexerEnd - indexer - 1);
            if (!Int32.TryParse(indexString, out index)) throw new ArgumentException("Invalid indexer: " + name);
        }

        private static PropertyInfo GetPropertyInfo(object target, string propName)
        {
            return target.GetType().GetProperty(propName );
        }
    }
}

