using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debugDisplay : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();
    public Text display;
    private string debugKey;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;

    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            string sortBy = "feta";

            if (logString.Contains(sortBy)) {

                string[] splitString = logString.Split(char.Parse(":"));
                debugKey = splitString[0];

                string debugValue = splitString.Length > 1 ? splitString[1] : "";

                if (debugLogs.ContainsKey(debugKey))
                    debugLogs[debugKey] = debugValue;
                else
                    debugLogs.Add(debugKey, debugValue);
            }

        }

        string displayText = "";
        foreach (KeyValuePair<string, string> log in debugLogs)
        {

            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += log.Key + ": " + log.Value + "\n";

        }
        display.text = displayText;

    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log("feta Time:" + Time.time);



    }
}
