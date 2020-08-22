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

public class DisplayUtil : MonoBehaviour
{
    private const int WALLET = 0;
    private const int MONSTER = 1;
    private const int USERNAME = 2;
    private const int GACHA = 3;
    private const int PARTY = 4;

    private const int RECIPE = 5;

    private const int QUEST = 6;

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
            case PARTY:
                displayhandler = new DisplayPartyInfoHandler();
                return displayhandler;
                break;
            case RECIPE:
                displayhandler = new DisplayRecipeInfoHandler();
                return displayhandler;
                break;
            case QUEST:
                displayhandler = new DisplayQuestInfoHandler();
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
    public int possession_id ;

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
    public Monster monsterinfo;
}

[DataContract]
public class Recipe{
    [DataMember]
    public int id;

    [DataMember]
    public string name;
}

[DataContract]
public class Quest{
    [DataMember]
    public int id;

    [DataMember]
    public string name;
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





public class DisplayRecipeInfoHandler : DisplayHandler{
    public DisplayRecipeInfoHandler(){
        url = "http://rqmul.wfm.jp/gradeup";
    }

    public override void render(){
        string json = request.downloadHandler.text;
 
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        if (request.responseCode == 200){
            var data = JsonUtils.ToObject<List<Recipe>>(json);

            GameObject recipeobj = GameObject.Find("Recipe");

            foreach (Recipe m in data){
                Dbg.s(m);
                GameObject clone = GameObjectUtils.Clone(recipeobj);
                clone.GetComponentInChildren<Text>().text = m.name;
                clone.GetComponent<RcipeInfo>().id = m.id;


    
            }                    

            Object.Destroy(recipeobj.gameObject);
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            //Messageobj.text = errorobj.ErrorMessage;

            Debug.Log("GETRECIPEFAILED");
        }
    }
}







public class DisplayPartyInfoHandler : DisplayHandler{
    public DisplayPartyInfoHandler(){
        url = "http://rqmul.wfm.jp/party";
    }

    public override void render(){
        string json = request.downloadHandler.text;

        if (request.responseCode == 200){
            List<Party> list = GetPartyInfoFromJson(json);

            GameObject party1infoobj = GameObject.Find("Party1info");
            party1infoobj.GetComponent<Text>().text = list[0].monsterinfo.name;

            GameObject party2infoobj = GameObject.Find("Party2info");
            party2infoobj.GetComponent<Text>().text = list[1].monsterinfo.name;

            GameObject party3infoobj = GameObject.Find("Party3info");
            party3infoobj.GetComponent<Text>().text = list[2].monsterinfo.name;

            party1infoobj.GetComponent<PartyRadio>().id = list[0].partyid;
            party2infoobj.GetComponent<PartyRadio>().id = list[1].partyid;
            party3infoobj.GetComponent<PartyRadio>().id = list[2].partyid;

            /*foreach (Party p in list){
                GameObject monsterobj = GameObject.Find("Monsterinfo");
                GameObject clone = GameObjectUtils.Clone(monsterobj);
                clone.GetComponent<Text>().text = p.monsterinfo.name;
            }*/
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            //Messageobj.text = errorobj.ErrorMessage;

            Debug.Log("GETMONSTERTFAILED");
        }
    }

    //こんなものを書く必要はない気がするが、
    //DataContoractJsonSerializerでどうしてもJsonから抽出がうまく行かなかったので..
    public List<Party> GetPartyInfoFromJson(string json){
        Dictionary<string, string> dictionary;
        //jsonsample   @"{""190"":{""name"":""dragon"",""rarity"":2,""hp"":1000,""mp"":7,""speed"":4,""atk"":1000,""def"":1000},""6"":{""name"":""aaaaaaaaaaaaaaa"",""rarity"":1,""hp"":100,""mp"":7,""speed"":4,""atk"":100,""def"":100},""8"":{""name"":""aaaaaaaaaaaaaaa"",""rarity"":1,""hp"":100,""mp"":7,""speed"":4,""atk"":100,""def"":100}}";

        // パーティの仕様は決まってるので正規表現はハードコードで
        Match matche = Regex.Match(json, "\"([0-9]+)\":{(.+)},\"([0-9]+)\":{(.+)},\"([0-9])+\":{(.+)}");
    
        int party1id = int.Parse(matche.Groups[1].Value);
        string party1value = matche.Groups[2].Value;

        int party2id = int.Parse(matche.Groups[3].Value);
        string party2value = matche.Groups[4].Value;

        int party3id = int.Parse(matche.Groups[5].Value);
        string party3value = matche.Groups[6].Value;

        List<Party> partylist = new List<Party>();

        Party party1info = new Party();
        party1info.partyid = party1id;
        party1info.monsterinfo = new Monster();

        Party party2info = new Party();
        party2info.partyid = party2id;
        party2info.monsterinfo = new Monster();

        Party party3info = new Party();
        party3info.partyid = party3id;
        party3info.monsterinfo = new Monster();


        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10;

        string party1monsterinfojson = "{" +party1value + "}";
        Monster monsterinfo1 = JsonUtils.ToObject<Monster>(party1monsterinfojson);

        string party2monsterinfojson = "{" + party2value + "}";
        Monster monsterinfo2 = JsonUtils.ToObject<Monster>(party2monsterinfojson);

        string party3monsterinfojson = "{" + party3value + "}";
        Monster monsterinfo3 = JsonUtils.ToObject<Monster>(party3monsterinfojson);

        party1info.monsterinfo = monsterinfo1;
        party2info.monsterinfo = monsterinfo2;
        party3info.monsterinfo = monsterinfo3;

        partylist.Add(party1info);
        partylist.Add(party2info);
        partylist.Add(party3info);

        return partylist;
    }

}











public class DisplayQuestInfoHandler : DisplayHandler{
    public DisplayQuestInfoHandler(){
        url = "http://rqmul.wfm.jp/dangeon";
    }

    public override void render(){
        string json = request.downloadHandler.text;
 
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();

        if (request.responseCode == 200){
            var data = JsonToQuestList(json);

            GameObject questobj = GameObject.Find("Quest");

            //Debug.Log(data.Key);
                //Debug.Log(data.FirstOrDefault());
            Debug.Log(data.Count);

            foreach (KeyValuePair<int, string> m in data){
                //Dbg.s(m);
                GameObject clone = GameObjectUtils.Clone(questobj);
                clone.GetComponentInChildren<Text>().text = m.Value;
                clone.GetComponent<QuestInfo>().id = m.Key;
            }

            Object.Destroy(questobj.gameObject);
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            //Messageobj.text = errorobj.ErrorMessage;

            Debug.Log("GETQUESTFAILED");
        }
    }


    private Dictionary<int, string> JsonToQuestList(string json){
        //{"1":"井上の洞窟"}
        Dictionary<int, string> aaa = new Dictionary<int, string>();

        if (Regex.IsMatch("ABC", "gewa")){
            //ダンジョンが複数あり、[{"1":"gwea"},{"2":"hyj"}]みたいな形式で来たときはこっち。()
            //todo

        }else{
            Debug.Log(json);
            //json = "{\"jjjjjjjjjj\":\"gaawwwew\"}";
            //aaa = JsonUtils.ToObject<Dictionary<string, string>>(json);
            
            //不特定数の複数jsonきたときってどうすっかここ
            Match matche = Regex.Match(json, "{\"([0-9+])\":\"(.+)\"}");

            int questid = int.Parse(matche.Groups[1].Value);
            string questname = matche.Groups[2].Value;

            aaa.Add(questid, questname);

        }

        return aaa;
    }
}













