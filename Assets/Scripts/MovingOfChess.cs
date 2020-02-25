using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 棋子的移动类
/// </summary>
public class MovingOfChess
{
    private GameManager gameManager;
    //当前搜索深度下的着法编号计数器
    public int moveCount;

    //存放所有合法着法的列表，第一个索引代表搜索深度，
    //第二个索引代表当前搜索深度下的着法编号
    public ChessReseting.Chess[,] moveList = new ChessReseting.Chess[8, 80];

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
        AudioSourceManager.Instance.PlaySound(1);
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
        AudioSourceManager.Instance.PlaySound(2);
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
                GetShuaiMove(gameManager.chessBoard, FromX, FromY);
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
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
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
        int x, y;
        int chessID;
        //得到当前选中棋子的ID，用于判断敌我
        chessID = position[FromX, FromY];
        //右
        x = FromX;
        y = FromY + 1;
        while (y < 9)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
                break;
            }
            y++;
        }
        //左
        x = FromX;
        y = FromY - 1;
        while (y >= 0)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
                break;
            }
            y--;
        }
        //下
        x = FromX + 1;
        y = FromY;
        while (x < 10)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
                break;
            }
            x++;
        }
        //上
        x = FromX - 1;
        y = FromY;
        while (x >= 0)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
                break;
            }
            x--;
        }
    }

    private void GetMaMove(int[,] position, int FromX, int FromY)
    {
        int x, y;
        //竖
        //右下
        x = FromX + 2;
        y = FromY + 1;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //右上
        x = FromX - 2;
        y = FromY + 1;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左下
        x = FromX + 2;
        y = FromY - 1;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左上
        x = FromX - 2;
        y = FromY - 1;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }

        //横
        //右下
        x = FromX + 1;
        y = FromY + 2;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //右上
        x = FromX - 1;
        y = FromY + 2;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左下
        x = FromX + 1;
        y = FromY - 2;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左上
        x = FromX - 1;
        y = FromY - 2;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
    }

    private void GetPaoMove(int[,] position, int FromX, int FromY)
    {
        int x, y;
        //是否满足翻山条件
        bool flag;
        int chessID;
        chessID = position[FromX, FromY];
        //右
        x = FromX;
        y = FromY + 1;
        flag = false;
        while (y < 9)
        {
            //是否为空格
            if (position[x, y] == 0)
            {
                //未达成翻山条件前显示所有可移动路径，达成之后不可空翻
                if (!flag)
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
            //棋子
            else
            {
                //条件未满足时，开启flag
                if (!flag)
                {
                    flag = true;
                }
                //判断敌我
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, FromX, FromY, x, y);
                    }
                    break;
                }
            }
            y++;
        }

        //左
        y = FromY - 1;
        flag = false;
        while (y >= 0)
        {
            if (position[x, y] == 0)
            {
                if (!flag)
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
            else
            {
                if (!flag)
                {
                    flag = true;
                }
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, FromX, FromY, x, y);
                    }
                    break;
                }
            }
            y--;
        }

        //下
        x = FromX + 1;
        y = FromY;
        flag = false;
        while (x < 10)
        {
            //是否为空格
            if (position[x, y] == 0)
            {
                //未达成翻山条件前显示所有可移动路径，达成之后不可空翻
                if (!flag)
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
            //棋子
            else
            {
                //条件未满足时，开启flag
                if (!flag)
                {
                    flag = true;
                }
                //判断敌我
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, FromX, FromY, x, y);
                    }
                    break;
                }
            }
            x++;
        }

        //上
        x = FromX - 1;
        flag = false;
        while (x >= 0)
        {
            //是否为空格
            if (position[x, y] == 0)
            {
                //未达成翻山条件前显示所有可移动路径，达成之后不可空翻
                if (!flag)
                {
                    GetCanMovePos(position, FromX, FromY, x, y);
                }
            }
            //棋子
            else
            {
                //条件未满足时，开启flag
                if (!flag)
                {
                    flag = true;
                }
                //判断敌我
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, FromX, FromY, x, y);
                    }
                    break;
                }
            }
            x--;
        }
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
        if (x < 10 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //右上
        x = FromX - 2;
        y = FromY + 2;
        if (x >= 0 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左下
        x = FromX + 2;
        y = FromY - 2;
        if (x < 10 && y >= 0 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //左上
        x = FromX - 2;
        y = FromY - 2;
        if (x >= 0 && y >= 0 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
    }

    private void GetZuMove(int[,] position, int FromX, int FromY)
    {
        int x, y;
        int chessID;
        chessID = position[FromX, FromY];
        x = FromX + 1;
        y = FromY;
        if (x < 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        if (FromX > 4)
        {
            x = FromX;
            y = FromY + 1;//右
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            y = FromY - 1;//左
            if (y >= 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
        }

    }

    private void GetBingMove(int[,] position, int FromX, int FromY)
    {
        int x, y;
        int chessID;
        chessID = position[FromX, FromY];
        x = FromX - 1;
        y = FromY;
        if (x > 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        //过河后
        if (FromX < 5)
        {
            x = FromX;
            y = FromY + 1;//右
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            y = FromY - 1;//左
            if (y >= 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
        }

    }

    #endregion

    #region 添加对应种类的棋子当前可以移动的所有着法到着法列表


    /// <summary>
    /// 将帅
    /// </summary>

    private void GetJiangMove(int[,] position, int FromX, int FromY,int depth)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
        }
    }

    private void GetShuaiMove(int[,] position, int FromX, int FromY, int depth)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
        }
    }
    /// <summary>
    /// 車
    /// </summary>
    private void GetJuMove(int[,] position, int FromX, int FromY, int depth)
    {
        int x, y;
        int chessID;
        //得到当前选中棋子的ID，用于判断敌我
        chessID = position[FromX, FromY];
        //右
        x = FromX;
        y = FromY + 1;
        while (y < 9)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
                break;
            }
            y++;
        }
        //左
        x = FromX;
        y = FromY - 1;
        while (y >= 0)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
                break;
            }
            y--;
        }
        //下
        x = FromX + 1;
        y = FromY;
        while (x < 10)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
                break;
            }
            x++;
        }
        //上
        x = FromX - 1;
        y = FromY;
        while (x >= 0)
        {
            //判断空格子
            if (position[x, y] == 0)
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            else
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
                break;
            }
            x--;
        }
    }

    private void GetMaMove(int[,] position, int FromX, int FromY, int depth)
    {
        int x, y;
        //竖
        //右下
        x = FromX + 2;
        y = FromY + 1;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //右上
        x = FromX - 2;
        y = FromY + 1;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //左下
        x = FromX + 2;
        y = FromY - 1;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //左上
        x = FromX - 2;
        y = FromY - 1;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }

        //横
        //右下
        x = FromX + 1;
        y = FromY + 2;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //右上
        x = FromX - 1;
        y = FromY + 2;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //左下
        x = FromX + 1;
        y = FromY - 2;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //左上
        x = FromX - 1;
        y = FromY - 2;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
    }

    private void GetPaoMove(int[,] position, int FromX, int FromY, int depth)
    {
        int x, y;
        //是否满足翻山条件
        bool flag;
        int chessID;
        chessID = position[FromX, FromY];
        //右
        x = FromX;
        y = FromY + 1;
        flag = false;
        while (y < 9)
        {
            //是否为空格
            if (position[x, y] == 0)
            {
                //未达成翻山条件前显示所有可移动路径，达成之后不可空翻
                if (!flag)
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
            //棋子
            else
            {
                //条件未满足时，开启flag
                if (!flag)
                {
                    flag = true;
                }
                //判断敌我
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, FromX, FromY, x, y,depth);
                    }
                    break;
                }
            }
            y++;
        }

        //左
        y = FromY - 1;
        flag = false;
        while (y >= 0)
        {
            if (position[x, y] == 0)
            {
                if (!flag)
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
            else
            {
                if (!flag)
                {
                    flag = true;
                }
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, FromX, FromY, x, y,depth);
                    }
                    break;
                }
            }
            y--;
        }

        //下
        x = FromX + 1;
        y = FromY;
        flag = false;
        while (x < 10)
        {
            //是否为空格
            if (position[x, y] == 0)
            {
                //未达成翻山条件前显示所有可移动路径，达成之后不可空翻
                if (!flag)
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
            //棋子
            else
            {
                //条件未满足时，开启flag
                if (!flag)
                {
                    flag = true;
                }
                //判断敌我
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, FromX, FromY, x, y,depth);
                    }
                    break;
                }
            }
            x++;
        }

        //上
        x = FromX - 1;
        flag = false;
        while (x >= 0)
        {
            //是否为空格
            if (position[x, y] == 0)
            {
                //未达成翻山条件前显示所有可移动路径，达成之后不可空翻
                if (!flag)
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
            //棋子
            else
            {
                //条件未满足时，开启flag
                if (!flag)
                {
                    flag = true;
                }
                //判断敌我
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, FromX, FromY, x, y,depth);
                    }
                    break;
                }
            }
            x--;
        }
    }

    private void GetB_ShiMove(int[,] position, int FromX, int FromY, int depth)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
        }
    }

    private void GetR_ShiMove(int[,] position, int FromX, int FromY, int depth)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
                {
                    AddMove(position, FromX, FromY, x, y,depth);
                }
            }
        }
    }

    private void GetXiangMove(int[,] position, int FromX, int FromY, int depth)
    {
        int x, y;
        //右下
        x = FromX + 2;
        y = FromY + 2;
        if (x < 10 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //右上
        x = FromX - 2;
        y = FromY + 2;
        if (x >= 0 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //左下
        x = FromX + 2;
        y = FromY - 2;
        if (x < 10 && y >= 0 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //左上
        x = FromX - 2;
        y = FromY - 2;
        if (x >= 0 && y >= 0 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
    }

    private void GetZuMove(int[,] position, int FromX, int FromY, int depth)
    {
        int x, y;
        int chessID;
        chessID = position[FromX, FromY];
        x = FromX + 1;
        y = FromY;
        if (x < 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        if (FromX > 4)
        {
            x = FromX;
            y = FromY + 1;//右
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            y = FromY - 1;//左
            if (y >= 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
        }

    }

    private void GetBingMove(int[,] position, int FromX, int FromY, int depth)
    {
        int x, y;
        int chessID;
        chessID = position[FromX, FromY];
        x = FromX - 1;
        y = FromY;
        if (x > 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        //过河后
        if (FromX < 5)
        {
            x = FromX;
            y = FromY + 1;//右
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            y = FromY - 1;//左
            if (y >= 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
        }

    }

    #endregion

    private void GetCanMovePos(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        if (!gameManager.rules.KingKill(position, FromX, FromY, ToX, ToY))
        {
            return;
        }
        GameObject item;
        //是空格子
        if (position[ToX, ToY] == 0)
        {
            item = gameManager.PopCanMoveUI();
        }
        //是可吃棋子
        else
        {
            item = gameManager.canEatPosUIGo;
        }
        item.transform.SetParent(gameManager.boardGrid[ToX, ToY].transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;

    }

    /// <summary>
    /// AI具体走棋方法
    /// </summary>
    public void HaveGoodMove(ChessReseting.Chess aChessStep)
    {
        //走棋
        if (aChessStep.chessTwo == null)
        {
            gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.chessOneID, aChessStep.chessTwoID);
            IsMove(aChessStep.chessOne, aChessStep.gridTwo,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex);
        }
        //吃子
        else
        {
            gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount,
              aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
              aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
              aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
              aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex,
              aChessStep.chessOneID, aChessStep.chessTwoID);
            IsEat(aChessStep.chessOne, aChessStep.chessTwo,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex);
        }
    }

    /// <summary>
    /// 产生当前局面所有棋子可能移动着法的方法
    /// </summary>
    /// <param name="position">当前棋盘状况</param>
    /// <param name="depth">搜索深度</param>
    /// <param name="side">当前是哪方视角,false时为AI层即奇数层，true为player层即偶数层</param>
    /// <returns></returns>
    public int CreatePossibleMove(int[,] position, int depth, bool side)
    {
        int chessID;
        moveCount = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (position[i,j]!=0)
                {
                    chessID = position[i,j];
                    if (!side&&gameManager.rules.isRed(chessID))
                    {
                        //偶数层AI产生黑方走法跳过红方
                        continue;
                    }
                    if (side&&gameManager.rules.isBlack(chessID))
                    {
                        //奇数层player产生红方走法跳过黑方
                        continue;
                    }
                    switch (chessID)
                    {
                        case 1://将
                            GetJiangMove(position, i, j,depth);
                            break;
                        case 8://帅
                            GetShuaiMove(position, i, j, depth);
                            break;
                        case 2:
                        case 9://車
                            GetJuMove(position, i, j, depth);
                            break;
                        case 3:
                        case 10://马
                            GetMaMove(position, i, j, depth);
                            break;
                        case 4:
                        case 11://炮
                            GetPaoMove(position, i, j, depth);
                            break;
                        case 5:
                            GetB_ShiMove(position, i, j, depth);
                            break;
                        case 12://士
                            GetR_ShiMove(position, i, j, depth);
                            break;
                        case 6:
                        case 13://象
                            GetXiangMove(position, i, j, depth);
                            break;
                        case 7:
                            GetZuMove(position, i, j, depth);
                            break;
                        case 14://兵
                            GetBingMove(position, i, j, depth);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        return moveCount;
    }

    /// <summary>
    /// 当时AI行走时，把所有可能的着法存入着法列表
    /// </summary>
    /// <param name="position"></param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="ToX"></param>
    /// <param name="ToY"></param>
    /// <param name="depth"></param>
    private void AddMove(int[,] position,int FromX,int FromY,int ToX,int ToY,int depth)
    {
        //将帅不在一条直线上
        if (gameManager.rules.KingKill(position,FromX,FromY,ToX,ToY))
        {
            //把当前着法存进着法列表
            moveList[depth, moveCount].from.x = FromX;
            moveList[depth, moveCount].from.y = FromY;
            moveList[depth, moveCount].to.x = ToX;
            moveList[depth, moveCount].to.y = ToY;
            moveCount++;
           // gameManager.searchEngine.AddHistoryScore(moveList[depth,moveCount],depth);
        }
    }
}
