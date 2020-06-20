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


public class EditPartyContoroller : SceneController
{

    private int monsterinfonextoffset = 10;
    private int monsterinfobackoffset = 0;
    private DisplayUtil displayutil;


    public bool Executionflag {
        get; set;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

        displayutil.WrappedGetAndRenderInfo(PARTY);

        int offset = 0;
        StartCoroutine(DisplayMonsterToggle(offset));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayNextMonsterInfo(){
        DestroyMonsterInfoClones();

        StartCoroutine(DisplayMonsterToggle(monsterinfonextoffset));

        monsterinfonextoffset += 10;
        monsterinfobackoffset += 10;
    }

    public void DisplayBackMonsterInfo(){
        DestroyMonsterInfoClones();

        StartCoroutine(DisplayMonsterToggle(monsterinfobackoffset));

        monsterinfonextoffset -= 10;
        monsterinfobackoffset -= 10;
    }

    //DisplayUtilに持たせたさはある
    private IEnumerator DisplayMonsterToggle(int offset=0){
        string offsetstring = offset.ToString();

        string url = "http://rqmul.wfm.jp/monsters/" + offsetstring;

        UnityWebRequest　request = new UnityWebRequest(url);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        string cookie = "session_id=" + PlayerPrefs.GetString("session_id");

        request.SetRequestHeader("Cookie", cookie);

        yield return request.Send();

        string json = request.downloadHandler.text;
 
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10;

        if (request.responseCode == 200){
            var data = JsonUtils.ToObject<List<Monster>>(json);

            GameObject monsterobj = GameObject.Find("Monsterinfo");
            
            
            
            foreach (Monster m in data){
                GameObject clone = GameObjectUtils.Clone(monsterobj);
                
                clone.tag = "clonemonsterinfo";
                //clone.GetComponent<Text>().text = m.name;

                clone.GetComponent<MonsterLabel>().usermonsterid = m.possession_id;
            }

            GameObject[] clonelabellist = GameObject.FindGameObjectsWithTag("clonemonsterinfo");

            int count = 0;
            foreach (GameObject g in clonelabellist){
                
                g.GetComponentInChildren<Text>().text = data[count].name;
                count++;
            }

        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            //Messageobj.text = errorobj.ErrorMessage;

            Debug.Log("GETMONSTERTFAILED");
        }
    }

    void UpdateParty(){
        int usermonnsterid = 4;

        //ToggleGroup mToggleGroup = new ToggleGroup();
        //Toggle tgl = mToggleGroup.ActiveToggles().ToArray().First();

        //間違いなくここは冗長なコード
        //todo unity側の機能でサクッとtoggleonのオブジェクト取ってこれる関数
        GameObject party1obj = GameObject.Find("Party1");
        bool party1objectison = party1obj.GetComponent<Toggle>().isOn;

        GameObject party2obj = GameObject.Find("Party2");
        bool party2objectison = party2obj.GetComponent<Toggle>().isOn;

        GameObject party3obj = GameObject.Find("Party3");
        bool party3objectison = party3obj.GetComponent<Toggle>().isOn;

        Dictionary<GameObject, bool> partydictionary= new Dictionary<GameObject, bool>(){
            {party1obj, party1objectison},
            {party2obj, party2objectison},
            {party3obj, party3objectison}
        };

        GameObject updatetarget = partydictionary.FirstOrDefault( c => c.Value == true ).Key;
        int updatetargetpartyid;

        //クソコード
       switch (updatetarget.name)
       {
            case "Party1":
                updatetargetpartyid = GameObject.Find("Party1info").GetComponent<PartyRadio>().id;
                break;
            case "Party2":
                updatetargetpartyid = GameObject.Find("Party2info").GetComponent<PartyRadio>().id;
                break;
            case "Party3":
                updatetargetpartyid = GameObject.Find("Party3info").GetComponent<PartyRadio>().id;
                break;   
            default:
                updatetargetpartyid = -1;
                break;
       }

       // [^0-9]がきたら例外処理

Debug.Log(updatetargetpartyid);


        var monsterinfos = GameObject.FindGameObjectsWithTag("clonemonsterinfo");
        int usermonsterid = -1;

        foreach (GameObject m in monsterinfos){
            if(m.name == "Monsterinfo(Clone)"){
                if(m.GetComponent<Toggle>().isOn){
                    usermonsterid = m.GetComponent<MonsterLabel>().usermonsterid;
                }
            }
        }

        if(usermonsterid == -1){
            // todo 例外処理ちゃんと書く
            Debug.Log("usermonsteridがちゃんと指定できてなくてアレ");
        }

        string json = "[" + updatetargetpartyid + "," + usermonsterid + "]";

Debug.Log(json);
        StartCoroutine(Put("http://rqmul.wfm.jp/party", json));
    }


    private void DestroyMonsterInfoClones(){
        var monsterinfos = GameObject.FindGameObjectsWithTag("clonemonsterinfo");
        foreach (GameObject m in monsterinfos){
            if(m.name == "Monsterinfo(Clone)"){
                Destroy(m);
            }
        }
    }



    IEnumerator Put(string url, string bodyJsonString){
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();

        if (request.responseCode == 201){
            Messageobj.text = "UPDATEしたよ";

            displayutil.WrappedGetAndRenderInfo(PARTY);

        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            
            Messageobj.text = request.downloadHandler.text;
        }
    }
}


/*
[DataContract]
public class Monster{
    [DataMember]
    public int id ;

    [DataMember]
    public string name ;

    [DataMember]
    public int atk;
}
*/
