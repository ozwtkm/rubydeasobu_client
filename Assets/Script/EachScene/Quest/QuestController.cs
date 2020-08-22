using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using GeneralPurpose;
using System.Text;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

public class QuestController : SceneController
{
    //イベントごとの行動分岐用
    private const int NONE = 0;
    private const int ITEM = 1;
    private const int EQUIPMENT = 3;
    private const int BATTLE = 5;



    private const string URL = "http://rqmul.wfm.jp/quest";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void walk(int direction){
        int kind = 0; //walk

        string json = "[" + kind + "," + direction + "]";
        string url = "http://rqmul.wfm.jp/quest";

        StartCoroutine(WalkPut(json,url));
    }


    public void battle(){
        int command = 0;
        int subcommand = 0;

        string json = "[" + command + "," + subcommand + "]";
        string url = "http://rqmul.wfm.jp/battle";

        StartCoroutine(BattlePut(json, url));
    }

    public void cancel(){
        string url = "http://rqmul.wfm.jp/quest";
        StartCoroutine(CancelDelete(url));
    }


    public void action(){

    }

    public void Escape(){
        int kind = 3;
        int value = 0;

        string json = "[" + kind + "," + value + "]";
        string url = "http://rqmul.wfm.jp/quest";

        StartCoroutine(EscapePut(json,url));
    }

    //この粒度でpublic関数作りたくないけど、
    //onclickで呼べる関数は引数1つまでしか取れないという制限があるっぽい
    public void GetItem(){
        int kind = 1;
        int value = 0;

        string json = "[" + kind + "," + value + "]";
        string url = "http://rqmul.wfm.jp/quest";

        StartCoroutine(GetItemPut(json,url));
    }

    public void DropItem(){
        int kind = 1;
        int value = 1;

        string json = "[" + kind + "," + value + "]";
        string url = "http://rqmul.wfm.jp/quest";

        StartCoroutine(DropItemPut(json,url));     
    }


    private IEnumerator CancelDelete(string url){
        UnityWebRequest request = new UnityWebRequest(url, "DELETE");
        request.useHttpContinue = false;
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        yield return request.Send();

        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();

        if (request.responseCode != 205){
            Messageobj.text = request.downloadHandler.text;
        } else {
            Messageobj.text = "クエスト終了";
        }
    }


    private IEnumerator BattlePut(string bodyJsonString, string url){
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.useHttpContinue = false;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();

        
        if (request.responseCode != 200){
            Messageobj.text = request.downloadHandler.text;
        } else {
            //Messageobj.text = "";
            BattleRenderResponse(json);
        }
    }


    private IEnumerator WalkPut(string bodyJsonString, string url){
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.useHttpContinue = false;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        //Text Messageobj = GameObject.Find("Message").GetComponent<Text>();
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();
        
        if (request.responseCode != 200){
            Messageobj.text = request.downloadHandler.text;
        } else {
            Messageobj.text = "";
            WalkRenderResponse(json);
        }
    }

    private void WalkRenderResponse(string json){
        //2回パースするの馬鹿らしいが、
        //多分あとから特定の変数だけ動的型付けで設定するとかできないはずなので、
        //switch用とQuestinfo用で2回パースするのが無難かなと。。。
        QuestInfomation<QuestEventObj> questinfo = JsonUtils.ToObject<QuestInfomation<QuestEventObj>>(json);

        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();

        Match matche = Regex.Match(json, "\"object\":{([^{}]*)}");
        string json2 = "{" + matche.Groups[1].Value + "}";

        //var eventinfoclasstype = questinfo.eventinfo.GetType();
        //Debug.Log(questinfo.situation);
        //situationによりeventclassの構造とかレンダリングする内容が変わる。それはここで吸収
        switch (questinfo.situation)
        {
            case NONE:
                QuestInfomation<QuestEventObj> questinfo2 = JsonUtils.ToObject<QuestInfomation<QuestEventObj>>(json);
                break;
            case ITEM:
                QuestInfomation<ItemEventObj> questinfowithitem = JsonUtils.ToObject<QuestInfomation<ItemEventObj>>(json);
        
                ItemEventObj tmpitemobj = JsonUtils.ToObject<ItemEventObj>(json2);

                questinfowithitem.eventinfo = tmpitemobj;
                Messageobj.text = questinfowithitem.eventinfo.name + "があるよ";
                break;
            case EQUIPMENT:
                QuestInfomation<EquipmentEventObj> questinfowithequipment = JsonUtils.ToObject<QuestInfomation<EquipmentEventObj>>(json);
        
                EquipmentEventObj tmpequipobj = JsonUtils.ToObject<EquipmentEventObj>(json2);

                questinfowithequipment.eventinfo = tmpequipobj;
                Messageobj.text = questinfowithequipment.eventinfo.name + "があるよ";
                break;
            case BATTLE:
                QuestInfomation<BattleEventObj> questinfowithbattle = JsonUtils.ToObject<QuestInfomation<BattleEventObj>>(json);
        
                BattleEventObj tmpbattleobj = JsonUtils.ToObject<BattleEventObj>(json2);

                questinfowithbattle.eventinfo = tmpbattleobj;
                Messageobj.text = questinfowithbattle.eventinfo.name + "があるよ バトルだよ";

                break;
            default:
                break;
        }

        GameObject x = GameObject.Find("Coordinate/X");
        GameObject y = GameObject.Find("Coordinate/Y");
        GameObject z = GameObject.Find("Coordinate/Z");

        x.GetComponent<Text>().text = questinfo.x.ToString();
        y.GetComponent<Text>().text = questinfo.y.ToString();
        z.GetComponent<Text>().text = questinfo.z.ToString();
    }


    private void BattleRenderResponse(string json){
        //2回パースするの馬鹿らしいが、
        //多分あとから特定の変数だけ動的型付けで設定するとかできないはずなので、
        //switch用とQuestinfo用で2回パースするのが無難かなと。。。
        BattleInfo battleinfo = JsonUtils.ToObject<BattleInfo>(json);

        Text QuestLogObj = GameObject.Find("QuestLog/Text").GetComponent<Text>();


        if(battleinfo.finish_flg){
            QuestLogObj.text += "\nバトルに勝った";
        }else{
            
        }


    }


    private IEnumerator GetItemPut(string bodyJsonString, string url){
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.useHttpContinue = false;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();
        
        if (request.responseCode != 200){

            Messageobj.text = request.downloadHandler.text;
        } else {
            Messageobj.text = "";
            GetItemRenderResponse(json);
        }
    }


    private IEnumerator DropItemPut(string bodyJsonString, string url){
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.useHttpContinue = false;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();
        
        if (request.responseCode != 200){

            Messageobj.text = request.downloadHandler.text;
        } else {
            Messageobj.text = "";
            DropItemRenderResponse(json);
        }
    }


    private void GetItemRenderResponse(string json){
        Text log = GameObject.Find("QuestLog/Text").GetComponent<Text>();
        log.text += "\nアイテム拾った";
    }



    private void DropItemRenderResponse(string json){
        Text log = GameObject.Find("QuestLog/Text").GetComponent<Text>();
        log.text += "\nアイテム捨てた";
    }

/*
    private QuestInfomation DeserializeResponseJson(){
        QuestInfomation hoge = new QuestInfomation();

        return hoge;
    }
*/

    private IEnumerator EscapePut(string bodyJsonString, string url){
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.useHttpContinue = false;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10;

        //string json = request.downloadHandler.text;
        
        
        if (request.responseCode != 200){
            Text Messageobj = GameObject.Find("Message").GetComponent<Text>();
            Messageobj.text = request.downloadHandler.text;
        } else {
            Text Messageobj = GameObject.Find("QuestLog/Text").GetComponent<Text>();
            Messageobj.text += "\nクエストクリア";
        }
    }



}

//バトルのレスポンス
/*
{
  "before_scene": 2,
  "player": {
    "name": "aaaaaaaaaaaaaaa",
    "hp": 100,
    "mp": 7,
    "atk": 100,
    "def": 100,
    "speed": 4,
    "turn": 3
  },
  "partner": {
    "name": "inoue",
    "hp": 0,
    "mp": 700,
    "atk": 1,
    "def": 2,
    "speed": 1,
    "turn": 0
  },
  "enemy": {
    "name": "なみえる",
    "hp": 0,
    "mp": 7,
    "atk": 6,
    "def": 4,
    "speed": 4,
    "money": 33,
    "turn": 3
  },
  "finish_flg": true,
  "add_enemy_flg": false
}
*/



[DataContract]
public class BattleInfo
{
    [DataMember]
    public bool finish_flg;

    [DataMember]
    public bool add_enemy_flg;

    // todo HP表示とか
}





[DataContract]
public class QuestInfomation<T>
{
    [DataMember]
    public int x;
    [DataMember]
    public int y;
    [DataMember]
    public int z;
    [DataMember]
    public int situation;
    
    public T eventinfo; //ここは不定なので個別にパースする必要がある
}

public class QuestEventObj {
    //public string name;
}

public class BattleEventObj : QuestEventObj{
    public int id;　//敵モンスターのID
    public string name; //敵モンスターの名前
    //public int img_id;

  /*
  "object": {
    "id": 14,
    "name": "なみえる",
    "img_id": 6
  }
  */
}

public class EquipmentEventObj : QuestEventObj{
    public int id;
    public string name;

    public int kind;

    public int? value;

    /*
    "object":{"id":1,"name":"ダンジョン出口","kind":2,"value":null
    */
}
public class ItemEventObj : QuestEventObj{
    public int id;
    public string name;
    public int kind;
    public int value;
    //public int img_id;

    /*
    "object": {
        "id": 1,
        "name": "薬草",
        "kind": 2,
        "value": 10,
        "img_id": 100
    }
    }*/
}


/*
{
  "dangeon_info": [
    [
      {
        "8": true,
        "4": true
      },
      {
        "8": true,
        "4": true,
        "2": true
      },
      {
        "4": true,
        "2": true
      }
    ],
    [
      {
        "4": true,
        "1": true
      },
      {
        "1": true
      },
      {
        "4": true,
        "1": true
      }
    ],
    [
      {
        "8": true,
        "1": true
      },
      {
        "8": true,
        "2": true
      },
      {
        "2": true,
        "1": true
      }
    ]
  ],
  "team_info": {
    "party": 1,
    "partner": 5
  },
  "x": 1,
  "y": 1,
  "z": 1,
  "situation": 5,
  "object": {
    "id": 14,
    "name": "なみえる",
    "img_id": 6
  }
}
*/