using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
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
        SendMsg(new int[] { 3, 0, 0, 0, 0, 0 });
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
                    //处理接收到的数据
                    int[] result = BytesToInt(resultBuffer,0);
                    for (int i = 0; i < 6; i++)
                    {
                        GameManager.Instance.gameCodeReceive[i] = result[i];
                    }
                    GameManager.Instance.receiveNewCode = true;

                    //string msg = Encoding.UTF8.GetString(resultBuffer, 0, byteRead);
                    //Debug.Log(msg);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("客户端接收数据异常："+ex.Message);
                    Close();
                }
            } while (true);
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

    /// <summary>
    /// 发送信息
    /// </summary>
    public void SendMsg(int []gameCode)
    {
        if (localClient != null)
        {
            localClient.Client.Send(IntToBytes(gameCode,0));
        }
    }
}
