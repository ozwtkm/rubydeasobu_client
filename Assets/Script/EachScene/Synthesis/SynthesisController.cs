using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesisController : SceneController
{
    public bool Executionflag = false;

    DisplayUtil displayutil;

    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(WALLET);
        displayutil.WrappedGetAndRenderInfo(USERNAME);
        displayutil.WrappedGetAndRenderInfo(RECIPE);

    }

    // Update is called once per frame
    void Update()
    {
        if(Executionflag){
            displayutil.WrappedGetAndRenderInfo(WALLET);
            Executionflag = false;
        }
    }
}
