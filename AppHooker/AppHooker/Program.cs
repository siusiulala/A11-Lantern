using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace AppHooker
{
    class Program
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(int hWnd);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(int handle, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        static int restartPeriod = 120; //60min

        static Socket socketListen;//用於監聽的socket
        static Socket socketConnect;//用於通訊的socket
        static string RemoteEndPoint;     //客戶端的網路節點  
        static Dictionary<string, Socket> dicClient = new Dictionary<string, Socket>();//連線的客戶端集合

        static void Main(string[] args)
        {
            StartSocket();

            int restartCuntDown = restartPeriod;
            while (true)
            {
                try
                {
                    Process[] p = Process.GetProcessesByName("彩繪天燈");
                    if (p.Length > 0)
                    {
                        //  Console.WriteLine("");
                        int hwnd = p[0].MainWindowHandle.ToInt32();
                        ShowWindow(hwnd, 3);
                        SetForegroundWindow(hwnd);
                        int memSize = GetMemSize(p[0]);

                        if (memSize > 2048000)
                        {
                            p[0].Kill();
                            Thread.Sleep(1000);
                            Process.Start("彩繪天燈.exe");
                        }

                        if (restartCuntDown <= 0)
                        {
                            /*   p[0].Kill();
                               Thread.Sleep(1000);
                               Process.Start("彩繪天燈.exe");*/
                            restartCuntDown = restartPeriod;
                        }

                    }
                    else
                    {
                        Process.Start("彩繪天燈.exe");
                    }
                    Thread.Sleep(30000);
                    restartCuntDown--;
                }
               catch(Exception e)
                {

                }
              
            }
           
         }
        static int GetMemSize(Process proc)
        {
            int memsize = 0; // memsize in Megabyte
            PerformanceCounter PC = new PerformanceCounter();
            PC.CategoryName = "Process";
            PC.CounterName = "Working Set - Private";
            PC.InstanceName = proc.ProcessName;
            memsize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            PC.Close();
            PC.Dispose();
            return memsize;
        }

         static void Restart()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C shutdown -f -r -t 5";
            Process.Start(proc);
        }

        static void StartSocket()
        {
            //建立套接字
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 2266);
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
        static void AsyncConnect(Socket socket)
        {
            try
            {
                socket.BeginAccept(asyncResult =>
                {
                    //獲取客戶端套接字
                    socketConnect = socket.EndAccept(asyncResult);
                    RemoteEndPoint = socketConnect.RemoteEndPoint.ToString();
                    dicClient.Add(RemoteEndPoint, socketConnect);
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
        static void AsyncReceive(Socket socket)
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
                        if (length > 0)
                        {
                            //print(length);
                            //print(Encoding.UTF8.GetString(data));
                            string msg = Encoding.UTF8.GetString(data);
                            Console.WriteLine(msg);
                            //guiText.text = header;
                            //print(header);
                            if (msg.Contains("reboot"))
                            {
                                Restart(); 
                            }
                        }
                    }
                    //setText(Encoding.UTF8.GetString(data));
                
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
    }
}
