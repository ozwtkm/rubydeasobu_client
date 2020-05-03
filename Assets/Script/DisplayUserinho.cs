using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUserinho : MonoBehaviour
{
    private GUIStyle style;

    // Start is called before the first frame update
    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI () {
        Rect rect = new Rect(10, 10, 100, 100);

        string username = PlayerPrefs.GetString("username");
        string message = username + "でログイン中";

        GUI.Label(rect, message, style);
    }
}
