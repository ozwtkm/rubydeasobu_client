using System.Collections;
using System.Collections.Generic;
using System.IO; // ←追加
using System.Runtime.Serialization.Json; // ←追加
using UnityEngine;
using System.Text;

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using System.Runtime.Serialization;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

[DataContract]
public class Username
{
    [DataMember]
    public string username { get; set;}
}

[DataContract]
class Data
{
    [DataMember]
    public int a;

    [DataMember]
    public double d;

    [DataMember]
    public string s;
}

namespace JsonTestApp
{
    public static class JsonUtils
    {
        /// <summary>
        /// JSONからオブジェクトへ変換します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">オブジェクトへ変換するJSON</param>
        /// <returns>オブジェクト</returns>
        public static T ToObject<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }

    [DataContract] // ←重要
    class SampleData
    {
        [DataMember] // ←重要
        public string name { get; set; }
        
        [DataMember] // ←重要
        public string age { get; set; }
    }
}


namespace JsonTestApp
{
public class Abc : MonoBehaviour
{
    private const string URL = "http://rqmul.wfm.jp/login";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onclick()
    {
        string json = "[\"kkk\",\"k\"]";

        Debug.Log("AAAAAAAAAAAAA");
        //StartCoroutine("OnSend2", URL);
        StartCoroutine(Post(URL, json));
    }

    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);

        //FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);

        //var ms = new MemoryStream();
        //var sr = new StreamReader(ms);

        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        //var serializer = new DataContractJsonSerializer(typeof(Username), settings);

        //Username loginname = serializer.ReadObject(ms) as Username;
        //ms.Position = 0;

        //var json = sr.ReadToEnd();


        //string json = @"{""a"":123, ""s"":""test!"", ""pi"":3.14}";
/*
        var serializer = new DataContractJsonSerializer(typeof(Data));
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        Debug.Log(ms);
        var data = serializer.ReadObject(ms);
        Data fff = new Data();

        fff.s = "fwfaf";

Debug.Log(fff.s);
Debug.Log(data.s);
Debug.Log("ffffLLLL");
*/

        string json = request.downloadHandler.text;
        //string json = "{\"age\":\"44歳\",\"name\":\"大泉 洋\"}";
        Username data = JsonUtils.ToObject<Username>(json);
        Debug.Log(data.username);
        //Debug.Log(data.age);

        //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Username));
        //Username loginname = serializer.ReadObject("{\"username\":\"kk\"}") as Username;

        Debug.Log(request.downloadHandler.text);
        //Debug.Log(request.responseBody.ToString());
    }

    
    IEnumerator OnSend(string url)
    {
        //URLをGETで用意
        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        //URLに接続して結果が戻ってくるまで待機
        yield return webRequest.SendWebRequest();

        //エラーが出ていないかチェック
        if (webRequest.isNetworkError)
        {
            //通信失敗
            Debug.Log(webRequest.error);
        }
        else
        {
            //通信成功
            Debug.Log(webRequest.downloadHandler.text);
        }
    }



    IEnumerator OnSend2(string url)
    {
        //POSTする情報
        WWWForm form = new WWWForm();
        form.AddField("name", 1000001);

        //URLをPOSTで用意
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        //UnityWebRequestにバッファをセット
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        //URLに接続して結果が戻ってくるまで待機
        yield return webRequest.SendWebRequest();

        //エラーが出ていないかチェック
        if (webRequest.isNetworkError)
        {
            //通信失敗
            Debug.Log(webRequest.error);
        }
        else
        {
            //通信成功
            Debug.Log(webRequest.downloadHandler.text);
        }
    }

    void readjson(){

    }




    public void test(){
        Debug.Log("kkkkkkkkkkkk");

        //string testjsonstr = "[1,3,5]";

        var p = new Person();
        p.Name = "aKato Jun";
        p.Age = 31;

        var ms = new MemoryStream();
        var sr = new StreamReader(ms);

        //MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testjsonstr));

        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
    
        settings.MaxItemsInObjectGraph = 10; //これを越えるとWriteObject時エラーが起きる

        var serializer = new DataContractJsonSerializer(typeof(Person), settings);


        serializer.WriteObject(ms, p);
        ms.Position = 0;

        var json = sr.ReadToEnd();
        //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(object));
        //object hoge = serializer.ReadObject(stream);

        Debug.Log(json+"fffffffffffffffffffffffffffffffff");
    }
}


}