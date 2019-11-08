using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
public class Server : MonoBehaviour {

    public Text guiText;

    Socket socketListen;//用於監聽的socket
    Socket socketConnect;//用於通訊的socket
    string RemoteEndPoint;     //客戶端的網路節點  
    Dictionary<string, Socket> dicClient = new Dictionary<string, Socket>();//連線的客戶端集合

	// Use this for initialization
	void Start () {
        StartSocket();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnApplicationQuit()
    {
        foreach(Socket clientSocket in dicClient.Values)
        {
            clientSocket.Close();
        }
        socketConnect.Close();
        socketListen.Close();
    }

    public void StartSocket()
    {
        //建立套接字
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5566);
        socketListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //繫結埠和IP
        socketListen.Bind(ipe);
        //設定監聽
        socketListen.Listen(10);
        //連線客戶端
        AsyncConnect(socketListen);
    }

    /// <summary>
    /// 連線到客戶端
    /// </summary>
    /// <param name="socket"></param>
    private void AsyncConnect(Socket socket)
    {
        try
        {
            socket.BeginAccept(asyncResult =>
            {
                //獲取客戶端套接字
                socketConnect = socket.EndAccept(asyncResult);
                RemoteEndPoint = socketConnect.RemoteEndPoint.ToString();
                dicClient.Add(RemoteEndPoint, socketConnect);//新增至客戶端集合
                //comboBox1.Items.Add(RemoteEndPoint);//新增客戶端埠號
                AsyncSend(socketConnect, string.Format("歡迎你{0}", socketConnect.RemoteEndPoint));
                AsyncReceive(socketConnect);
                AsyncConnect(socketListen);
            }, null);


        }
        catch (Exception ex)
        {

        }

    }

    /// <summary>
    /// 接收訊息
    /// </summary>
    /// <param name="client"></param>
    private void AsyncReceive(Socket socket)
    {
        byte[] data = new byte[1024];
        try
        {
            //開始接收訊息
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {
                try
                {
                    int length = socket.EndReceive(asyncResult);
                    if(true)
                    {
                        byte[] headerData = data.ToList().GetRange(0, 9).ToArray();
                        string header = Encoding.UTF8.GetString(headerData);
                        guiText.text = header;
                        if(header.Contains("msg"))
                        {
                            byte[] msgData = data.ToList().GetRange(10, data.Length-9).ToArray(); 
                            string msg = Encoding.UTF8.GetString(msgData);
                            guiText.text = header + "\n" + msg;
                        }
                        //string msg = Encoding.UTF8.GetString(data);
                        //guiText.text = header+"\n"+msg;
                        //if(msg.Contains("###img###"))
                        //{
                        //    guiText.text = "receive a image";
                        //}
                    }
                    //setText(Encoding.UTF8.GetString(data));
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

    private static byte[] ReceiveVarData(Socket s)
    {
        int total = 0;
        int recv;
        byte[] datasize = new byte[4];

        recv = s.Receive(datasize, 0, 4, 0);
        int size = BitConverter.ToInt32(datasize, 0);
        int dataleft = size;
        byte[] data = new byte[size];


        while (total < size)
        {
            recv = s.Receive(data, total, dataleft, 0);
            if (recv == 0)
            {
                break;
            }
            total += recv;
            dataleft -= recv;
        }
        return data;
    }

    /// <summary>
    /// 傳送訊息
    /// </summary>
    /// <param name="client"></param>
    /// <param name="p"></param>
    private void AsyncSend(Socket client, string message)
    {
        if (client == null || message == string.Empty) return;
        //資料轉碼
        byte[] data = Encoding.UTF8.GetBytes(message);
        try
        {
            //開始傳送訊息
            client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成訊息傳送
                int length = client.EndSend(asyncResult);
            }, null);
        }
        catch (Exception ex)
        {
            //傳送失敗，將該客戶端資訊刪除
            string deleteClient = client.RemoteEndPoint.ToString();
            dicClient.Remove(deleteClient);
        }
    }

}
