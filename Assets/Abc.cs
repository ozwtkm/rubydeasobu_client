﻿using System.Collections;
using System.Collections.Generic;
using System.IO; // ←追加
using System.Runtime.Serialization.Json; // ←追加
using UnityEngine;
using System.Text;

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using System.Runtime.Serialization;

using GeneralPurpose;

[DataContract]
public class Username
{
    [DataMember]
    public string username { get; set;}
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

        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
        settings.MaxItemsInObjectGraph = 10; 

        string json = request.downloadHandler.text;
        Username data = JsonUtils.ToObject<Username>(json);
        Debug.Log(data.username);

        Debug.Log(request.downloadHandler.text);
    }
}


}