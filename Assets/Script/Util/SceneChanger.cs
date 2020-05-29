using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // int渡して条件分岐の方が綺麗ではあるが、まあ便宜的にこれくらいは文字列直渡しでも良い？
    public void ChangeScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void test()
    {

    }
}
