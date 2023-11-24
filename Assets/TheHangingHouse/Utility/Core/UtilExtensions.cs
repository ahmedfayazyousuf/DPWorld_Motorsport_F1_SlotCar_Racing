using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEditor;

namespace TheHangingHouse.Utility.Extensions
{
    public static class UtilExtensions
    {
        /// <summary>
        /// Convert given group of elements into string and make sure to include (inbetween variable) between elements.
        /// inbetween variable might be any character. for example if you want each element to be in separate lines, put inbetween variable as \n.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="inbetween"></param>
        /// <returns></returns>
        public static string Read<T>(this IEnumerable<T> ienumerable, string inbetween = ", ")
        {
            var list = new List<T>(ienumerable);
            var result = string.Empty;
            for (int i = 0; i < list.Count; i++)
                result += $"{list[i]}" +
                    $"{(i < list.Count - 1 ? inbetween : string.Empty)}";
            return result;
        }

        /// <summary>
        /// Convert given grid of elements into string and make sure to include (inbetweenColumns variable) between columns and (inbetweenRows variable) between rows.
        /// inbetween variable might be any character. for example if you want each element to be in separate lines, put inbetween variable as \n.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="inbetweenRows"></param>
        /// <returns></returns>
        public static string Read<T>(this T[,] grid, string inbetweenRows = "\n", string inbetweenColumns = ", ")
        {
            var result = "";
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                    result += $"{grid[i, j]}" +
                        $"{(j < grid.GetLength(1) - 1 ? inbetweenColumns : "")}";
                result += inbetweenRows;
            }
            return result;
        }

        /// <summary>
        /// Apply operator between each item in group and return the result as T type.
        /// The operation is based to (elements[i - 1], i - 1, elements[i], i).  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Reduce<T>(this IEnumerable<T> ienumerable, System.Func<T, int, T, int, T> func)
        {
            T result = default(T);
            var index = 0;
            foreach (var item in ienumerable)
            {
                if (index == 0)
                {
                    result = item;
                    index++;
                    continue;
                }

                result = func(result, index - 1, item, index);
                index++;
            }
            return result;
        }

        /// <summary>
        /// Apply operator between each item in group and return the result as T type.
        /// The operation is based to (elements[i - 1], elements[i]). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Reduce<T>(this IEnumerable<T> ienumerable, System.Func<T, T, T> func) =>
            Reduce(ienumerable, (x1, i1, x2, i2) => func(x1, x2));

        /// <summary>
        /// Resize an array. Incase of newLength is less than array.CurrentLength, 
        /// (outItems) will be the removed items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="newLength"></param>
        /// <param name="outItems"></param>
        /// <returns></returns>
        public static T[] Resize<T>(this T[] arr, int newLength, out T[] outItems)
        {
            outItems = new T[0];

            if (newLength == arr.Length)
            {
                outItems = new T[0];
                return arr;
            }

            if (newLength < arr.Length)
            {
                outItems = new T[arr.Length - newLength];
                for (int i = 0; i < outItems.Length; i++)
                    outItems[i] = arr[i + newLength];
            }

            var result = new T[newLength];
            for (int i = 0; i < result.Length; i++)
                if (i < arr.Length)
                    result[i] = arr[i];

            return result;
        }

        /// <summary>
        /// Check if one of element satisfies given condition.
        /// bool func(T element, int i) will be applied to all elements, The function will be true if atleast one of the elements satisfies func, 
        /// otherwise the result will be false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool Check<T>(this IEnumerable<T> ienumerable, System.Func<T, int, bool> func)
        {
            var index = 0;
            foreach (var item in ienumerable)
            {
                if (func(item, index))
                    return true;
                index++;
            }
            return false;
        }

        /// <summary>
        /// Check if one of element satisfies given condition.
        /// bool func(T element) will be applied to all elements, The function will be true if atleast one of the elements satisfies func, 
        /// otherwise the result will be false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool Check<T>(this IEnumerable<T> ienumerable, System.Func<T, bool> func) =>
            Check(ienumerable, (x, i) => func(x));

        /// <summary>
        /// (Map) function will go through all elements in (arr) and apply TOutput func(T element, int i) on them, 
        /// after that put the result into a new array and return it.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TOutput[] Map<TInput, TOutput>(this TInput[] arr, System.Func<TInput, int, TOutput> func)
        {
            var result = new TOutput[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                result[i] = func(arr[i], i);
            return result;
        }

        /// <summary>
        /// (Map) function will go through all elements in (arr) and apply TOutput func(T element) on them, 
        /// after that put the result into new array and return it.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TOutput[] Map<TInput, TOutput>(this TInput[] arr, System.Func<TInput, TOutput> func) =>
            Map(arr, (x, i) => func(x));

        /// <summary>
        /// (Map) function will go through all elements in (list) and apply TOutput func(T element, int i) on them, 
        /// after that put the result into new list and return it.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<TOutput> Map<TInput, TOutput>(this List<TInput> list, System.Func<TInput, int, TOutput> func)
        {
            var result = new TOutput[list.Count];
            for (int i = 0; i < list.Count; i++)
                result[i] = func(list[i], i);
            return new List<TOutput>(result);
        }

        /// <summary>
        /// (Map) function will go through all elements in (grid) and apply TOutput func(T element, int i, int j) on them, 
        /// after that put the result into new 2D array of type T and return it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[,] Map<T>(this T[,] grid, System.Func<T, int, int, T> func)
        {
            var result = new T[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    result[i, j] = func(grid[i, j], i, j);
            return result;
        }

        /// <summary>
        /// (Map) function will go through all elements in (grid) and apply TOutput func(T element) on them, 
        /// after that put the result into new 2D array of type T and return it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[,] Map<T>(this T[,] grid, System.Func<T, T> func) =>
            Map(grid, (x, i, j) => func(x));

        /// <summary>
        /// (Map) function will go through all elements in (grid) and apply TOutput func(int i, int j) on them, 
        /// after that put the result into new 2D array of type T and return it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[,] Map<T>(this T[,] grid, System.Func<int, int, T> func) =>
            Map(grid, (x, i, j) => func(i, j));

        /// <summary>
        /// (Map) function will go through all elements in (list) and apply TOutput func(T element) on them, 
        /// after that put the result into new list and return it.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<TOutput> Map<TInput, TOutput>(this List<TInput> list, System.Func<TInput, TOutput> func) =>
            Map(list, (x, i) => func(x));

        /// <summary>
        /// (Apply) function will go through all elements in (arr) and apply void action(T element, int i) on them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="action"></param>
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in collection)
                action?.Invoke(item, i++);
        }

        /// <summary>
        /// (Apply) function will go through all elements in (arr) and apply void action(T element) on them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="action"></param>
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action) =>
            Foreach(collection, (x, i) => action?.Invoke(x));

        /// <summary>
        /// (Apply) function will go through all childrens of (element) and apply void action(Transform element, int i) on them. 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="action"></param>
        public static void Apply(this Transform element, System.Action<Transform, int> action)
        {
            for (int i = 0; i < element.childCount; i++)
                action(element.GetChild(i), i);
        }

        /// <summary>
        /// (Apply) function will go through all childrens of (element) and apply void action(Transform element) on them. 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="action"></param>
        public static void Apply(this Transform element, System.Action<Transform> action) =>
            Apply(element, (x, i) => action(x));

        /// <summary>
        /// (Apply) function will go through all elements in (grid) and apply void action(T element, int i, int j) on them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="action"></param>
        public static void Foreach<T>(this T[,] grid, System.Action<T, int, int> action)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                    action?.Invoke(grid[i, j], i, j);
        }

        /// <summary>
        /// (Apply) function will go through all elements in (grid) and apply void action(int i, int j) on them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="action"></param>
        public static void Apply<T>(this T[,] grid, System.Action<int, int> action) =>
            Foreach(grid, (x, i, j) => action?.Invoke(i, j));

        /// <summary>
        /// The result will be elements that make bool func(T element, int i) return true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] Filter<T>(this T[] arr, System.Func<T, int, bool> func)
        {
            var result = new List<T>();
            for (int i = 0; i < arr.Length; i++)
                if (func(arr[i], i))
                    result.Add(arr[i]);
            return result.ToArray();
        }

        /// <summary>
        /// The result will be elements that make bool func(T element) return true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] Filter<T>(this T[] arr, System.Func<T, bool> func) =>
            Filter(arr, (x, b) => func(x));

        /// <summary>
        /// The result will be elements that make bool func(T element, int i) return true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> Filter<T>(this List<T> list, System.Func<T, int, bool> func)
        {
            var result = new List<T>();
            for (int i = 0; i < list.Count; i++)
                if (func(list[i], i))
                    result.Add(list[i]);
            return result;
        }

        /// <summary>
        /// The result will be elements that make bool func(T element) return true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> Filter<T>(this List<T> list, System.Func<T, bool> func) =>
            Filter(list, (x, b) => func(x));

        /// <summary>
        /// The result will be characters that make bool func(string txt, int i) return true.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string Filter(this string str, System.Func<char, int, bool> func)
        {
            var strResult = string.Empty;
            for (int i = 0; i < str.Length; i++)
                if (func(str[i], i))
                    strResult += str[i];
            return strResult;
        }

        /// <summary>
        /// The result will be characters that make bool func(string txt) return true.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string Filter(this string str, System.Func<char, bool> func) =>
            Filter(str, (ch, i) => func(ch));

        /// <summary>
        /// (AddSpaces) function will add space before any big case character and return the result.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddSpaces(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        /// <summary>
        /// (Search) function will apply func(T element, int i) to (arr) elements,
        /// The result will be the first element that satisfies bool func(T element, int i).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Search<T>(this T[] arr, System.Func<T, int, bool> func)
        {
            for (int i = 0; i < arr.Length; i++)
                if (func(arr[i], i))
                    return arr[i];
            return default;
        }

        /// <summary>
        /// (Search) function will apply func(T element) to (arr) elements,
        /// The result will be the first element that satisfies bool func(T element). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Search<T>(this T[] arr, System.Func<T, bool> func) =>
            Search(arr, (x, i) => func(x));

        /// <summary>
        /// (Search) function will apply func(T element, int i) to (list) elements,
        /// The result will be the first element that satisfies bool func(T element, int i). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Search<T>(this List<T> list, System.Func<T, int, bool> func)
        {
            for (int i = 0; i < list.Count; i++)
                if (func(list[i], i))
                    return list[i];
            return default;
        }

        /// <summary>
        /// (Search) function will apply func(T element) to (list) elements,
        /// The result will be the first element that satisfies bool func(T element).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Search<T>(this List<T> list, System.Func<T, bool> func) =>
            Search(list, (x, i) => func(x));

        /// <summary>
        /// (Find) function will go through all elements in (arr).
        /// The result will be all elements that satisfies bool func(T element, int i).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] Find<T>(this T[] arr, System.Func<T, int, bool> func)
        {
            var result = new List<T>();
            for (int i = 0; i < arr.Length; i++)
                if (func(arr[i], i))
                    result.Add(arr[i]);
            return result.ToArray();
        }

        /// <summary>
        /// (Find) function will go through all elements in (arr).
        /// The result will be all elements that satisfies bool func(T element). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] Find<T>(this T[] arr, System.Func<T, bool> func) =>
           Find(arr, (x, i) => func(x));

        /// <summary>
        /// (Find) function will go through all elements in (list).
        /// The result will be all elements that satisfies bool func(T element, int i).  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> Find<T>(this List<T> list, System.Func<T, int, bool> func)
        {
            var result = new List<T>();
            for (int i = 0; i < list.Count; i++)
                if (func(list[i], i))
                    result.Add(list[i]);
            return result;
        }

        /// <summary>
        /// (Find) function will go through all elements in (list).
        /// The result will be all elements that satisfies bool func(T element). 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> Find<T>(this List<T> list, System.Func<T, bool> func) =>
            Find(list, (x, i) => func(x));

        /// <summary>
        /// Last element in (arr).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T Last<T>(this T[] arr)
        {
            return arr[arr.Length - 1];
        }

        /// <summary>
        /// Last element in (list).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Last<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static Transform[] Childs(this Transform transform)
        {
            var childs = new Transform[transform.childCount];
            for (int i = 0; i < childs.Length; i++)
                childs[i] = transform.GetChild(i);
            return childs;
        }

        /// <summary>
        /// Encodes all the characters in the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="stringToEncode"></param>
        /// <returns></returns>
        public static byte[] Encode(this string stringToEncode)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] bytename = new byte[1024];
            bytename = utf8.GetBytes(stringToEncode);
            return bytename;
        }

        /// <summary>
        /// Check if (type) inherit from (interfaceType).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool ImplmentsInterface(this System.Type type, System.Type interfaceType)
        {
            if (type == interfaceType) return true;
            return new List<System.Type>(type.GetInterfaces()).Contains(interfaceType);
        }

        /// <summary>
        /// Coppy (text) to the clipboard.
        /// </summary>
        /// <param name="text"></param>
        public static void CopyToClipboard(this string text)
        {
            var textEditor = new TextEditor();
            textEditor.text = text;
            textEditor.SelectAll();
            textEditor.Copy();
            Debug.Log($"Copy To Clipboard.\n{text}");
        }

        /// <summary>
        /// Call action after (seconds), Note that delay is done by using coroutines, so if you call StopAllCoroutines() the action you passed will not work.
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="action"></param>
        /// <param name="seconds"></param>
        public static void Delay(this MonoBehaviour monoBehaviour, System.Action action, float seconds = 1f)
        {
            monoBehaviour.StartCoroutine(Wait());
            IEnumerator Wait()
            {
                yield return new WaitForSeconds(seconds);
                action?.Invoke();
            }
        }

        /// <summary>
        /// Call action after (frames).Note that delay is done by using coroutines, so if you call StopAllCoroutines() the action you passed will not work.
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="action"></param>
        /// <param name="frames"></param>
        public static void DelayFrames(this MonoBehaviour monoBehaviour, System.Action action, int frames = 1)
        {
            monoBehaviour.StartCoroutine(Wait());
            IEnumerator Wait()
            {
                while (frames-- > 0)
                    yield return new WaitForEndOfFrame();
                action?.Invoke();
            }
        }

        /// <summary>
        /// Create a shuffled copy of input array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] arr)
        {
            var result = new T[arr.Length];
            Array.Copy(arr, result, arr.Length);
            for (int i = 0; i < result.Length; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, result.Length);
                var currentElement = result[i];
                result[i] = result[randomIndex];
                result[randomIndex] = currentElement;
            }
            return result;
        }

        /// <summary>
        /// Create a shuffled copy of input list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> list) =>
            new List<T>(list.ToArray().Shuffle());

        public static List<T> RemoveRange<T>(this List<T> list, IEnumerable<T> collection)
        {
            var result = new List<T>(list);
            foreach (var element in result)
                result.Remove(element);
            return result;
        }

        /// <summary>
        /// Search for item, and return his index. If item is not found, the function will return -1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static int SearchIndex<T>(this IEnumerable<T> collection, Func<T, int, bool> func)
        {
            var i = 0;
            foreach(var item in collection)
            {
                if (func(item, i))
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Search for item, and return his index. If item is not found, the function will return -1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static int SearchIndex<T>(this IEnumerable<T> collection, Func<T, bool> func) =>
            SearchIndex(collection, (x, i) => func(x));
    }
}
