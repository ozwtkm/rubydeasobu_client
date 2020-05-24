using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPageController : SceneController
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        DisplayUtil displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(WALLET);
        displayutil.WrappedGetAndRenderInfo(MONSTER);
        displayutil.WrappedGetAndRenderInfo(USERNAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
