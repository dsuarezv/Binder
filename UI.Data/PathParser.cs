using System;
using System.Reflection;

namespace UI.Data
{
    public class PathParser
    {
        public static object GetTargetValue(object target, string path)
        {
            // Split path in .
            var components = path.Split('.');
            if (components.Length == 0) return null;

            object current = target;

            for (int i = 0; i < components.Length; ++i) 
            {
                current = ParseProperty(components[i], target);
            }

            return current;
        }

        private static object ParseProperty(string name, object target)
        {
            var indexer = name.IndexOf('[');

            if (indexer == -1)
            {
                // regular property, simply retrieve value
                var p = GetPropertyInfo(target, name);
                if (p == null) throw new ArgumentException("Property not found in object: " + name);

                return p.GetValue(target);
            }
            else
            {
                // indexed property, parse index and cast target to collection
                var indexerEnd = name.IndexOf(']');
                if (indexerEnd == -1) throw new ArgumentException("Indexed binding syntax error: " + name);

                var propName = name.Substring(0, indexer);
                var indexString = name.Substring(indexer + 1, indexerEnd - indexer - 1);

                int index;
                if (!Int32.TryParse(indexString, out index)) throw new ArgumentException("Invalid indexer: " + name);

                var p = GetPropertyInfo(target, propName);
                if (p == null) throw new ArgumentException("Property not found in object: " + propName);

                if (p.GetIndexParameters().Length > 0)
                {
                    // Index parameter
                }
                else
                {
                    // Array
                    var array = (Array)p.GetValue(target);
                    if (array == null) throw new ArgumentException("Indexed array is null.");

                    return array.GetValue(index);
                }

                try
                {
                    return p.GetValue(target, new object[] { index });
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private static PropertyInfo GetPropertyInfo(object target, string propName)
        {
            return target.GetType().GetProperty(propName );
        }
    }
}

