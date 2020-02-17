using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
/// <summary>
/// 客户端
/// </summary>
public class GameCilient 
{
    private string serverAddress;
    private int port;
    private TcpClient localClient;
    private Thread receiveThread;

    public GameCilient(string serverAddress,int port)
    {
        this.serverAddress = serverAddress;
        this.port = port;
    }

    /// <summary>
    /// 关闭客户端
    /// </summary>
    public void Close()
    {
        if (localClient!=null)
        {
            localClient.Close();
        }
        if (receiveThread!=null)
        {
            receiveThread.Abort();
        }
    }

    /// <summary>
    /// 启动客户端的方法
    /// </summary>
    /// <returns></returns>
    public bool Start()
    {
        localClient = new TcpClient();

        try
        {
            localClient.Connect(IPAddress.Parse(serverAddress),port);

            receiveThread = new Thread(SocketReceiver);
            receiveThread.Start();
            Debug.Log("客户端启动");
        }
        catch (System.Exception ex)
        {
            Debug.Log("客户端连接服务器异常:"+ex.Message);
            if (!localClient.Connected)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 客户端检测接收到的服务器信息
    /// </summary>
    private void SocketReceiver()
    {
        if (localClient!=null)
        {
            //缓冲区
            int bufferSize = 8192;
            byte[] resultBuffer = new byte[bufferSize];
            do
            {
                try
                {
                    int byteRead =localClient.Client.Receive(resultBuffer);
                    if (byteRead==0)
                    {
                        Debug.Log("与服务器连接中断");
                        break;
                    }
                    //处理接收到的数据 todo

                }
                catch (System.Exception ex)
                {
                    Debug.Log("客户端接收数据异常："+ex.Message);
                    Close();
                }
            } while (true);
        }
    }
}
