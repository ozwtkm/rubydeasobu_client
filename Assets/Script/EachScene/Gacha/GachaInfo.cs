using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using UnityEngine.Networking;
using System.Text;
using GeneralPurpose;
using System.IO;
using UnityEngine.UI;



public class GachaInfo : MonoBehaviour
{
    private int gachaid;
    private static string URL="http://rqmul.wfm.jp/gacha";

    public int Gachaid
    {
        set { this.gachaid = value; }
        get { return this.gachaid; }
    }

    public void SetGachaid(int id){
        Gachaid = id;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoGacha(){
        string postdata = CreateJson();

        StartCoroutine(Post(postdata));

        Debug.Log(Gachaid);
    }

    private string CreateJson(){
        //List idlist = new List<int>();
        //idlist.Add(Gachaid);

        //To Do:シリアライズちゃんとする
        string json = "["+ Gachaid + "]";

        return json;
    }

    private void RenderMessage(){

    }

    IEnumerator Post(string postdata){
        UnityWebRequest request = new UnityWebRequest(URL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(postdata);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        GameObject Messageobj = GameObject.Find("Message");

        if (request.responseCode == 200){
            GachaResponse data = JsonUtils.ToObject<GachaResponse>(json);
//Dbg.s(Messageobj);
            Text Messagetext = Messageobj.GetComponent<Text>();
            Messagetext.text = data.name + "をゲットしました";
            

            GameObject gachacontrollerobj = GameObject.Find("GachaController");
            GachaController gachacontroller = gachacontrollerobj.GetComponent<GachaController>();

            gachacontroller.Executionflag = true;
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            Text Messagetext = Messageobj.GetComponent<Text>();

            Messagetext.text = errorobj.ErrorMessage;

            Debug.Log("GACHAFAILED");
        }
    }

    [DataContract]
    public class GachaResponse{
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public int rarity;
    }



}
