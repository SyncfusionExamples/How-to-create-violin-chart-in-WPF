using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViolinChartSample
{

    public static class ReflectionExt
    {
        public static object GetInternalProperty(string property, object src)
        {
            return src.GetType().GetProperty(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).
                GetValue(src);
        }

        public static void SetInternalProperty(string property, object src, object destination)
        {
            var value = src.GetType().GetProperty(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).
                GetValue(src);

            destination.GetType().GetProperty(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).
                SetValue(destination, value);
        }

        public static object GetInternalField(string property, object src)
        {
            return src.GetType().GetField(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).
                GetValue(src);
        }
        public static object GetInternalField(string property, object src, Type type)
        {
            return type.GetField(property, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).
                GetValue(src);
        }
    }
}
