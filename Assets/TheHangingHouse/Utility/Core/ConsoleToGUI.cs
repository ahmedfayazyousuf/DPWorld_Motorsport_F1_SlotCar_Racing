using UnityEngine;

namespace TheHangingHouse.Utility
{
    public class ConsoleToGUI : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        public bool show;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    show = !show;
                    Debug.Log("LeftControl+Q Pressed");
                }
            }
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        public void SetConsoleActive(bool consoleActive)
        {
            show = consoleActive;
        }

        public void NegateActive()
        {
            show = !show;
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            if (show)
            {
                myLog = GUI.TextArea(new Rect(10, 10, Screen.width * 0.5f, Screen.height * 0.5f), myLog);
            }
        }
        //#endif
    }
}