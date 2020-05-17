using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using GeneralPurpose;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class DisplayUserinho : MonoBehaviour
{
    private GUIStyle style;

    private const int WALLET = 0;
    private const int MONSTER = 1;

    // Start is called before the first frame update
    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 50;


        GetInfo();
    }

    void GetInfo(){
        //定数まとめファイル作りたい
        const string WALLETURL = "http://rqmul.wfm.jp/wallet";
        const string MONSTERURL = "http://rqmul.wfm.jp/monsters/0";

        StartCoroutine(Displayuserinfo(WALLETURL, WALLET));
        StartCoroutine(Displayuserinfo(MONSTERURL, MONSTER));
    }


    IEnumerator Displayuserinfo(string url, int apikind){
        // To do: factoryなんちゃらみたいなデザインパターン使う
        
        UnityWebRequest request = new UnityWebRequest(url);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        string cookie = "session_id=" + PlayerPrefs.GetString("session_id");

        request.SetRequestHeader("Cookie", cookie);

        yield return request.Send();
        string json = request.downloadHandler.text;

        
        switch (apikind)
        {
            case WALLET:

                
                //DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                //settings.MaxItemsInObjectGraph = 10; 

                if (request.responseCode == 200){
                    var data = JsonUtils.ToObject<Wallet>(json);

                    Text Messageobj = GameObject.Find("Wallet").GetComponent<Text>();

                    Messageobj.text = "gem: " + data.gem.ToString() + "\n" + "money: " + data.money.ToString();


                    //PlayerPrefs.SetString("username", data.username);
                    Debug.Log(data.gem);
                    //print("LOGIN");
                }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
                    ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
                    //errorobj.message = "test";
                    //Messageobj.text = errorobj.ErrorMessage;

                    print("GETWALLETFAILED");
                }
        

                break;
            case MONSTER:                
                //DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                //settings.MaxItemsInObjectGraph = 10; 

                if (request.responseCode == 200){
                    var data = JsonUtils.ToObject<List<Monster>>(json);

                    foreach (Monster m in data){
                        GameObject monsterobj = GameObject.Find("Monsterinfo");
                        GameObject clone = GameObjectUtils.Clone(monsterobj);
                        clone.GetComponent<Text>().text = m.name;
                    }                    
                }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
                    ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
                    //errorobj.message = "test";
                    //Messageobj.text = errorobj.ErrorMessage;

                    print("GETMONSTERTFAILED");
                }

                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI () {
        Rect rect = new Rect(10, 10, 100, 100);

        string username = PlayerPrefs.GetString("username");
        string message = username + "でログイン中";

        GUI.Label(rect, message, style);
    }
}

public static class GameObjectUtils
{
    /// <summary>
    /// 指定された GameObject を複製して返します
    /// </summary>
    public static GameObject Clone( GameObject go )
    {
        var clone = GameObject.Instantiate( go ) as GameObject;
        clone.transform.parent = go.transform.parent;
        clone.transform.localPosition = go.transform.localPosition;
        clone.transform.localScale = go.transform.localScale;
        return clone;
    }
}

[DataContract]
public class Wallet{
    [DataMember]
    public int gem;

    [DataMember]
    public int money;
}



[DataContract]
public class Monsters{
    [DataMember]
    public Monster[] monsterlist;
}



[DataContract]
public class Monster{
    [DataMember]
    public string name ;

    [DataMember]
    public int atk;
}