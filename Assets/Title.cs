using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
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

        // ラベルを表示する
        GUI.Label(rect, "Ruby Quest Monsters Ultra Light", style);
    }
}
