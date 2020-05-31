using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditPartyContoroller : SceneController
{
    private DisplayUtil displayutil;

    public bool Executionflag {
        get; set;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
