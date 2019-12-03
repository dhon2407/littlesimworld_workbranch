using UnityEngine;
using System.Collections;

 public class DevLogger : MonoBehaviour
{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    string myLog;
    Queue myLogQueue = new Queue();

    private void Start()
    {
        Debug.LogError("Starting log console.");
    }

    //void OnEnable()
    //{
    //    Application.logMessageReceived += HandleLog;
    //}

    //void OnDisable()
    //{
    //    Application.logMessageReceived -= HandleLog;
    //}

    //void HandleLog(string logString, string stackTrace, LogType type)
    //{
    //    //myLog = logString;
    //    //string newString = "\n [" + type + "] : " + myLog;
    //    //myLogQueue.Enqueue(newString);
    //    //if (type == LogType.Exception)
    //    //{
    //    //    newString = "\n" + stackTrace;
    //    //    myLogQueue.Enqueue(newString);
    //    //}
    //    //myLog = string.Empty;
    //    //foreach (string mylog in myLogQueue)
    //    //{
    //    //    myLog += mylog;
    //    //}
    //}

    //void OnGUI()
    //{
    //    //GUILayout.Label(myLog);
    //}
#endif
}