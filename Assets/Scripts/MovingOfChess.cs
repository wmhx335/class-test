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
    public void IsMove(GameObject chessGo,GameObject targetGrid,int x1,int y1,int x2,int y2)
    {
        gameManager.ShowLastPositionUI(chessGo.transform.position);
        chessGo.transform.SetParent(targetGrid.transform);
        chessGo.transform.localPosition = Vector3.zero;
        gameManager.chessBoard[x2, y2] =gameManager.chessBoard[x1,y1];
        gameManager.chessBoard[x1, y1] =0;       
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
    /// 显示当前选中棋子可以移动到的位置
    /// </summary>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="chessMove">当前轮次</param>
    public void ClickChess(int FromX,int FromY,bool chessMove)
    {

    }
}
