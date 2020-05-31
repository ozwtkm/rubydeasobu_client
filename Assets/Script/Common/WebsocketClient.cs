using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Text;

using GeneralPurpose;




public class WebsocketClient: MonoBehaviour {

    WebSocket ws;
    private const string URL = "ws://rqmul.wfm.jp/unko";
    private List<string> messageque = new List<string>();

    void Start()
    {

        ws = new WebSocket(URL);

        string cookiename = "session_id";
        string cookievalue = PlayerPrefs.GetString("session_id");
        ws.SetCookie (new Cookie (cookiename, cookievalue));

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };

        ws.OnMessage += (sender, e) =>
        {
            //ここで直接レンダリングしようとするとmainではないスレッドになるのでGamObject探索とかするときエラーになる
            messageque.Add(e.Data);
            
            Debug.Log("WebSocket Message Type: " + e.GetType() + ", Data: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {Dbg.s(e);
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect();

    }

    void Update()
    {
        foreach(string message in messageque){
            WebsocketClientUtil.DisplayMessage(message);
        }

        messageque.Clear();

    }

    private void DisplayMessage(MessageEventArgs e){
            GameObject messageobj = GameObject.Find("Messageobj");
            //GameObject clone = GameObjectUtils.Clone(messageobj);
            //clone.GetComponent<Text>().text = "gergeargehea";
    }


    public void SendMessage(){
        Text remarkobj = GameObject.Find("ChatForm/Text").GetComponent<Text>();
        string remark = remarkobj.text;

        ws.Send(remark);
    }

    public void Exit(){
        ws.Close();
        ws = null;
    }

    public void SendTest()
    {
        ws.Send("Test Message From Client");
        Debug.Log("Done SendTest");
    }

    void OnDestroy()
    {
        ws.Close();
        ws = null;
    }

    public void Leave(){
        ws.Close();
    }
}


public class WebsocketClientUtil{
    public static void DisplayMessage(string message){
            GameObject messageobj = GameObject.Find("Messageobj");
            GameObject clone = GameObjectUtils.Clone(messageobj);
            clone.GetComponent<Text>().text = message;
    }}