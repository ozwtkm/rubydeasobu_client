using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesisController : SceneController
{

    DisplayUtil displayutil;

    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(WALLET);
        displayutil.WrappedGetAndRenderInfo(USERNAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
