using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using TheHangingHouse.JsonSerializer;
using TheHangingHouse.Utility.Extensions;
using UnityEditor.Callbacks;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using TheHangingHouse.Utility;
using System.Reflection;

namespace TheHangingHouse.JsonSerializerEditor
{
    [CustomEditor(typeof(BehaviourJsonSerializer))]
    public class BehaviourJsonSerializerEditor : Editor
    {
        private BehaviourJsonSerializer m_jsonSerializer;
        private int m_selectedDataIndex;

        private string[] m_dataNames;

        private void OnEnable()
        {
            m_jsonSerializer = (BehaviourJsonSerializer)target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                m_jsonSerializer.Save(m_dataNames[m_selectedDataIndex]);
                Debug.Log($"JSON Data Saved ! ({m_dataNames[m_selectedDataIndex]})");
            }

            if (GUILayout.Button("Save All"))
            {
                m_jsonSerializer.Save();
                Debug.Log($"JSON Data Saved !");
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Load"))
            {
                var objs = Resources.FindObjectsOfTypeAll<MonoBehaviourID>();
                Undo.RecordObjects(objs, "Load Application Data");
                var data = m_jsonSerializer.Load();
                m_jsonSerializer.Apply(data);
            }

            EditorGUILayout.BeginHorizontal();
            {
                m_dataNames = BehaviourJsonSerializer.GetDataNames().ToArray();
                m_selectedDataIndex = EditorGUILayout.Popup("Data", m_selectedDataIndex, m_dataNames);
                BehaviourJsonSerializer.GetFields();
                if (GUILayout.Button("Open"))
                {
                    var path = BehaviourJsonSerializer.DataPath(m_dataNames[m_selectedDataIndex]);
                    System.Diagnostics.Process.Start(path);
                    Debug.Log($"Open: {path}");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var buildDirectory = Directory.GetParent(pathToBuiltProject).FullName;

            foreach (var dataName in GetAllDataNames())
            {
                if (!File.Exists(BehaviourJsonSerializer.DataPath(dataName)))
                {
                    Debug.LogWarning($"({dataName}) Not Included In The Build.\n" +
                                    $"Unable to find {dataName} make sure to save your data before build.");
                    continue;
                }
                var filePath = @$"{buildDirectory}\{Path.GetFileName(BehaviourJsonSerializer.DataPath(dataName))}";
                File.Copy(BehaviourJsonSerializer.DataPath(dataName), filePath, true);
            }
        }

        public static List<string> GetAllDataNames()
        {
            var names = new List<string>();
            var types = Util.DerivedFrom(typeof(MonoBehaviourID));
            foreach (var type in types)
            {
                foreach (var field in type.GetFields(BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    var jsonSeriaizeField = field.GetCustomAttribute<JsonSerializeField>();
                    if (jsonSeriaizeField == null) continue;
                    if (!names.Contains(jsonSeriaizeField.DataName))
                        names.Add(jsonSeriaizeField.DataName);
                }
            }
            if (names.Count == 0)
                names.Add(BehaviourJsonSerializer.DEFAULT_DATA_NAME);
            return names;
        }
    }
}