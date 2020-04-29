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

namespace GeneralPurpose {


public class GeneralPurpose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

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


[DataContract]
public class ErrorResponse
{
    [DataMember]
    public string ErrorMessage;
}

/*
[DataContract]
public class Username
{
    [DataMember]
    public string username { get; set;}
}
*/
}


