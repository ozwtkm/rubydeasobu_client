using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaController : SceneController
{
    //private bool executionflag = false;
    private DisplayUtil displayutil;

    public bool Executionflag {
        get; set;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(WALLET);
        displayutil.WrappedGetAndRenderInfo(GACHA);

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
