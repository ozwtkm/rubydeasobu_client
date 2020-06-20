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
public class RcipeInfo : MonoBehaviour
{
    public int id;

    private static string URL="http://rqmul.wfm.jp/gradeup";


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DoGradeUp(){
        string postdata = CreateJson();

        StartCoroutine(Post(postdata));
    }

    private string CreateJson(){
        //To Do:シリアライズちゃんとする
        string json = "["+ id + "]";

        return json;
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
            GradeupResponse data = JsonUtils.ToObject<GradeupResponse>(json);

            Text Messagetext = Messageobj.GetComponent<Text>();
            Messagetext.text = data.name + "をゲットしました";
            

            GameObject synthesiscontrollerobj = GameObject.Find("SynthesisController");
            SynthesisController synthesiscontroller = synthesiscontrollerobj.GetComponent<SynthesisController>();

            synthesiscontroller.Executionflag = true;
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            Text Messagetext = Messageobj.GetComponent<Text>();

            Messagetext.text = errorobj.ErrorMessage;

            Debug.Log("GRADEUPFAILED");
        }
    }


    [DataContract]
    public class GradeupResponse{
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public int rarity;
    }

}
