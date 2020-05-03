using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUserInho : MonoBehaviour
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
        Rect rect = new Rect(10, 10, 400, 300);

        string username = PlayerPrefs.GetString("session_id");

        GUI.Label(rect, username, style);
    }
}
