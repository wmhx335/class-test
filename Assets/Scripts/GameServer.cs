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

    public GameServer(string address, int port)
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
        IPAddress ip = IPAddress.Parse(address);
        TcpListener tcpListener = new TcpListener(ip, port);
        //监听对象开始监听
        tcpListener.Start();
        Debug.Log("服务器已启动");
        //如果有远程客户端连接，此时就会得到一个对象用于通讯，两边联通
        romoteClient = tcpListener.AcceptTcpClient();
        Debug.Log("客户端连接成功");
        SendMsg(new int[] { 3, 0, 0, 0, 0, 0 });
        NetworkStream stream = romoteClient.GetStream();
        //循环接收信息输入
        do
        {
            try
            {
                //获取与客户端连接的数据
                byte[] buffer = new byte[bufferSize];
                //读取信息
                int byteRead = stream.Read(buffer, 0, bufferSize);
                //客户端断开连接
                if (byteRead == 0)
                {
                    Debug.Log("客户端断开");
                    break;
                }
                //具体处理接受信息的数据
                int[] result = BytesToInt(buffer, 0);
                for (int i = 0; i < 6; i++)
                {
                    GameManager.Instance.gameCodeReceive[i] = result[i];
                }
                GameManager.Instance.receiveNewCode = true;

                //string msg = Encoding.UTF8.GetString(buffer, 0, byteRead);
                //Debug.Log(msg);
            }
            catch (System.Exception ex)
            {
                Debug.Log("客户端异常：" + ex.Message);
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
        if (romoteClient != null)
        {
            romoteClient.Client.Shutdown(SocketShutdown.Both);
        }
        if (connectThread != null)
        {
            connectThread.Abort();
        }
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    public void SendMsg(int []gameCode)
    {
        if (romoteClient != null)
        {
            romoteClient.Client.Send(IntToBytes(gameCode,0));
        }
    }

    /// <summary>
    /// int 转byte
    /// </summary>
    /// <param name="src"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private byte[] IntToBytes(int[] src, int offset)
    {
        byte[] values = new byte[src.Length * 4];
        for (int i = 0; i < src.Length; i++)
        {
            values[offset] = (byte)src[i];
            // >> 右移XX位
            values[offset] = (byte)(src[i] >> 8);
            values[offset + 2] = (byte)(src[i] >> 16);
            values[offset + 3] = (byte)(src[i] >> 24);
            offset += 4;
        }
        return values;
    }

    /// <summary>
    /// byte数组转int数组
    /// </summary>
    /// <returns></returns>
    private int[] BytesToInt(byte[] src, int offset)
    {
        int[] values = new int[src.Length / 4];
        for (int i = 0; i < src.Length / 4; i++)
        {
            int value = (int)(src[offset] & 0xFF)
            | (int)(src[offset + 1] & 0xFF << 8)
            | (int)(src[offset + 2] & 0xFF << 16)
            | (int)(src[offset + 3] & 0xFF << 24);
            values[i] = value;
            offset += 4;
        }
        return values;
    }
}
