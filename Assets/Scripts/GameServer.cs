using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class GameServer 
{
    //当前服务器监听子线程
    private Thread connectThread;
    //当前IP地址
    private string address;
    //当前本地端口
    public int port;
    //远程客户端
    private TcpClient romoteClient;

    public GameServer(string address,int port)
    {
        this.address = address;
        this.port = port;
    }

    /// <summary>
    /// 实例化服务器端套接字对象
    /// </summary>
    private void InitServerSocket()
    {
        //缓冲区的大小
        int bufferSize = 8192;
        IPAddress ip =IPAddress.Parse(address);
        TcpListener tcpListener = new TcpListener(ip,port);
        //监听对象开始监听
        tcpListener.Start();
        Debug.Log("服务器已启动");
        //如果有远程客户端连接，此时就会得到一个对象用于通讯，两边联通
        romoteClient = tcpListener.AcceptTcpClient();
        Debug.Log("客户端连接成功");
        NetworkStream stream=romoteClient.GetStream();
        //循环接收信息输入
        do
        {
            try
            {
                //获取与客户端连接的数据
                byte[] buffer = new byte[bufferSize];
                //读取信息
                int byteRead = stream.Read(buffer,0,bufferSize);
                //客户端断开连接
                if (byteRead==0)
                {
                    Debug.Log("客户端断开");
                    break;
                }
                //具体处理接受信息的数据
                string msg = Encoding.UTF8.GetString(buffer,0,byteRead);
                Debug.Log(msg);
            }
            catch (System.Exception ex)
            {
                Debug.Log("客户端异常："+ex.Message);
                tcpListener.Stop();
            }
        } while (true);
    }

    /// <summary>
    /// 启动服务器
    /// </summary>
    public void Start()
    {
        connectThread = new Thread(InitServerSocket);
        connectThread.Start();

    }

    /// <summary>
    /// 关闭服务器
    /// </summary>
    public void Close()
    {
        //如果客户端连接着服务器，那么需要关闭
        if (romoteClient!=null)
        {
            romoteClient.Client.Shutdown(SocketShutdown.Both);
        }
        if (connectThread!=null)
        {
            connectThread.Abort();
        }
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    private void SendMsg()
    {
        if (romoteClient!=null)
        {
            romoteClient.Client.Send(Encoding.UTF8.GetBytes("Hello Client,this is Server！"));
        }
    }
}
