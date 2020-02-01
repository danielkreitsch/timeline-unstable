using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class visibleLog : MonoBehaviour
{

	string myLog;
	Queue myLogQueue = new Queue();
	private int lines = 0;

	// Start is called before the first frame update
	void OnEnable()
    {
	    //Application.logMessageReceived += HandleLog;
	    Application.logMessageReceivedThreaded += HandleLog;
		Debug.Log("Enablked Now");
    }

    void OnDisable()
    {
	    Debug.Log("Disabled");
	    //Application.logMessageReceived -= HandleLog;
	    Application.logMessageReceivedThreaded -= HandleLog;
	}

    void HandleLog(string logString, string stackTrace, LogType type)
    {
	    myLog = logString;
	    string newString = "\n [" + type + "] : " + myLog;
	    myLogQueue.Enqueue(newString);
	    if (type == LogType.Exception)
	    {
		    newString = "\n" + stackTrace;
		    myLogQueue.Enqueue(newString);
	    }
	    myLog = string.Empty;
	    foreach (string mylog in myLogQueue)
	    {
		    myLog += mylog;
	    }

		Run.OnMainThread(() => {
			var Text = GetComponent<Text>();
			Text.text = myLog;
		});
	    
    }
}
