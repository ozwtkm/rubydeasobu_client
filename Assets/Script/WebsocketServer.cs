using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

using GeneralPurpose;

public class WebsocketServer: MonoBehaviour {

    WebSocketServer server;

    void Start ()
    {
        server = new WebSocketServer(3100);

        server.AddWebSocketService<Echo>("/");
        server.Start();
Dbg.m(server, "server");
    }

    void OnDestroy()
    {
        server.Stop();
        server = null;
    }

    public void Broadcast(MessageEventArgs e)
    {
        //Sessions.Broadcast(e.Data);
    }

}

public class Echo : WebSocketBehavior
{
    protected override void OnMessage (MessageEventArgs e)
    {
        Sessions.Broadcast(e.Data);
    }
}