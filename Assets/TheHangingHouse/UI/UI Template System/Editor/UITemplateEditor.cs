using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TheHangingHouse.Utility.Extensions;
using System.Text.RegularExpressions;
using TheHangingHouse.Utility;
using System;
using TheHangingHouse.JsonSerializer;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;
using UnityEngine.EventSystems;

namespace TheHangingHouse.UI.UITemplate.Editor
{
    public static class TemplateData
    {
        public static string ScriptNameTag = "__ScriptName";

        public static ScriptTemplateSlot[] TemplatePaths =
        {
            new ScriptTemplateSlot
            {
                templatePath = $"{Application.dataPath}/TheHangingHouse/UI/UI Template System/Resources/Page Template.txt",
                defaultScriptPath = "Scripts\\Pages",
                createFolderContainer = true
            },
            new ScriptTemplateSlot
            {
                templatePath = $"{Application.dataPath}/TheHangingHouse/UI/UI Template System/Resources/Animatable Element Template.txt",
                defaultScriptPath = "Scripts",
                createFolderContainer = false
            },
            new ScriptTemplateSlot
            {
                templatePath = $"{Application.dataPath}/TheHangingHouse/UI/UI Template System/Resources/Scale Animatable Element Template.txt",
                defaultScriptPath = "Scripts",
                createFolderContainer = false
            },
            new ScriptTemplateSlot
            {
                templatePath = $"{Application.dataPath}/TheHangingHouse/UI/UI Template System/Resources/Scale Animatable Element Template.txt",
                defaultScriptPath = "Scripts",
                createFolderContainer = false
            },
        };
    }

    public class UITemplateEditor : EditorWindow
    {
        public EditorGUIControle[] editors;

        [MenuItem("THH/Setup 3D Scene")]
        public static void Setup3DScene()
        {
            var camerasSectionDevider = new GameObject("----------------- CAMERA -----------------");
            camerasSectionDevider.transform.SetSiblingIndex(0);
            Undo.RegisterCreatedObjectUndo(camerasSectionDevider, "camerasSectionDevider");

            var lightSectionDevider = new GameObject("----------------- LIGHT -----------------");
            lightSectionDevider.transform.SetSiblingIndex(2);
            Undo.RegisterCreatedObjectUndo(lightSectionDevider, "lighSectionDevider");

            var userInterfaceSectionDevider = new GameObject("----------------- USER INTERFACE -----------------");
            userInterfaceSectionDevider.transform.SetSiblingIndex(4);
            Undo.RegisterCreatedObjectUndo(userInterfaceSectionDevider, "userInterfaceSectionDevider");

            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            Undo.RegisterCreatedObjectUndo(canvasGO, "canvas");

            var pagesGO = new GameObject("Pages");
            var pagesManager = pagesGO.AddComponent<PagesManager>();
            var rectTransform = pagesGO.AddComponent<RectTransform>();
            pagesGO.transform.SetParent(canvasGO.transform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            Undo.RegisterCreatedObjectUndo(pagesGO, "PagesManager");

            var eventSystemGO = new GameObject("Event System");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystemGO, "eventSystem");
        }

        [MenuItem("THH/Setup 2D Scene")]
        public static void Setup2DScene()
        {
            var camerasSectionDevider = new GameObject("----------------- CAMERA -----------------");
            camerasSectionDevider.transform.SetSiblingIndex(0);
            Undo.RegisterCreatedObjectUndo(camerasSectionDevider, "camerasSectionDevider");

            var userInterfaceSectionDevider = new GameObject("----------------- USER INTERFACE -----------------");
            userInterfaceSectionDevider.transform.SetSiblingIndex(2);
            Undo.RegisterCreatedObjectUndo(userInterfaceSectionDevider, "userInterfaceSectionDevider");

            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            Undo.RegisterCreatedObjectUndo(canvasGO, "canvas");

            var pagesGO = new GameObject("Pages");
            var pagesManager = pagesGO.AddComponent<PagesManager>();
            var rectTransform = pagesGO.AddComponent<RectTransform>();
            pagesGO.transform.SetParent(canvasGO.transform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            Undo.RegisterCreatedObjectUndo(pagesGO, "PagesManager");

            var eventSystemGO = new GameObject("Event System");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystemGO, "eventSystem");
        }

        [MenuItem("THH/Window/UI Template")]
        public static void ShowWindow()
        {
            var window = GetWindowWithRect<UITemplateEditor>(new Rect(200, 200, 600, 300), true, "THH UI Template");
        }

        private void OnEnable()
        {
            editors = new EditorGUIControle[]
            {
                new ScriptProducerEditor("NULL", TemplateData.TemplatePaths),
                new BehaviourPageGeneratorEditor(),
            };
        }

        private void OnGUI()
        {
            var containerRect = new Rect(0, 0, position.width, position.height);
            containerRect.x += 5;
            containerRect.y += 5;
            containerRect.width -= containerRect.x * 2;
            containerRect.height -= containerRect.y;

            GUILayout.BeginArea(containerRect);
            { 
                for (int i = 0; i < editors.Length; i++)
                {
                    var currentEditor = editors[i];
                    currentEditor.OnGUI();

                    EditorGUILayout.Space(10);
                }
            }
            GUILayout.EndArea();
        }
    }

    public struct ScriptTemplateSlot
    {
        public string templatePath;
        public string defaultScriptPath;
        public bool createFolderContainer;
    }

    public abstract class EditorGUIControle
    {
        public abstract void OnGUI();
    }

    public class ScriptProducerEditor : EditorGUIControle
    {
        public static int LastTemplateIndex
        {
            get => PlayerPrefs.GetInt("TemplateIndex");
            set => PlayerPrefs.SetInt("TemplateIndex", value);
        }

        public string pageName;

        public string ScriptPath
        {
            get => PlayerPrefs.GetString("ScriptPath");
            set => PlayerPrefs.SetString("ScriptPath", value);
        }

        public ScriptTemplateSlot[] templates;
        public bool createFolderContainer;

        public int templateIndex;

        public string[] TemplateNames => templates.Map(temp => Path.GetFileName(temp.templatePath).Split(".")[0]);

        private int m_canchedTemplateIndex = -1;

        public ScriptProducerEditor(string pageName, ScriptTemplateSlot[] templates)
        {
            templateIndex = LastTemplateIndex;
            m_canchedTemplateIndex = templateIndex;

            this.pageName = pageName;
            this.templates = templates;

            if (string.IsNullOrEmpty(ScriptPath) ||
                string.IsNullOrWhiteSpace(ScriptPath))
                ScriptPath = templates[templateIndex].defaultScriptPath;

            createFolderContainer = templates[templateIndex].createFolderContainer;
        }

        public override void OnGUI()
        {
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            GUILayout.Label(GetType().Name.AddSpaces(), style);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Template");
                templateIndex = EditorGUILayout.Popup(templateIndex, TemplateNames);
                LastTemplateIndex = templateIndex;

                if (m_canchedTemplateIndex != templateIndex)
                {
                    ScriptPath = templates[templateIndex].defaultScriptPath;
                    createFolderContainer = templates[templateIndex].createFolderContainer;
                }

                m_canchedTemplateIndex = templateIndex;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Create Folder Container");
                createFolderContainer = EditorGUILayout.Toggle(createFolderContainer);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Script Name");
                pageName = EditorGUILayout.TextField(pageName);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Script Path");
                ScriptPath = EditorGUILayout.TextField(ScriptPath);

                if (GUILayout.Button("Pick"))
                {
                    var path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "Scripts");
                    ScriptPath = path.Split("Assets")[1].Remove(0, 1);
                }

                if (GUILayout.Button("Create Script"))
                {
                    CreatePageScript(
                        GlobalizePath(ScriptPath), 
                        pageName,
                        templates[templateIndex].templatePath,
                        createFolderContainer
                        );
                    AssetDatabase.Refresh();
                }
            }
            GUILayout.EndHorizontal();
        }

        public void CreatePageScript(string directoryPath, string pageName, string templatePath, bool createFolderContainer)
        {
            directoryPath = Regex.Replace(directoryPath, "/", "\\");
            if (createFolderContainer)
                directoryPath = $"{directoryPath}\\{pageName.AddSpaces()}";
            Directory.CreateDirectory(directoryPath);
            var script = ProducePageScript(pageName, templatePath);
            File.WriteAllText($"{directoryPath}\\{pageName.Filter(ch => ch != ' ')}.cs", script);
        }

        public string ProducePageScript(string pageName, string templatePath)
        {
            pageName = pageName.Filter(ch => ch != ' ');
            var templateScript = File.ReadAllText(templatePath);
            templateScript = Regex.Replace(templateScript, TemplateData.ScriptNameTag, pageName);
            return templateScript;
        }

        public string GlobalizePath(string localPath)
        {
            localPath = Regex.Replace(localPath, "/", "\\");
            var dataPath = Regex.Replace(Application.dataPath, "/", "\\");
            return $"{dataPath}\\{localPath}";
        }
    }

    public class BehaviourPageGeneratorEditor : EditorGUIControle
    {
        public Transform container;
        public Type[] pagesTypes;
        public int selectedPageIndex;
        public Sprite background;
        public bool customizedName;
        public string gameObjectName;

        public BehaviourPageGeneratorEditor()
        {
            var pagesGO = GameObject.Find("Pages");
            if (pagesGO != null)
            {
                container = pagesGO.transform;
                pagesTypes = Util.DerivedFrom(typeof(Page));
                selectedPageIndex = pagesTypes.Length - 1;
            }
        }

        public override void OnGUI()
        {
            if (pagesTypes == null || pagesTypes.Length <= 0) return;

            var titleStyle = new GUIStyle();
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.white;

            GUILayout.Label(GetType().Name.AddSpaces(), titleStyle);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Script Name");
                var options = pagesTypes.Map(t => t.Name.AddSpaces());
                selectedPageIndex = EditorGUILayout.Popup(selectedPageIndex, options);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                var labelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label);
                labelStyle.normal.textColor = customizedName ? Color.white : Color.gray;

                var fieldStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).textField);
                fieldStyle.normal.textColor = customizedName ? Color.white : Color.gray;

                GUILayout.Label("Game Object Name", labelStyle);

                GUILayout.Space(110);
                var newGameObjectName = EditorGUILayout.TextField(gameObjectName, fieldStyle);
                customizedName = EditorGUILayout.Toggle(customizedName);

                if (customizedName)
                    gameObjectName = newGameObjectName;
                else
                {
                    gameObjectName = pagesTypes[selectedPageIndex].Name.AddSpaces();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Background");
                background = EditorGUILayout.ObjectField(background, typeof(Sprite), false) as Sprite;
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Create Behaviour Page"))
            {
                CreateBehaviourPage(container, pagesTypes[selectedPageIndex]);
            }
        }

        public void CreateBehaviourPage(Transform container, System.Type pageType)
        {
            var pagePrefab = Resources.Load<GameObject>("Page");
            var pageGO = UnityEngine.Object.Instantiate(pagePrefab);

            pageGO.name = gameObjectName;
            pageGO.transform.SetParent(container, false);
            (pageGO.AddComponent(pageType) as MonoBehaviourID).id = Guid.NewGuid().ToString();
            pageGO.GetComponent<Image>().sprite = background;
            UnityEngine.Object.FindObjectOfType<PagesManager>().pages.Add(pageGO.GetComponent(pageType) as Page);

            Undo.RegisterCreatedObjectUndo(pageGO, "Create New Behaviour " + pageType.Name);
        }
    }
}