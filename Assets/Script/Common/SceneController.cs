﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//各シーンごとに、そのシーンを俯瞰するhogecontrollerを作る
//その際このコントローらを継承する
public class SceneController : MonoBehaviour
{
    //todo constファイルに外だし
    protected const int WALLET = 0;
    protected const int MONSTER = 1;
    protected const int USERNAME = 2;
    protected const int GACHA = 3;
    protected const int PARTY = 4;

    protected const int RECIPE = 5;

    protected const int QUEST = 6;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
