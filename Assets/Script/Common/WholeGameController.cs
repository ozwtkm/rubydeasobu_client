using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シングルトン
public class WholeGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string initialloadscenename = Constants.GAMESTARTSCENE;
        
        SceneChanger scenechanger = new SceneChanger();
        scenechanger.ChangeScene(initialloadscenename);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("ggg");
    }

    void Awake()
    {
       DontDestroyOnLoad(this);
    }

    //シングルトンにするためprivate
    private WholeGameController()
    {

    }
}
