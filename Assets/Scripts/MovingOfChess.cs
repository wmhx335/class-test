using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 棋子的移动类
/// </summary>
public class MovingOfChess
{
    private GameManager gameManager;
    public MovingOfChess(GameManager mGameManager)
    {
        gameManager = mGameManager;
    }

    /// <summary>
    /// 棋子的移动方法
    /// </summary>
    /// <param name="chessGo">什么棋子</param>
    /// <param name="targetGrid">目标格子</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void IsMove(GameObject chessGo, GameObject targetGrid, int x1, int y1, int x2, int y2)
    {
        gameManager.ShowLastPositionUI(chessGo.transform.position);
        chessGo.transform.SetParent(targetGrid.transform);
        chessGo.transform.localPosition = Vector3.zero;
        gameManager.chessBoard[x2, y2] = gameManager.chessBoard[x1, y1];
        gameManager.chessBoard[x1, y1] = 0;
    }

    /// <summary>
    /// 棋子的吃子方法
    /// </summary>
    /// <param name="firstChess">什么棋子</param>
    /// <param name="secondChess">被吃的棋子</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void IsEat(GameObject firstChess, GameObject secondChess, int x1, int y1, int x2, int y2)
    {
        gameManager.ShowLastPositionUI(firstChess.transform.position);
        GameObject secondChessGrid = secondChess.transform.parent.gameObject;//得到被吃棋子的父对象
        firstChess.transform.SetParent(secondChessGrid.transform);
        firstChess.transform.localPosition = Vector3.zero;
        gameManager.chessBoard[x2, y2] = gameManager.chessBoard[x1, y1];
        gameManager.chessBoard[x1, y1] = 0;
        gameManager.BeEat(secondChess);
    }
    /// <summary>
    /// 判断当前选中棋子及其可以移动到的位置
    /// </summary>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    public void ClickChess(int FromX, int FromY)
    {
        int chessID = gameManager.chessBoard[FromX, FromY];
        switch (chessID)
        {
            case 1://将
                GetJiangMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 8://帅
                GetShuaiMove(gameManager.chessBoard,FromX,FromY);
                break;
            case 2:
            case 9://車
                GetJuMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 3:
            case 10://马
                GetMaMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 4:
            case 11://炮
                GetPaoMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 5:
                GetB_ShiMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 12://士
                GetR_ShiMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 6:
            case 13://象
                GetXiangMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 7:
                GetZuMove(gameManager.chessBoard, FromX, FromY);
                break;
            case 14://兵
                GetBingMove(gameManager.chessBoard, FromX, FromY);
                break;
            default:
                break;
        }
    }

    #region 得到对应种类的棋子当前可以移动的所有路径
    /// <summary>
    /// 将帅
    /// </summary>

    private void GetJiangMove(int[,] position, int FromX, int FromY)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position,FromX,FromY,x,y))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
        }
    }
    private void GetShuaiMove(int[,] position, int FromX, int FromY)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
        }
    }
    /// <summary>
    /// 車
    /// </summary>
    private void GetJuMove(int[,] position, int FromX, int FromY)
    {

    }

    private void GetMaMove(int[,] position, int FromX, int FromY)
    {

    }

    private void GetPaoMove(int[,] position, int FromX, int FromY)
    {

    }

    private void GetB_ShiMove(int[,] position, int FromX, int FromY)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
        }
    }

    private void GetR_ShiMove(int[,] position, int FromX, int FromY)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
        }
    }

    private void GetXiangMove(int[,] position, int FromX, int FromY)
    {
        int x, y;
        //右下
        x = FromX + 2;
        y = FromY + 2;
        if (gameManager.rules.IsValidMove(position,FromX,FromY,x,y)&&x<10&&y<9)
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //右上
        x = FromX - 2;
        y = FromY + 2;
        if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y)&&x>=0&&y<9)
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左下
        x = FromX + 2;
        y = FromY - 2;
        if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y)&&x<10&&y>=0)
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左上
        x = FromX - 2;
        y = FromY - 2;
        if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y)&&x>=0&&y>=0)
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
    }

    private void GetZuMove(int[,] position, int FromX, int FromY)
    {

    }
    private void GetBingMove(int[,] position, int FromX, int FromY)
    {

    }

    #endregion

    private void GetCanMovePos(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {

    }
}
