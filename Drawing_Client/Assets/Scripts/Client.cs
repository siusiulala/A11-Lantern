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
using UnityEngine.UI;

public class Client : MonoBehaviour {

    //
    string serverIP = "127.0.0.1";
    string projectorIP = "127.0.0.1";
    //
    Socket client;
    bool startConnect = false;
    //public GameObject disconnectDialog;

    float socketCheckTimer = 3f;
    const float socketCheckPeriod = 3f;
    //
    //Socket server;
    //public AsyncCallback pfnWorkerCallBack;
    //const int MAX_Buffer_Size = 1024;
    //delegate void SetTextCallback(int index, String text);
    ////IAsyncResult Result;
    //public class SocketPacket
    //{
    //    public Socket m_currentSocket;
    //    public byte[] dataBuffer = new byte[MAX_Buffer_Size];
    //    public StringBuilder sb = new StringBuilder();
    //}

    public int clientId = 1;
    //public Image[] cornerBtns = new Image[4];

	void Start () {
        DontDestroyOnLoad(this.gameObject);
        if (PlayerPrefs.HasKey("HostIp"))
        {
            serverIP = PlayerPrefs.GetString("HostIp");
        }
        GameObject.Find("ServerIpInput").GetComponent<InputField>().text = serverIP;

        if (PlayerPrefs.HasKey("ProjectorIp"))
        {
            projectorIP = PlayerPrefs.GetString("ProjectorIp");
        }
        GameObject.Find("ProjectorIpInput").GetComponent<InputField>().text = projectorIP;
    }
	
	// Update is called once per frame
	void Update () {

        if (client != null && client.Connected)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "1_Home")
                ToDrawView();
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "9_IpSetting")
                ToSettingView();
        }

        if (startConnect)
        {
            socketCheckTimer -= Time.deltaTime;
            if (socketCheckTimer <= 0)
            {
                
                if (SocketConnected(client) && Application.internetReachability != NetworkReachability.NotReachable)
                {
                    FindObjectOfType<DisconnectDialogController>().Hide();// disconnectDialog.SetActive(false);
                }
                else
                {
                    FindObjectOfType<DisconnectDialogController>().Show(); //disconnectDialog.SetActive(true);
                    AsyncConnect();
                }
                socketCheckTimer = socketCheckPeriod;
            }

        }
    }
    
    private void OnApplicationQuit()
    {
        if (client != null && client.Connected)
        {
            client.Close();
        }
    }

    bool SocketConnected(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        //print(part1 + ", " + part2);
        if (part1 && part2)
            return false;
        else
            return true;
    }
    /// <summary>
    /// 連線到伺服器
    /// </summary>
    public void AsyncConnect()
    {
        try
        {
            startConnect = true;
            //埠及IP
            //IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5566);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(serverIP), 5566);
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

    //public static void SendCallBack(IAsyncResult asyncResult)
    //{
    //    Socket server = (System.Net.Sockets.Socket)asyncResult.AsyncState;
    //    //int bytesSend = server.EndSend(asyncResult);
    //    //server.Close();
    //}

    public void SendFile(byte[] fileData)
    {
        string IPAddress = serverIP;
        int Port = 5500 + clientId;

        //string Filename = @"C:\Users\Ben\Desktop\TT.zip";


        int bufferSize = 1024;
        byte[] buffer = null;
        byte[] header = null;


        //FileStream fs = new FileStream(Filename, FileMode.Open);
        //bool read = true;

        int bufferCount = Convert.ToInt32(Math.Ceiling((double)fileData.Length / (double)bufferSize));



        TcpClient tcpClient = new TcpClient(IPAddress, Port);
        tcpClient.SendTimeout = 600000;
        tcpClient.ReceiveTimeout = 600000;
        string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
        string headerStr = "Content-length:" + fileData.Length.ToString() + "\r\nFilename:" + fileName + "\r\n";
        header = new byte[bufferSize];
        Array.Copy(Encoding.ASCII.GetBytes(headerStr), header, Encoding.ASCII.GetBytes(headerStr).Length);

        tcpClient.Client.Send(header);

        int leftsize = fileData.Length;
        for (int i = 0; i < bufferCount; i++)
        {
            int sendsize = (leftsize < bufferSize) ? leftsize : bufferSize;
            buffer = new byte[sendsize];
            //print(sendsize);
            //print(bufferSize * bufferCount);
            //print(fileData.ToList().Count);
            fileData.ToList().GetRange(i * bufferSize, sendsize).ToArray().CopyTo(buffer, 0);
            //int size = fs.Read(buffer, 0, bufferSize);

            tcpClient.Client.Send(buffer, sendsize, SocketFlags.Partial);

            leftsize -= sendsize;

        }

        tcpClient.Client.Close();

        ToLanternView();
        //fs.Close();
        /////////
        //print(fileData.Length);
        //StartCoroutine(DoSendFile(fileData));
        ////
        //server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //IPEndPoint iped = new IPEndPoint(IPAddress.Parse("192.168.0.116"), 1999);
        //try
        //{
        //    server.Connect(iped);
        //    string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
        //    byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
        //    //byte[] fileData = File.ReadAllBytes(openFileDialog.FileName.ToString());
        //    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
        //    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
        //    print(fileNameByte.Length);
        //    fileNameLen.CopyTo(clientData, 0);
        //    fileNameByte.CopyTo(clientData, 4);
        //    fileData.CopyTo(clientData, 4 + fileNameByte.Length);
        //    server.BeginSend(clientData, 0, clientData.Length, SocketFlags.None, new AsyncCallback(SendCallBack), server);
        //}
        //catch (Exception se)
        //{
        //    print(se.ToString());
        //}
        //while(!server.Connected)
        //{
            
        //}


        //if (client == null) return;
        ////編碼
        //byte[] headerData = Encoding.UTF8.GetBytes("###img###");
        //var newData = MergeByteArray(headerData, data);
        //try
        //{
        //    int totalsend = 0;
        //    int datasize = newData.Length;
        //    int dataleft = datasize;
        //    while (totalsend < datasize)
        //    {
        //        int sendSize = (dataleft < 1024000) ? dataleft : 1024000;
        //        print(sendSize);
        //        client.BeginSend(newData.ToList().GetRange(totalsend,sendSize).ToArray(), totalsend,sendSize, SocketFlags.None, asyncResult =>
        //        {
        //            //完成傳送訊息
        //            //int length = client.EndSend(asyncResult);
        //        }, null);
        //        totalsend += sendSize;
        //        dataleft -= sendSize;
        //    }

        //    //client.BeginSend(newData, 0, newData.Length, SocketFlags.None, asyncResult =>
        //    //{
        //    //    //完成傳送訊息
        //    //    int length = client.EndSend(asyncResult);
        //    //}, null);
        //}
        //catch (Exception ex)
        //{
        //}

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
    //IEnumerator DoSendFile(byte[] fileData)
    //{
    //    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //    IPEndPoint iped = new IPEndPoint(IPAddress.Parse(serverIP), 1999);
    //    try
    //    {
    //        server.Connect(iped);

    //    }
    //    catch (Exception se)
    //    {
    //        print(se.ToString());
    //    }
    //    yield return new WaitForSeconds(3);
    //    string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
    //    byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
    //    //byte[] fileData = File.ReadAllBytes(openFileDialog.FileName.ToString());
    //    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
    //    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
    //    print(fileData.Length);
    //    fileNameLen.CopyTo(clientData, 0);
    //    fileNameByte.CopyTo(clientData, 4);
    //    fileData.CopyTo(clientData, 4 + fileNameByte.Length);
    //    server.BeginSend(clientData, 0, clientData.Length, SocketFlags.None, new AsyncCallback(SendCallBack), server);
    //}

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
        //print(newData.Length);
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
                    if(length>0)
                    {
                        string msg = Encoding.UTF8.GetString(data);
                        Debug.Log(msg);
                        if (msg.Contains("Welcome"))
                        {
                            //ToDrawView();
                        }
                    }

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


    public void ToDrawView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("2_DrawViewV1");
    }

    public void ToCalibView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void ToLanternView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("3_Lantern");
    }

    public void ToSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5_SettingView");
    }

    public void ConnectToServer(InputField ipInputField)
    {
        serverIP = ipInputField.text;
        PlayerPrefs.SetString("HostIp",serverIP);
        PlayerPrefs.SetString("ProjectorIp", projectorIP);
        AsyncConnect();
    }

    public void ClientDisconnect()
    {
        startConnect = false;
        if (client != null && client.Connected)
        {
            client.Disconnect(false);
            client = null;
        }
    }
}
