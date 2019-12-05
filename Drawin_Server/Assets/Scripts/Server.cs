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
using System.IO;

public class Server : MonoBehaviour {

    public float fadeTime = 3f;
    public MeshStudy mesh1;
    Thread fileStreamThread1;
    public GameObject imgPlane1;
    bool needChangeImg = false;
    bool newImgComing1 = false;
    bool calibrateMode = false;
    bool removeImg1 = false;
    bool hideTimerStart = false;
    int showPeriod = 300;
    float hideTimer;
    string receivedPath;
    string imageURL1 = "";

    Socket socketListen;//用於監聽的socket
    Socket socketConnect;//用於通訊的socket
    string RemoteEndPoint;     //客戶端的網路節點  
    Dictionary<string, Socket> dicClient = new Dictionary<string, Socket>();//連線的客戶端集合

    const int MAX_CLIENTS = 10;
    const int MAX_Buffer_Size = 1024 * 50000;
    private Socket[] WorkerSocket = new Socket[MAX_CLIENTS];
    private int ClientCount = 0;
    private SocketPacket theSocPkt;
    private Socket newsock;
    public AsyncCallback pfnWorkerCallBack;
    delegate void SetTextCallback(int index, String text);
    public class SocketPacket
    {
        public Socket m_currentSocket;
        public byte[] dataBuffer = new byte[MAX_Buffer_Size];
        public StringBuilder sb = new StringBuilder();
    }

    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        StartCoroutine(StartUpFullscreen());

        imgPlane1.GetComponent<Renderer>().material.mainTexture = Texture2D.blackTexture;
        
        receivedPath = Application.persistentDataPath + "/";
                                  
        StartSocket();
        //
        //IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 1999);
        //newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //newsock.Bind(ipep);
        //newsock.Listen(10);
        //newsock.BeginAccept(new AsyncCallback(OnClientConnect), null);
        fileStreamThread1 = new Thread(FileStreamProcess1);
        fileStreamThread1.Start();

        print(Application.dataPath);
        if (PlayerPrefs.HasKey("ShowPeriod"))
        {
            showPeriod = PlayerPrefs.GetInt("ShowPeriod");
            print(showPeriod);
        }
        else
        {
            PlayerPrefs.SetInt("ShowPeriod", 300);
            showPeriod = 300;
        }
       
    }

    IEnumerator StartUpFullscreen()
    {
        Screen.SetResolution(1920, 1080, false);
        yield return new WaitForSeconds(1f);
        Screen.SetResolution(1920, 1080, true);
    }

    void FileStreamProcess1()
    {
        //
        int Port = 5501;
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        while(true)
        {
            Socket socket = listener.AcceptSocket();

            int bufferSize = 1024;
            byte[] buffer = null;
            byte[] header = null;
            string headerStr = "";
            string fileName = "";
            int filesize = 0;


            header = new byte[bufferSize];

            socket.Receive(header);

            headerStr = Encoding.ASCII.GetString(header);


            string[] splitted = headerStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string s in splitted)
            {
                if (s.Contains(":"))
                {
                    headers.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                }

            }
            //Get filesize from header
            filesize = Convert.ToInt32(headers["Content-length"]);
            //Get filename from header
            fileName = headers["Filename"];
            print(receivedPath + fileName);
            print(filesize);

            int bufferCount = Convert.ToInt32(Math.Ceiling((double)filesize / (double)bufferSize));


            FileStream fs = new FileStream(receivedPath + fileName, FileMode.OpenOrCreate);

            while (filesize > 0)
            {
                buffer = new byte[bufferSize];

                int size = socket.Receive(buffer, SocketFlags.Partial);

                fs.Write(buffer, 0, size);

                filesize -= size;
            }


            fs.Close();
            newImgComing1 = true;
            imageURL1 = receivedPath + fileName;
        }
    }

    // Update is called once per frame
    void Update () {
        
		if(newImgComing1 && needChangeImg)
        {
            StartCoroutine(ChangeImage1());
            newImgComing1 = false;
            needChangeImg = false;
        }
        
        if(calibrateMode)
        {
            imgPlane1.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("cali");
            imgPlane1.SetActive(true);
            calibrateMode = false;
        }

        if(removeImg1)
        {
            Texture2D texture = Texture2D.blackTexture;
            imgPlane1.GetComponent<Renderer>().material.mainTexture = texture;
            try
            {
                File.Delete(imageURL1);
            }
            catch(Exception e)
            {
                print(e.Message);
            }
            imgPlane1.SetActive(false);
            removeImg1 = false;
        }

        if (hideTimerStart)
        {
            hideTimer -= Time.deltaTime;
            if(hideTimer<=0)
            {
                imgPlane1.SetActive(false);
                 hideTimerStart = false;
            }

        }
	}

    IEnumerator ChangeImage1()
    {
        imgPlane1.SetActive(true);
        WWW www = new WWW("file:///"+imageURL1);
        while (!www.isDone)
            yield return null;
        float timer = fadeTime;
        while(timer>0)
        {
            imgPlane1.GetComponent<Renderer>().material.color = new Color(timer / fadeTime, timer / fadeTime, timer / fadeTime);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
      
        imgPlane1.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("whitelight_3");
        //imgPlane1.GetComponent<Renderer>().material.color = Color.white;
        /////
        timer = 0;
        while (timer < fadeTime)
        {
            imgPlane1.GetComponent<Renderer>().material.color = new Color(timer / fadeTime, timer / fadeTime, timer / fadeTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        timer = fadeTime;
        while (timer > 0)
        {
            imgPlane1.GetComponent<Renderer>().material.color = new Color(timer / fadeTime, timer / fadeTime, timer / fadeTime);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        timer = 0;
        while (timer < fadeTime)
        {
            imgPlane1.GetComponent<Renderer>().material.color = new Color(timer / fadeTime, timer / fadeTime, timer / fadeTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        timer = fadeTime;
        while (timer > 0)
        {
            imgPlane1.GetComponent<Renderer>().material.color = new Color(timer / fadeTime, timer / fadeTime, timer / fadeTime);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        //////

        Texture2D source = www.texture;
        Texture2D textureRev = new Texture2D(source.width, source.height);
        Color[] pix = source.GetPixels(0, 0, source.width, source.height);
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = new Color(1f - pix[i].r, 1f - pix[i].g, 1f - pix[i].b);
        }
        textureRev.SetPixels(pix);
        textureRev.Apply();
        imgPlane1.GetComponent<Renderer>().material.mainTexture = textureRev;
        timer = 0;
        while (timer < fadeTime)
        {
            imgPlane1.GetComponent<Renderer>().material.color = new Color(timer / fadeTime, timer / fadeTime, timer / fadeTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        hideTimer = (float)showPeriod;
        hideTimerStart = true;
    }

    public void StartSocket()
    {
        //建立套接字
        IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 5566);
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
                AsyncSend(socketConnect, string.Format("Welcome{0}", socketConnect.RemoteEndPoint));
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
                    if(length>0)
                    {
                        print(length);
                        print(Encoding.UTF8.GetString(data));
                        byte[] headerData = data.ToList().GetRange(0, 9).ToArray();
                        string header = Encoding.UTF8.GetString(headerData);
                        //guiText.text = header;
                        //print(header);
                        if(header.Contains("msg"))
                        {
                            byte[] msgData = data.ToList().GetRange(9, data.Length-9).ToArray(); 
                            string msg = Encoding.UTF8.GetString(msgData);
                            print(msg);
                            int moveVertex = 0;

                            if (msg.Contains("RTop"))
                            {
                                moveVertex = 0;
                            }
                            else if (msg.Contains("LTop"))
                            {
                                moveVertex = 1;
                            }
                            else if (msg.Contains("LBottom"))
                            {
                                moveVertex = 2;
                            }
                            else if (msg.Contains("RBottom"))
                            {
                                moveVertex = 3;
                            }

                            if(msg.Contains("Up"))
                            {
                                if (msg.Contains("P1"))
                                    mesh1.MoveVertex(moveVertex, new Vector3(0f, 0.005f, 0f));
                                calibrateMode = true;
                            }
                            else if (msg.Contains("Down"))
                            {
                                if (msg.Contains("P1"))
                                    mesh1.MoveVertex(moveVertex, new Vector3(0f, -0.005f, 0f));
                                calibrateMode = true;
                            }
                            else if (msg.Contains("Left"))
                            {
                                if (msg.Contains("P1"))
                                    mesh1.MoveVertex(moveVertex, new Vector3(0f, 0f, -0.005f));
                                calibrateMode = true;
                            }
                            else if (msg.Contains("Right"))
                            {
                                if (msg.Contains("P1"))
                                    mesh1.MoveVertex(moveVertex, new Vector3(0f, 0f, 0.005f));
                                calibrateMode = true;
                            }

                            if (msg.Contains("Clear"))
                            {
                                print("removeContent");
                                removeImg1 = true;
                            }

                            if (msg.Contains("Show"))
                            {
                                print("needChangeImg");
                                needChangeImg = true;
                            }
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
