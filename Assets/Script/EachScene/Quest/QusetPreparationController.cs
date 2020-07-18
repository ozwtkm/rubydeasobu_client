using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QusetPreparationController : SceneController
{
    private DisplayUtil displayutil;
    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

       // displayutil.WrappedGetAndRenderInfo(QUEST);
        displayutil.WrappedGetAndRenderInfo(PARTY);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
