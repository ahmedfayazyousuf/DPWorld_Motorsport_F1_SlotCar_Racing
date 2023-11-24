using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TheHangingHouse.Utility;
using TheHangingHouse.Utility.Extensions;
using TheHangingHouse.UI.TableInternal;

namespace TheHangingHouse.UI.TableEditor
{
    [CustomEditor(typeof(Table))]
    public class TableEditor : Editor
    {
        public Table table;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Generate From CSV"))
                {
                    Undo.RecordObject(table, "Table");

                    var path = EditorUtility.OpenFilePanel("CSV File", Application.dataPath, "csv");
                    var csvData = File.ReadAllText(path);
                    var csvDict = CSVFormatter.FromCSV(csvData);
                    Debug.Log(csvDict.Map(dict => dict.Read()).Read("\n"));
                    table.Clear();
                    table.Generate(csvDict);

                    
                    foreach (var t in table.generatorParameters.rowsContainer.Childs())
                        Undo.RegisterCreatedObjectUndo(t.gameObject, "Clear");
                    if (table.generatorParameters.titleRow != null)
                        foreach (var t in table.generatorParameters.titleRow.GetChild(0).Childs())
                            Undo.RegisterCreatedObjectUndo(t.gameObject, "Clear");
                }

                if (GUILayout.Button("Clear"))
                {
                    Undo.RecordObject(table, "Clear");

                    var childCount = table.generatorParameters.rowsContainer.childCount;
                    foreach (var child in table.generatorParameters.rowsContainer.Childs())
                        Undo.DestroyObjectImmediate(child.gameObject);
                    if (table.generatorParameters.titleRow != null)
                    {
                        var cells = table.generatorParameters.titleRow.GetComponentsInChildren<Cell>();
                        foreach (var cell in cells)
                            Undo.DestroyObjectImmediate(cell.gameObject);
                    }
                }
            } 
        }

        private void OnEnable()
        {
            table = (Table)target;
        }

        [MenuItem("THH/UI/Table")]
        public static void CreateTable()
        {
            var path = $"Assets/TheHangingHouse/UI/Table/Prefabs/Table.prefab";
            var prefabTable = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = CreateCanvas();
                var eventSystem = CreateEventSystem();

                Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create Canvas");
                Undo.RegisterCreatedObjectUndo(eventSystem.gameObject, "Create Event System");
            }
            var go = Instantiate(prefabTable, canvas.transform, false);
            go.name = "Table";

            Selection.activeObject = go;

            Undo.RegisterCreatedObjectUndo(go, "Create Table");
        }

        private static Canvas CreateCanvas()
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
            var graphicsRaycaster = canvasGO.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            return canvas;
        }

        private static EventSystem CreateEventSystem()
        {
            var eventSystemGO = new GameObject("Event System");
            var eventSystem = eventSystemGO.AddComponent<EventSystem>();
            var standardInputModule = eventSystemGO.AddComponent<StandaloneInputModule>();
            return eventSystem;
        }
    }
}
