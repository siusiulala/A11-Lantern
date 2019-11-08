using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using IndieStudio.DrawingAndColoring.Logic;
using System.IO;

public class Client : MonoBehaviour {

    Socket client;

	// Use this for initialization
	void Start () {
        AsyncConnect();
	}
	
	// Update is called once per frame
	void Update () {
        if (client.Connected)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                AsyncSend(client, "按下W");
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                AsyncSend(client, "按下S");
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                AsyncSend(client, "按下A");
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                AsyncSend(client, "按下D");
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                //AsyncSend(client, "###img###asdasdsdsdsdsdsdsaaa");
                GameObject.FindObjectOfType<WebPrint>().PrintScreen();
            }
        }
	}

    private void OnApplicationQuit()
    {
        client.Close();
    }

    /// <summary>
    /// 連線到伺服器
    /// </summary>
    public void AsyncConnect()
    {
        try
        {
            //埠及IP
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5566);
            //建立套接字
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //開始連線到伺服器
            client.BeginConnect(ipe, asyncResult =>
            {
                client.EndConnect(asyncResult);
                //向伺服器傳送訊息
                AsyncSend(client, "你好我是客戶端");
                //接受訊息
                AsyncReceive(client);
            }, null);
        }
        catch (Exception ex)
        {

        }


    }

    public void SendFile(byte[] data)
    {
        if (client == null) return;
        //編碼
        byte[] headerData = Encoding.UTF8.GetBytes("###img###");
        var newData = MergeByteArray(headerData, data);
        try
        {

            client.BeginSend(newData, 0, newData.Length, SocketFlags.None, asyncResult =>
            {
                //完成傳送訊息
                int length = client.EndSend(asyncResult);
            }, null);
        }
        catch (Exception ex)
        {
        }

        //Debug.Log("SendFile:" + data.Length); 
        ////找到服务器的IP地址
        //IPAddress address = IPAddress.Parse("127.0.0.1");
        ////创建TcpClient对象实现与服务器的连接
        //TcpClient fileClient = new TcpClient();
        ////连接服务器
        //fileClient.Connect(address, 888);
        //using (client)
        //{
        //    //连接完服务器后便在客户端和服务端之间产生一个流的通道
        //    NetworkStream ns = fileClient.GetStream();
        //    using (ns)
        //    {
        //        //通过此通道将图片数据写入网络流，传向服务器端接收
        //        ns.Write(data, 0, data.Length);
        //    }
        //}
        //Debug.Log("End of SendFile");
    }

    public void AsyncSend(string message)
    {
        AsyncSend(client, message);
    }

    /// <summary>
    /// 傳送訊息
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="message"></param>
    public void AsyncSend(Socket socket, string message)
    {
        if (socket == null || message == string.Empty) return;
        //編碼
        byte[] data = Encoding.UTF8.GetBytes(message);
        byte[] headerData = Encoding.UTF8.GetBytes("###msg###");
        var newData = MergeByteArray(headerData, data);
        print(newData.Length);
        try
        {

            socket.BeginSend(newData, 0, newData.Length, SocketFlags.None, asyncResult =>
            {
                //完成傳送訊息
                int length = socket.EndSend(asyncResult);
            }, null);
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    /// 接收訊息
    /// </summary>
    /// <param name="socket"></param>
    public void AsyncReceive(Socket socket)
    {
        byte[] data = new byte[1024];
        try
        {

            //開始接收資料
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {
                try
                {
                    int length = socket.EndReceive(asyncResult);
                    Debug.Log(Encoding.UTF8.GetString(data));
                }
                catch (Exception)
                {
                    AsyncReceive(socket);
                }


                AsyncReceive(socket);
            }, null);
        }
        catch (Exception ex)
        {
        }
    }


    public byte[] MergeByteArray(byte[] bArrayA, byte[] bArrayB)
    {
        byte[] newArray = new byte[bArrayA.Length + bArrayB.Length];
        bArrayA.CopyTo(newArray, 0);
        bArrayB.CopyTo(newArray, bArrayA.Length);
        return newArray;
    }
}
