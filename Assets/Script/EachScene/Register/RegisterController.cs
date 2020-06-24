using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Json;
using GeneralPurpose;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class RegisterController : MonoBehaviour
{

    //todo constに引っ越し
    string URL = "http://rqmul.wfm.jp/user";


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Register(){
        Text usernameobj = GameObject.Find("Username/Text").GetComponent<Text>();
        Text passwordobj = GameObject.Find("Password/Text").GetComponent<Text>();

        string username = usernameobj.text;
        string password = passwordobj.text;

        //To Do:シリアライズちゃんとする
        string json = "[\"" + username + "\", \"" + password + "\"]";

        StartCoroutine(Post(URL, json));
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
            Username data = JsonUtils.ToObject<Username>(json);

            Messageobj.text = "ようこそ " + data.username + " !!";

            PlayerPrefs.SetString("username", data.username);

            print("LOGIN");
        }else{ // todo 配列で返ってきたパターンでちゃんと表示できるようにする
            ErrorResponse errorobj = JsonUtils.ToObject<ErrorResponse>(json);
            //errorobj.message = "test";
            Messageobj.text = errorobj.ErrorMessage;

            print("LOGINFAILED");
        }
    }
}
