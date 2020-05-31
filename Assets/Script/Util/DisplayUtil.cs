﻿using System.Collections;
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

public class DisplayUtil : MonoBehaviour
{
    private const int WALLET = 0;
    private const int MONSTER = 1;
    private const int USERNAME = 2;
    private const int GACHA = 3;

    // ライブラリ的に扱う想定なのでこいつ自身が何かを呼ぶのでなく他オブジェクトからwrappedgetandrenderinfoを叩く形で使う
    void Start()
    {/* 
        StartCoroutine(GetAndRenderInfo(WALLET));
        StartCoroutine(GetAndRenderInfo(MONSTER));
        StartCoroutine(GetAndRenderInfo(USERNAME)); //非同期処理噛ませる必要は無いが、統一的に扱いたくてこうしてる
    */}

    // 外からこいつを叩く！！
    public void WrappedGetAndRenderInfo(int apikind){
            StartCoroutine(GetAndRenderInfo(apikind));
    }

    public void WrappedGetAndRenderInfo(int apikind, int optionarg){
            StartCoroutine(GetAndRenderInfo(apikind, optionarg));
    }


    // Factoryパターンもどき？
    DisplayHandler CreateDisplayHandler(int apikind){
        DisplayHandler displayhandler;
        
        // switchの中で代入したdisplayhandlerは外から参照できないのでswitch内でreturn
        switch (apikind)
        {
            case WALLET:
                displayhandler = new DisplayWalletInfoHandler();
                return displayhandler;
                break;
            case MONSTER:
                displayhandler = new DisplayMonsterInfoHandler();
                return displayhandler;
                break;
            case USERNAME:
                displayhandler = new DisplayUsernameInfoHandler();
                return displayhandler;
                break;
            case GACHA:
                displayhandler = new DisplayGachaInfoHandler();
                return displayhandler;
                break;
            default:
                displayhandler = new DisplayDummyHandler();
                return displayhandler;
        }
                
    }

    // int型の引数が来たとき用のオーバーロード（こういう使い方で合ってるのか？
    DisplayHandler CreateDisplayHandler(int apikind, int optionarg){
        DisplayHandler displayhandler;
        
        // switchの中で代入したdisplayhandlerは外から参照できないのでswitch内でreturn
        switch (apikind)
        {
            case WALLET:
                displayhandler = new DisplayWalletInfoHandler();
                return displayhandler;
                break;
            case MONSTER:
                displayhandler = new DisplayMonsterInfoHandler(optionarg);
                return displayhandler;
                break;
            case USERNAME:
                displayhandler = new DisplayUsernameInfoHandler();
                return displayhandler;
                break;
            case GACHA:
                displayhandler = new DisplayGachaInfoHandler();
                return displayhandler;
                break;
            default:
                displayhandler = new DisplayDummyHandler();
                return displayhandler;
        }
                
    }


    IEnumerator GetAndRenderInfo(int apikind){
        DisplayHandler displayhandler = CreateDisplayHandler(apikind);

        yield return displayhandler.SendHttpRequest();

        displayhandler.render();
    }

    //int引数ありのオーバーロど
    IEnumerator GetAndRenderInfo(int apikind, int optionarg){
        DisplayHandler displayhandler = CreateDisplayHandler(apikind, optionarg);

        yield return displayhandler.SendHttpRequest();

        displayhandler.render();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI () {/* 
        Rect rect = new Rect(10, 10, 100, 100);

        string username = PlayerPrefs.GetString("username");
        string message = username + "でログイン中";

        GUI.Label(rect, message, style);*/
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
    public int id ;

    [DataMember]
    public string name ;

    [DataMember]
    public int atk;
}


[DataContract]
public class Gacha{
    [DataMember]
    public int id;

    [DataMember]
    public string name; 
}


[DataContract]
public class Party{
    [DataMember]
    public int partyid;

    [DataMember]
    public List<Monster> partyinfo;
}


public abstract class DisplayHandler{
    protected string url;
    protected UnityWebRequest request;
    
    public DisplayHandler(){
        //各具象クラスのコンストラクタでurlをセッティングする
        //もう少しいけてるやり方はありそう
    }

    public object SendHttpRequest(){
        request = new UnityWebRequest(url);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        string cookie = "session_id=" + PlayerPrefs.GetString("session_id");

        request.SetRequestHeader("Cookie", cookie);

        return request.Send();
    }

    public abstract void render();
}



public class DisplayDummyHandler : DisplayHandler{
    public override void render(){}
}


public class DisplayWalletInfoHandler : DisplayHandler{
    public DisplayWalletInfoHandler(){
        url = "http://rqmul.wfm.jp/wallet";
    }

    public override void render(){
        string json = request.downloadHandler.text;
 
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        if (request.responseCode == 200){
            var data = JsonUtils.ToObject<Wallet>(json);

            Text Messageobj = GameObject.Find("Wallet").GetComponent<Text>();

            Messageobj.text = "gem: " + data.gem.ToString() + "\n" + "money: " + data.money.ToString();
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
        // Todo:　→のエラー解消　XmlException: The token 'null' was expected but found 'naiy'.

            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            //Messageobj.text = errorobj.ErrorMessage;

            Debug.Log("GETWALLETFAILED");
        }
    }
}




//Todo ページング
public class DisplayMonsterInfoHandler : DisplayHandler{
    public DisplayMonsterInfoHandler(int offset=0){
        string offsetstring = offset.ToString();
        
        url = "http://rqmul.wfm.jp/monsters/" + offsetstring;
    }

    public override void render(){
        string json = request.downloadHandler.text;
 
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

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

            Debug.Log("GETMONSTERTFAILED");
        }
    }
}

public class DisplayUsernameInfoHandler : DisplayHandler{
    public DisplayUsernameInfoHandler(){
        url = "Dummy";
    }

    public object SendHttpRequest(){
        object dummyobj = new object();

        return dummyobj;
    }


    public override void render(){
        string username = PlayerPrefs.GetString("username");
        string message = username + "でログイン中";

        Text Messageobj = GameObject.Find("Username").GetComponent<Text>();

        Messageobj.text = message;
    }
}



public class DisplayGachaInfoHandler : DisplayHandler{
    public DisplayGachaInfoHandler(){
        url = "http://rqmul.wfm.jp/gacha";
    }

    public override void render(){
        string json = request.downloadHandler.text;
 
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        if (request.responseCode == 200){
            var data = JsonUtils.ToObject<List<Gacha>>(json);

            GameObject gachaobj = GameObject.Find("GachaInfo");

            foreach (Gacha m in data){
                GameObject clone = GameObjectUtils.Clone(gachaobj);
                clone.GetComponentInChildren<Text>().text = m.name;
                clone.GetComponent<GachaInfo>().Gachaid = m.id;
            }                    

            Object.Destroy(gachaobj.gameObject);
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            //Messageobj.text = errorobj.ErrorMessage;

            Debug.Log("GETGACHAFAILED");
        }
    }
}