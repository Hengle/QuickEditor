namespace QuickEditor.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    public static class QEditorExtensions
    {

        public static IList ForEach(this IList source, Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in source)
                action();

            return source;
        }

        #region String Extensions

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                int length = value.Length;
                for (int i = 0; i < length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string value, params object[] args)
        {
            return args.Length > 0 ? string.Format(value, args) : value;
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public static string Format(this string value, object arg0)
        {
            return arg0 != null ? string.Format(value, arg0) : value;
        }

        public static int ToInt(this string value)
        {
            return Int32.Parse(value);
        }

        public static int TryParseInt32(this string str, int defaultValue)
        {
            int result = defaultValue;
            return int.TryParse(str, out result) ? result : defaultValue;
        }

        public static bool IsURL(this string source)
        {
            if (source.IsNullOrEmpty()) { return false; }// TODO: raise exception or log error
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsMatch(this string input, string pattern)
        {
            if (input.IsNullOrEmpty()) return false;
            else return Regex.IsMatch(input, pattern);
        }

        public static string Match(this string input, string pattern)
        {
            if (input.IsNullOrEmpty()) return string.Empty;
            return Regex.Match(input, pattern).Value;
        }

        #endregion String Extensions
    }

    public static class QEditorWindowExtensions
    {
        public static EditorWindow minSize(this EditorWindow window, Vector2 minSize)
        {
            window.minSize = minSize;
            return window;
        }

        public static EditorWindow maxSize(this EditorWindow window, Vector2 maxSize)
        {
            window.maxSize = maxSize;
            return window;
        }
    }

    public static class QEditorRectExtensions
    {
        public static Rect WindowRect(this Rect rect)
        {
            return new Rect(rect)
            {
                x = 0,
                y = 0
            };
        }

        /// <summary>
        /// Contracts or Expands the selected rect by <c>padding</c> amount, leaving the center point unmoved.
        /// </summary>
        public static Rect WithPadding(this Rect rect, float padding)
        {
            rect.x += padding;
            rect.xMax -= padding * 2;
            rect.y += padding;
            rect.yMax -= padding * 2;

            return rect;
        }

        /// <summary>
        /// Returns this rect with the specified X
        /// </summary>
        public static Rect WithX(this Rect rect, float newX)
        {
            rect.x = newX;
            return rect;
        }

        /// <summary>
        /// Returns this rect with the specified Y
        /// </summary>
        public static Rect WithY(this Rect rect, float newY)
        {
            rect.y = newY;
            return rect;
        }

        /// <summary>
        /// Returns this rect with the specified xMax
        /// </summary>
        public static Rect WithXMax(this Rect rect, float newXMax)
        {
            rect.xMax = newXMax;
            return rect;
        }

        /// <summary>
        /// Returns this rect with the specified yMax
        /// </summary>
        public static Rect WithYMax(this Rect rect, float newYMax)
        {
            rect.yMax = newYMax;
            return rect;
        }

        /// <summary>
        /// Returns this rect with the specified width
        /// </summary>
        public static Rect WithW(this Rect rect, float newW)
        {
            rect.width = newW;
            return rect;
        }

        /// <summary>
        /// Returns this rect with the specified height
        /// </summary>
        public static Rect WithH(this Rect rect, float newH)
        {
            rect.height = newH;
            return rect;
        }
    }

    public static class QEditorAccessExtensions
    {
        public static T InvokeConstructor<T>(this Type type, Type[] paramTypes = null, object[] paramValues = null)
        {
            return (T)type.InvokeConstructor(paramTypes, paramValues);
        }

        public static object InvokeConstructor(this Type type, Type[] paramTypes = null, object[] paramValues = null)
        {
            if (paramTypes == null || paramValues == null)
            {
                paramTypes = new Type[] { };
                paramValues = new object[] { };
            }

            var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);

            return constructor.Invoke(paramValues);
        }

        public static T Invoke<T>(this object o, string methodName, params object[] args)
        {
            var value = o.Invoke(methodName, args);
            if (value != null)
            {
                return (T)value;
            }

            return default(T);
        }

        public static T Invoke<T>(this object o, string methodName, Type[] types, params object[] args)
        {
            var value = o.Invoke(methodName, types, args);
            if (value != null)
            {
                return (T)value;
            }

            return default(T);
        }

        public static object Invoke(this object o, string methodName, params object[] args)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
                types[i] = args[i] == null ? null : args[i].GetType();

            return o.Invoke(methodName, types, args);
        }

        public static object Invoke(this object o, string methodName, Type[] types, params object[] args)
        {
            var invaild = Array.FindIndex(types, item => item == null) != -1;
            var type = o is Type ? (Type)o : o.GetType();

            var method = invaild ? type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, types, null);
            if (method != null)
                return method.Invoke(o, args);

            return null;
        }

        public static T GetFieldValue<T>(this object o, string name)
        {
            var value = o.GetFieldValue(name);
            if (value != null)
            {
                return (T)value;
            }

            return default(T);
        }

        public static object GetFieldValue(this object o, string name)
        {
            var field = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                return field.GetValue(o);
            }

            return null;
        }

        public static void SetFieldValue(this object o, string name, object value)
        {
            var field = o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(o, value);
            }
        }

        public static T GetPropertyValue<T>(this object o, string name)
        {
            var value = o.GetPropertyValue(name);
            if (value != null)
            {
                return (T)value;
            }

            return default(T);
        }

        public static object GetPropertyValue(this object o, string name)
        {
            var property = o.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                return property.GetValue(o, null);
            }

            return null;
        }

        public static void SetPropertyValue(this object o, string name, object value)
        {
            var property = o.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                property.SetValue(o, value, null);
            }
        }
    }
}
