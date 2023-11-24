using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;
using System.Linq;
using TheHangingHouse.Utility;
using TheHangingHouse.Utility.Extensions;

namespace TheHangingHouse.JsonSerializer
{
    [DefaultExecutionOrder(-1)]
    public class BehaviourJsonSerializer : MonoBehaviour
    {
        public static string ProjectPath() => Directory.GetParent(Regex.Replace(Application.dataPath, "/", @"\")).FullName;
        public static string DataPath(string dataName = null) => @$"{ProjectPath()}\{(dataName != null ? dataName : DEFAULT_DATA_NAME)}.json";

        private static Type[] TargetTypes;

        public const string DEFAULT_DATA_NAME = "Application Data";

        private bool m_loaded;

        static BehaviourJsonSerializer()
        {
            TargetTypes = Util.DerivedFrom(typeof(MonoBehaviourID));
        }

        private void Awake()
        { 
            if (m_loaded) return;

            LoadAll();

            var jsonData = File.ReadAllText(DataPath());
            var objects = ExtractObjects(jsonData);
            var values = objects.Map(obj => GetStringValue(obj, nameof(Field.valueType)));
            values.Read("\n").CopyToClipboard();
            
        }

        public void LoadAll()
        {
            if (!File.Exists(DataPath(DEFAULT_DATA_NAME)))
                File.WriteAllText(DataPath(DEFAULT_DATA_NAME), "[]");

            var data = Load();
            Apply(data);
            m_loaded = true;
        }

        public void Apply(string jsonData)
        {
            Apply(FromJson(jsonData));
            Debug.Log($"Apply Json Data:\n\n {jsonData}");
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //static void OnBeforeSceneLoadRuntimeMethod()
        //{
        //    if (!File.Exists(DataPath(DEFAULT_DATA_NAME)))
        //        File.WriteAllText(DataPath(DEFAULT_DATA_NAME), JsonConvert.SerializeObject(new Field[0]));

        //    var jsonSerializers = Resources.FindObjectsOfTypeAll<BehaviourJsonSerializer>();
        //    if (jsonSerializers.Length == 0) return;
        //    var jsonSerializer = jsonSerializers[0];
        //    var data = jsonSerializer.Load();
        //    jsonSerializer.Apply(data);
        //}

        public void Save(string dataCategory = null)
        {
            var totalFields = GetFields();
            var packedFields = PackFields(totalFields);
            var dataSpecified = !string.IsNullOrEmpty(dataCategory) &&
                    !string.IsNullOrWhiteSpace(dataCategory);

            foreach(var pair in packedFields)
            {
                var dataName = pair.Key;
                if (dataSpecified && dataCategory != dataName) continue;
                var fields = pair.Value;
                var jsonData = ToJson(fields);
                var path = DataPath(dataName);
                File.WriteAllText(path, jsonData);
            }
        }

        public Dictionary<string, List<Field>> Load()
        {
            var resultDict = new Dictionary<string, List<Field>>();
            var dataNames = GetDataNames();

            foreach(var dataName in dataNames)
            {
                var dataPath = DataPath(dataName);
                var jsonData = File.Exists(dataPath) ? File.ReadAllText(dataPath) : string.Empty;
                var fields = FromJson(jsonData);
                resultDict.Add(dataName, new List<Field>(fields));
            }

            return resultDict;
        }

        public void Apply(IEnumerable<Field> fields)
        {
            var gos = Resources.FindObjectsOfTypeAll<MonoBehaviourID>();

            foreach(var field in fields)
            {
                var go = gos.Search(g => g.id.Equals(field.objectID)); // Game Object
                if (go == null) { Debug.LogWarning($"Unable To Find Object With ID {field.objectID}"); continue; }
                var fieldInfo = go.GetType().GetField(field.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo == null) { Debug.LogWarning($"Unable To Find FieldInfo With Name ({field.name}) in Type ({typeof(MonoBehaviourID).FullName})"); continue; }
                fieldInfo.SetValue(go, field.GetValue());
            }
        }

        public void Apply(Dictionary<string, List<Field>> data)
        {
            foreach (var fields in data.Values)
                Apply(fields);
            Debug.Log("Data Loaded Successfully!");
        }

        /// <summary>
        /// Get All Different Data Names(Only Work In Current Scene).
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDataNames()
        {
            var fields = PackFields(GetFields()).Keys.ToList();
            if (!fields.Contains(DEFAULT_DATA_NAME))
                fields.Add(DEFAULT_DATA_NAME);
            return fields;
        }

        public static List<(Field field, JsonSerializeField att)> GetFields()
        {
            var fields = new List<(Field field, JsonSerializeField att)>();

            foreach (var go in Resources.FindObjectsOfTypeAll<MonoBehaviourID>())
            {
                //Debug.Log($"{go.name}: {go.gameObject.scene.name}");

                if (go.gameObject.scene.name == null)
                    continue;

                foreach (var fieldInfo in go.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    var jsonSerializeFieldAtt = fieldInfo.GetCustomAttribute<JsonSerializeField>();
                    if (jsonSerializeFieldAtt == null) continue;
                    var field = Field.Create(
                        fieldInfo.Name,
                        go.id,
                        go.gameObject.name,
                        fieldInfo.GetValue(go),
                        fieldInfo.FieldType);
                    fields.Add((field, jsonSerializeFieldAtt));
                }
            }
            return fields;
        }

        public static Dictionary<string, List<Field>> PackFields(IEnumerable<(Field field, JsonSerializeField att)> fields)
        {
            var resultDict = new Dictionary<string, List<Field>>();

            foreach(var fieldPair in fields)
            {
                if (!resultDict.ContainsKey(fieldPair.att.DataName))
                    resultDict.Add(fieldPair.att.DataName, new List<Field>());
                resultDict[fieldPair.att.DataName].Add(fieldPair.field);
            }

            return resultDict;
        }

        /// <summary>
        /// Serialize collection of field to json.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string ToJson(IEnumerable<Field> fields)
        {
            var json = "[\n";
            foreach(var field in fields)
                json += $"{JsonUtility.ToJson(field, true)},\n";
            json = json.Remove(json.Length - 2, 2);
            json = json.Split("\n").Map((line, i) => $"{(i > 0 ? "\t" : "")}{line}").Read("\n");
            json += "\n]";
            return json;
        }

        /// <summary>
        /// Load jsonData as field array.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static Field[] FromJson(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogError("Empty Json Data");
                return null;
            }

            var objectsList = ExtractObjects(jsonData);
            var fields = new Field[objectsList.Count];
            var index = 0;
            foreach(var jsonObject in objectsList)
            {
                var valueTypeName = GetStringValue(jsonObject, nameof(Field.valueType));
                var valueType = Util.ByName(valueTypeName);
                if (valueType == null)
                {
                    Debug.LogWarning($"{valueTypeName} Type is exists in the project !");
                    continue;
                }
                var fieldType = typeof(Field<>).MakeGenericType(valueType);
                var field = (Field)JsonUtility.FromJson(jsonObject.ToString(), fieldType);
                fields[index++] = field;
            }
            fields = fields.Filter(f => f != null);
            return fields;
        }

        public static List<string> ExtractObjects(string jsonArrayData)
        {
            var jsonObjectsData = new List<string>();
            var implement = default(bool);
            var openedScopes = 0;

            for (int i = 0; i < jsonArrayData.Length; i++)
            {
                if (jsonArrayData[i] == '{')
                {
                    openedScopes++;
                    if (openedScopes == 1)
                    {
                        implement = true;
                        jsonObjectsData.Add("");
                    }
                }

                if (implement)
                    jsonObjectsData[jsonObjectsData.Count - 1] += jsonArrayData[i];

                if (jsonArrayData[i] == '}')
                {
                    openedScopes--;
                    if (openedScopes == 0)
                        implement = false;
                }
            }
            return jsonObjectsData;
        }

        public static string GetStringValue(string jsonObjectData, string labelName)
        {
            var valuePart = jsonObjectData.Split($"\"{labelName}\"")[1];
            var implement = false;
            var result = string.Empty;

            if (valuePart[0] == '\"')
                implement = true;

            for (int i = 1; i < valuePart.Length; i++)
            {
                if (valuePart[i] == '\"')
                {
                    if (valuePart[i - 1] != '\\')
                    {
                        implement = !implement;
                        if (implement) continue;
                        if (!implement) break;
                    }    
                }

                if (implement)
                    result += valuePart[i];
            }

            return result;
        }
    }

    [Serializable]
    public abstract class Field
    {
        public string name;
        public string objectID;
        public string gameObjectName;
        public string valueType;

        public static Field Create(string variableName, string objectID, string gameObjectName, object value, Type valueType)
        { 
            var fieldType = typeof(Field<>).MakeGenericType(new Type[] { valueType });
            var field = (Field)Activator.CreateInstance(fieldType);
            field.name = variableName;
            field.objectID = objectID;
            field.gameObjectName = gameObjectName;
            field.valueType = valueType.FullName;
            field.SetValue(value);
            return field;
        }

        public abstract void SetValue(object value);
        public abstract object GetValue();
    }

    [Serializable]
    public class Field<T> : Field
    {
        public T value;

        public override void SetValue(object value) => this.value = (T)value;
        public override object GetValue() => value;
    }
}