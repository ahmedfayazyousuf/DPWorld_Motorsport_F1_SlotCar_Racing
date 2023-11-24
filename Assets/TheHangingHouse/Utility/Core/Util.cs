using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using TheHangingHouse.Utility.Extensions;
using System;
using System.Reflection;
using System.IO;
using UnityEngine.UI;

namespace TheHangingHouse.Utility
{
    public static class Util
    {
        public static object Parse(string txt, System.Type type)
        {
            if (type.Equals(typeof(string))) return txt;

            var expectedParamters = new System.Type[] { typeof(string) };
            var methode = type.GetMethod("Parse", expectedParamters);

            if (methode == null)
                return System.Activator.CreateInstance(type);

            try
            {
                return methode.Invoke(null, new object[] { txt });
            }
            catch
            {
                return System.Activator.CreateInstance(type);
            }
        }

        public static T Parse<T>(string txt)
        {
            if (typeof(T).Equals(typeof(string))) return (T)(object)txt;
            var expectedParamters = new Type[] { typeof(string) };
            var methode = typeof(T).GetMethod("Parse", expectedParamters);

            if (methode == null)
                return (T)Activator.CreateInstance(typeof(T));

            return (T)methode.Invoke(null, new object[] { txt });
        }

        public static bool TryParse(string txt, Type type, out object obj)
        {
            obj = null;
            var parseable = true;

            try { obj = Parse(txt, type); }
            catch { parseable = false; }

            return parseable;
        }

        public static bool TryParse<T>(string txt, out object obj)
        {
            obj = null;
            var parsable = true;

            try { obj = Parse<T>(txt); }
            catch { parsable = false; }

            return parsable;
        }

        /// <summary>
        /// Search In All Assemblies In The Project (Very Expensive!)
        /// </summary>
        /// <param name="mainClass"></param>
        /// <returns></returns>
        public static Type[] DerivedFrom(Type mainClass)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(mainClass)).ToArray();
        }

        public static Type ByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }
            return null;
        }

        public static Texture2D LoadTexture(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            return texture;
        }

        public static void Copy(RectTransform sourceRT, RectTransform desticationRT)
        {
            desticationRT.pivot = sourceRT.pivot;
            desticationRT.sizeDelta = sourceRT.sizeDelta;
            desticationRT.anchorMin = sourceRT.anchorMin;
            desticationRT.anchorMax = sourceRT.anchorMax;
            desticationRT.offsetMin = sourceRT.offsetMin;
            desticationRT.offsetMax = sourceRT.offsetMax;
            desticationRT.anchoredPosition = sourceRT.anchoredPosition;
        }

        public static string GenerateDummyNumber(int digitCount)
        {
            var result = string.Empty;
            for (int i = 0; i < digitCount; i++)
                result += UnityEngine.Random.Range(i == 0 ? 1 : 0, 10);
            return result;
        }
    }
}
