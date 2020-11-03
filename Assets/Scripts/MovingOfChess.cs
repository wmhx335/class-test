using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingOfChess
{
    private GameManager gameManager;
    public int moveCount;

    public ChessReseting.Chess[,] moveList = new ChessReseting.Chess[8, 80];

    public MovingOfChess(GameManager mGameManager)
    {
        gameManager = mGameManager;
    }

    public void IsMove(GameObject chessGo, GameObject targetGrid, int x1, int y1, int x2, int y2)
    {
        AudioSourceManager.Instance.PlaySound(1);
        gameManager.ShowLastPositionUI(chessGo.transform.position);
        chessGo.transform.SetParent(targetGrid.transform);
        chessGo.transform.localPosition = Vector3.zero;
        gameManager.chessBoard[x2, y2] = gameManager.chessBoard[x1, y1];
        gameManager.chessBoard[x1, y1] = 0;
    }

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

    #region MovePath


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
        chessID = position[FromX, FromY];
        x = FromX;
        y = FromY + 1;
        while (y < 9)
        {
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
        x = FromX;
        y = FromY - 1;
        while (y >= 0)
        {
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
        x = FromX + 1;
        y = FromY;
        while (x < 10)
        {
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
        x = FromX - 1;
        y = FromY;
        while (x >= 0)
        {
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
        x = FromX + 2;
        y = FromY + 1;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX - 2;
        y = FromY + 1;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX + 2;
        y = FromY - 1;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX - 2;
        y = FromY - 1;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX + 1;
        y = FromY + 2;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX - 1;
        y = FromY + 2;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX + 1;
        y = FromY - 2;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
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
        bool flag;
        int chessID;
        chessID = position[FromX, FromY];
        x = FromX;
        y = FromY + 1;
        flag = false;
        while (y < 9)
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
            y++;
        }

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

        x = FromX + 1;
        y = FromY;
        flag = false;
        while (x < 10)
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
            x++;
        }

        x = FromX - 1;
        flag = false;
        while (x >= 0)
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
            x--;
        }
    }

    private void GetB_ShiMove(int[,] position, int FromX, int FromY)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 9; y++)
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
        for (int x = 5; x < 10; x++)
        {
            for (int y = 0; y < 9; y++)
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
        x = FromX + 2;
        y = FromY + 2;
        if (x < 10 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX - 2;
        y = FromY + 2;
        if (x >= 0 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
        x = FromX + 2;
        y = FromY - 2;
        if (x < 10 && y >= 0 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            GetCanMovePos(position, FromX, FromY, x, y);
        }
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
            y = FromY + 1;
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            y = FromY - 1;
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
        if (FromX < 5)
        {
            x = FromX;
            y = FromY + 1;
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
            y = FromY - 1;
            if (y >= 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, FromX, FromY, x, y);
            }
        }

    }

    #endregion

    #region CanMovePosition


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
        chessID = position[FromX, FromY];
        x = FromX;
        y = FromY + 1;
        while (y < 9)
        {
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
        x = FromX;
        y = FromY - 1;
        while (y >= 0)
        {
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
        x = FromX + 1;
        y = FromY;
        while (x < 10)
        {
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
        x = FromX - 1;
        y = FromY;
        while (x >= 0)
        {
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
        x = FromX + 2;
        y = FromY + 1;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX - 2;
        y = FromY + 1;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX + 2;
        y = FromY - 1;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX - 2;
        y = FromY - 1;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }

        x = FromX + 1;
        y = FromY + 2;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX - 1;
        y = FromY + 2;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX + 1;
        y = FromY - 2;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
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
        bool flag;
        int chessID;
        chessID = position[FromX, FromY];
        x = FromX;
        y = FromY + 1;
        flag = false;
        while (y < 9)
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
            y++;
        }

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

        x = FromX + 1;
        y = FromY;
        flag = false;
        while (x < 10)
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
            x++;
        }

        x = FromX - 1;
        flag = false;
        while (x >= 0)
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
            x--;
        }
    }

    private void GetB_ShiMove(int[,] position, int FromX, int FromY, int depth)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 9; y++)
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
        for (int x = 5; x < 10; x++)
        {
            for (int y = 0; y < 9; y++)
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
        x = FromX + 2;
        y = FromY + 2;
        if (x < 10 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX - 2;
        y = FromY + 2;
        if (x >= 0 && y < 9 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
        x = FromX + 2;
        y = FromY - 2;
        if (x < 10 && y >= 0 && gameManager.rules.IsValidMove(position, FromX, FromY, x, y))
        {
            AddMove(position, FromX, FromY, x, y,depth);
        }
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
            y = FromY + 1;
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            y = FromY - 1;
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
        if (FromX < 5)
        {
            x = FromX;
            y = FromY + 1;
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, FromX, FromY, x, y,depth);
            }
            y = FromY - 1;
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
        if (position[ToX, ToY] == 0)
        {
            item = gameManager.PopCanMoveUI();
        }
        else
        {
            item = gameManager.canEatPosUIGo;
        }
        item.transform.SetParent(gameManager.boardGrid[ToX, ToY].transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;

    }

    /// <summary>
    /// AIMove
    /// </summary>
    public void HaveGoodMove(ChessReseting.Chess aChessStep)
    {
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
                        continue;
                    }
                    if (side&&gameManager.rules.isBlack(chessID))
                    {
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

    private void AddMove(int[,] position,int FromX,int FromY,int ToX,int ToY,int depth)
    {
        if (gameManager.rules.KingKill(position,FromX,FromY,ToX,ToY))
        {
            moveList[depth, moveCount].from.x = FromX;
            moveList[depth, moveCount].from.y = FromY;
            moveList[depth, moveCount].to.x = ToX;
            moveList[depth, moveCount].to.y = ToY;
            moveCount++;
        }
    }
}
