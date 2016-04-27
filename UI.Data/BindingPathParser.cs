using System;
using System.Reflection;

namespace UI.Data
{

    public delegate object DirectPropertyDelegate(object target, PropertyInfo p);
    public delegate object IndexedPropertyDelegate(object target, PropertyInfo p, int index);
    public delegate object ArrayPropertyDelegate(Array target, PropertyInfo p, int index);


    public class BindingPathParser
    {
        public static object GetTargetValue(object target, string path)
        {
            return TraversePath(target, path);
        }

        public static void SetTargetValue(object target, string path, object newValue)
        {
            TraversePath(target, path, 
                 (t, p) => { p.SetValue(t, newValue); return null; },
                 (t, p, i) => { p.SetValue(t, newValue, new object[] { i }); return null; },
                 (t, p, i) => { t.SetValue(newValue, new int[] { i }); return null; }
            );
        }

        public static object TraversePath(object target, string path,
                                           DirectPropertyDelegate directCallback = null, 
                                           IndexedPropertyDelegate listCallback = null, 
                                           ArrayPropertyDelegate arrayCallback = null)
        {
            // Split path in .
            var components = path.Split('.');
            if (components.Length == 0) return null;

            object current = target;

            for (int i = 0; i < components.Length; ++i) 
            {
                var last = (i == components.Length - 1);
                
                current = ParseProperty(components[i], current, last, 
                                        directCallback, listCallback, arrayCallback);
            }

            return current;
        }

        private static object ParseProperty(string name, object target, bool isLast,
                                            DirectPropertyDelegate directCallback,
                                            IndexedPropertyDelegate listCallback,
                                            ArrayPropertyDelegate arrayCallback)
        {
            if (target == null) throw new NullReferenceException(name);

            var indexer = name.IndexOf('[');

            if (indexer == -1)
            {
                return GetDirectPropertyValue(name, target, isLast, directCallback);
            }
            else
            {
                    return GetIndexedPropertyValue(name, target, indexer, isLast, listCallback, arrayCallback);
            }
        }

        static object GetDirectPropertyValue(string name, object target, bool isLast, DirectPropertyDelegate directCallback)
        {
            // regular property, simply retrieve value
            var p = GetPropertyInfo(target, name);
            if (p == null) throw new ArgumentException("Property not found in object: " + name);

            if (isLast && directCallback != null)
            {
                return directCallback(target, p);
            }
            else
            {
                return p.GetValue(target);
            }
        }

        static object GetIndexedPropertyValue(string name, object target, int indexer, bool isLast, 
                                              IndexedPropertyDelegate listCallback, 
                                              ArrayPropertyDelegate arrayCallback)
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
                if (isLast && listCallback != null)
                {
                    return listCallback(target, p, index);
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

                if (isLast && arrayCallback != null)
                {
                    return arrayCallback(array, p, index);
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

