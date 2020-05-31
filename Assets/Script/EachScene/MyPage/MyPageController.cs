using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPageController : SceneController
{
    private int monsterinfonextoffset = 10;
    private int monsterinfobackoffset = 0;
    private static int INITIALOFFSET = 0;

    DisplayUtil displayutil;

    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(WALLET);
        displayutil.WrappedGetAndRenderInfo(MONSTER, INITIALOFFSET);
        displayutil.WrappedGetAndRenderInfo(USERNAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayNextMonsterInfo(){
        DestroyMonsterInfoClones();

        displayutil.WrappedGetAndRenderInfo(MONSTER, monsterinfonextoffset);

        monsterinfonextoffset += 10;
        monsterinfobackoffset += 10;
    }

    public void DisplayBackMonsterInfo(){
        DestroyMonsterInfoClones();

        displayutil.WrappedGetAndRenderInfo(MONSTER, monsterinfobackoffset);

        monsterinfonextoffset -= 10;
        monsterinfobackoffset -= 10;
    }


    private void DestroyMonsterInfoClones(){
        var monsterinfos = GameObject.FindGameObjectsWithTag("MonsterInfoTag");
        foreach (GameObject m in monsterinfos){
            if(m.name == "Monsterinfo(Clone)"){
                Destroy(m);
            }
        }
    }
}
