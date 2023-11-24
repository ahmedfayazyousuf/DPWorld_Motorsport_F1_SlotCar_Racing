using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;
using TheHangingHouse.Utility.Extensions;

namespace TheHangingHouse.Utility
{
    public static class CSVFormatter
    {
        /// <summary>
        /// All public properties which has atleast one attribute exists in targetAttributes will be included in the csv file. 
        /// If the targetAttributes is null or empty, all public properties will be included.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="targetAttributes"></param>
        /// <returns></returns>
        public static string ToCSV(IEnumerable<object> ienumerable, System.Type type, System.Type[] targetAttributes = null)
        {
            var propsList = new List<object>(ienumerable);
            var properties = new List<PropertyInfo>(type.GetProperties());

            if (targetAttributes != null)
                foreach (var att in targetAttributes)
                    properties = properties.Filter(prop => prop.GetCustomAttribute(att) != null);

            var dataText = properties.Map(property => property.Name).Read(",") + "\n";

            if (propsList == null)
                return dataText;

            for (int i = 0; i < propsList.Count; i++)
            {
                var values = properties.Map(property => property.GetValue(propsList[i]));
                dataText += $"{values.Read(",")}{(i < propsList.Count - 1 ? "\n" : "")}";
            }

            return dataText;
        }

        /// <summary>
        /// All public properties which has atleast one attribute exists in targetAttributes will be included in the csv table.
        /// If the attributes is null or empty, all public properties will be included in the csv table.
        /// Properties that included in properties to ignore array will not be show in the csv data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="targetAttributesType"></param>
        /// <returns></returns>
        public static string ToCSV(IEnumerable<object> ienumerable, System.Type type, System.Type[] targetAttributesType, PropertyInfo[] propertiesToIgnore)
        {
            var list = new List<object>(ienumerable);
            var properties = new List<PropertyInfo>(type.GetProperties());
            foreach (var att in targetAttributesType)
                properties = properties.Filter(prop => prop.GetCustomAttribute(att) != null);
            propertiesToIgnore.Foreach((prop) => properties.Remove(prop));

            var dataText = properties.Map(property => property.Name).Read(",") + "\n";

            if (list == null)
                return dataText;

            for (int i = 0; i < list.Count; i++)
            {
                var values = properties.Map(property => property.GetValue(list[i]));
                dataText += $"{values.Read(",")}{(i < list.Count - 1 ? "\n" : "")}";
            }

            return dataText;
        }

        /// <summary>
        /// Create T instance from csv data, T data will filled from properties inside csv file, so the propert name and column title in csv file should be same.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csv_data"></param>
        /// <returns></returns>
        public static T[] FromCSV<T>(string csv_data)
            where T : new()
        {
            if (string.IsNullOrEmpty(csv_data) ||
                string.IsNullOrWhiteSpace(csv_data))
                return new T[0];

            var lines = Regex.Split(csv_data, "\n").Filter(line =>
            !string.IsNullOrEmpty(line) && 
            !string.IsNullOrWhiteSpace(line));

            var result = new T[lines.Length - 1];
            var headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                var element = new T();
                var splites = lines[i].Split(',');

                for (int j = 0; j < splites.Length; j++)
                {
                    var property = typeof(T).GetProperty(headers[j].Trim());
                    if (property == null) continue;
                    var value = Util.Parse(splites[j], property.PropertyType);
                    property.SetValue(element, value);
                }

                result[i - 1] = element;
            }

            return result;
        }

        /// <summary>
        /// Create object instance from csv data, instance type will be (type).
        /// Data will filled from properties inside csv file, so the propert name and column title in csv file should be same.
        /// </summary>
        /// <param name="csv_data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object[] FromCSV(string csv_data, System.Type type)
        {
            if (string.IsNullOrEmpty(csv_data) ||
                string.IsNullOrWhiteSpace(csv_data))
                return null;

            var lines = Regex.Split(csv_data, "\n");
            var result = new object[lines.Length - 1];
            var headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                var element = System.Activator.CreateInstance(type);
                var splites = lines[i].Split(',');

                for (int j = 0; j < splites.Length; j++)
                {
                    var property = type.GetProperty(headers[j].Trim());
                    if (property == null) continue;
                    var value = Util.Parse(splites[j], property.PropertyType);
                    property.SetValue(element, value);
                }

                result[i - 1] = element;
            }

            return result;
        }

        public static Dictionary<string, string>[] FromCSV(string csv_data)
        {
            if (string.IsNullOrEmpty(csv_data) ||
                string.IsNullOrWhiteSpace(csv_data))
                return null;

            var lines = Regex.Split(csv_data, "\n");
            var result = new Dictionary<string, string>[lines.Length - 1];
            var headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                var element = new Dictionary<string, string>();
                var splites = lines[i].Split(',');

                for (int j = 0; j < splites.Length; j++)
                    element.Add(headers[j], splites[j]);

                result[i - 1] = element;
            }

            return result;
        }
    }
}
