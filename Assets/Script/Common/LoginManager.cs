using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // ←追加
using System.Runtime.Serialization.Json; // ←追加
using GeneralPurpose;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

 
public class LoginManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
 
    public void Login()
    {
        //ログインユーザ名を取得
        Text usernameobj = GameObject.Find("Input_name/Text").GetComponent<Text>();
        Text passwordobj = GameObject.Find("Input_password/Text").GetComponent<Text>();

        //Text型をstring型に変換
        string username = usernameobj.text;
        string password = passwordobj.text;

        //To Do:シリアライズちゃんとする
        string json = "[\"" + username + "\", \"" + password + "\"]";

        //string json = "[\"kkk\",\"k\"]";
        
        string URL = "http://rqmul.wfm.jp/login";

        StartCoroutine(Post(URL, json));
    }
 

    IEnumerator Post(string url, string bodyJsonString){
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        string setcookieheader = request.GetResponseHeader("Set-cookie");
    
        // To Do: 正規表現ちゃんと考える
        Match matche = Regex.Match(setcookieheader, "session_id=(.*)");
        
        string cookie = matche.Groups[1].Value;

        // to do: cookieの管理方法ちゃんと調べる
        PlayerPrefs.SetString("session_id", cookie);

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