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
public class QusetPreparationController : SceneController
{
    private DisplayUtil displayutil;
    // Start is called before the first frame update
    void Start()
    {
        GameObject displayhandler = GameObject.Find("DisplayUtil");
        displayutil = displayhandler.GetComponent<DisplayUtil>();

       // displayutil.WrappedGetAndRenderInfo(QUEST);
        displayutil.WrappedGetAndRenderInfo(PARTY);
        displayutil.WrappedGetAndRenderInfo(QUEST);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartQuest(){
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

        GameObject partyforquest = partydictionary.FirstOrDefault( c => c.Value == true ).Key;
        int partyforquestid;

        //クソコード
       switch (partyforquest.name)
       {
            case "Party1":
                partyforquestid = GameObject.Find("Party1info").GetComponent<PartyRadio>().id;
                break;
            case "Party2":
                partyforquestid = GameObject.Find("Party2info").GetComponent<PartyRadio>().id;
                break;
            case "Party3":
                partyforquestid = GameObject.Find("Party3info").GetComponent<PartyRadio>().id;
                break;   
            default:
                partyforquestid = -1;
                break;
       }

        if (partyforquestid == -1){
            //例外処理
        }

        var questinfos = GameObject.FindGameObjectsWithTag("Quest");
        int questid = -1;

        foreach (GameObject m in questinfos){          
            if(m.GetComponent<Toggle>().isOn){
                questid = m.GetComponent<QuestInfo>().id;
            }
        }

       //Debug.Log(updatetargetpartyid);
       int partnerid = 5; //まだ取得APIを作成してないのでハードコード

       string json = "[" + partnerid + "," + partyforquestid + "," + questid + "]";

        Debug.Log(json);
        StartCoroutine(Post("http://rqmul.wfm.jp/quest", json));
    }


    IEnumerator Post(string url, string bodyJsonString){
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();
    
        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Text Messageobj = GameObject.Find("Message").GetComponent<Text>();

        if (request.responseCode == 200){
            Messageobj.text = "クエスト開始した";
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            Messageobj.text = request.downloadHandler.text;
        }
    }
}
