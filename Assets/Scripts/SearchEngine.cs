using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SearchAI
/// </summary>
public class SearchEngine
{
    public int searchDepth;
    private GameManager gameManager;
    private ChessReseting.Chess bestStep;
    private int[,] unrealBoard = new int[10, 9];
    private ChessReseting.ChessSteps[] relatePos = new ChessReseting.ChessSteps[30];
    private int posCount;

    public SearchEngine()
    {
        gameManager = GameManager.Instance;
    }
    //価値
    private int[] baseValue = new int[15]
    {
        //将 车 马 炮 士 象 兵
        0,10000,900,400,450,250,350,100,
          10000,900,400,450,250,350,100
    };
    private int[] flexValue = new int[15]
    {
     //将 车 马 炮 士 象 兵
        0, 0, 6, 12, 6, 2, 3, 1,
           0, 6, 12, 6, 2, 3, 1
    };
    private int[,] attackPos;
    private int[,] guardPos;
    private int[,] flexPos;
    private int[,] chessValue;

    private int[,] r_bingValue = new int[10, 9]
    {
        {0,0,0,0,0,0,0,0,0} ,
        {90,90,110,120,120,120,110,90,90} ,
        {90,90,110,120,120,120,110,90,90} ,
        {70,90,110,110,110,110,110,90,70} ,
        {70,70,70,70,70,70,70,70,70} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,

    };
    private int[,] b_bingValue = new int[10, 9]
    {
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {70,70,70,70,70,70,70,70,70} ,
        {70,90,110,110,110,110,110,90,70} ,
        {90,90,110,120,120,120,110,90,90} ,
        {90,90,110,120,120,120,110,90,90} ,
        {0,0,0,0,0,0,0,0,0} ,
    };

    public ChessReseting.Chess SearchGoodMove(int[,] position)
    {
        searchDepth = gameManager.currentLevel;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unrealBoard[i, j] = position[i, j];
            }
        }
        //调用具体搜索算法
        //1、
        //NegaMax(searchDepth);
        //2、
        //AlphBeta(searchDepth, -20000, 20000);
        //3、
        //MergeSortAlphaBeta(searchDepth, -20000, 20000);
        //4、
        //AspirationSearch();
        //5、
        PrincipalVariation(searchDepth, -20000, 20000);

        HandleBestStep(position);

        return bestStep;
    }
    private void HandleBestStep(int[,] position)
    {
        GameObject item1 = gameManager.boardGrid[bestStep.from.x, bestStep.from.y];
        GameObject item2 = gameManager.boardGrid[bestStep.to.x, bestStep.to.y];
        bestStep.gridOne = item1;
        bestStep.gridTwo = item2;
        GameObject firstChess = item1.transform.GetChild(0).gameObject;
        bestStep.chessOne = firstChess;
        //移动
        bestStep.chessOneID = position[bestStep.from.x, bestStep.from.y];
        bestStep.chessTwoID = position[bestStep.to.x, bestStep.to.y];
        //吃子
        if (item2.transform.childCount != 0)
        {
            GameObject secondChess = item2.transform.GetChild(0).gameObject;
            bestStep.chessTwo = secondChess;
        }
    }

    #region アルゴリズム

    private int NegaMax(int depth)
    {
        int best = -20000;
        int score;
        int count;
        int willKillKing;
        int chessID;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }
        if (depth <= 0)
        {
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);

        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -NegaMax(depth - 1);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score > best)
            {
                best = score;
                if (depth == searchDepth)
                {
                    bestStep = gameManager.movingOfChess.moveList[depth, i];
                }
            }

        }
        return best;
    }

    private int AlphBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing, chessID;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }

        if (depth <= 0)
        {
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -AlphBeta(depth - 1, -beta, -alpha);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score >= beta)
            {
                return beta;
            }
            if (score > alpha)
            {
                alpha = score;
                if (depth == searchDepth)
                {
                    bestStep = gameManager.movingOfChess.moveList[depth, i];
                }
            }
        }
        return alpha;
    }

    private void AspirationSearch()
    {
        int X;
        int current;
        searchDepth = gameManager.currentLevel - 1;
        X = FalphaBeta(searchDepth, -20000, 20000);
        searchDepth = gameManager.currentLevel;
        current = FalphaBeta(searchDepth, X - 50, X + 50);
        if (current < X - 50)
        {
            FalphaBeta(searchDepth, -20000, X-50);
        }
        if (current>X+50)
        {
            FalphaBeta(searchDepth,X+50,20000);
        }
    }

    private int FalphaBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing, chessID;
        int current = -20000;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }

        if (depth <= 0)
        {
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -FalphaBeta(depth - 1, -beta, -alpha);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score>current)
            {
                current = score;
                if (score>=alpha)
                {
                    alpha = score;
                    if (depth==searchDepth)
                    {
                        bestStep = gameManager.movingOfChess.moveList[depth, i];
                    }
                }
                if (alpha>=beta)
                {
                    break;
                }
            }
        }
        return current;
    }

    private int PrincipalVariation(int depth,int alpha,int beta)
    {
        int score, count, willKillKing, chessID;
        int best;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing!=0)
        {
            return willKillKing;
        }
        if (depth<=0)
        {
            return Eveluate(unrealBoard,((searchDepth-depth)%2)!=0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard,depth, ((searchDepth - depth) % 2 )!= 0);
        chessID = MakeMove(gameManager.movingOfChess.moveList[depth, 0]);
        best = -PrincipalVariation(depth-1,-beta,-alpha);
        UnMakeMove(gameManager.movingOfChess.moveList[depth,0],chessID);
        if (depth==searchDepth)
        {
            bestStep = gameManager.movingOfChess.moveList[depth, 0];
        }
        for (int i = 0; i < count; i++)
        {
            if (best<beta)
            {
                if (best>alpha)
                {
                    alpha = best;
                }
                chessID = MakeMove(gameManager.movingOfChess.moveList[depth,i]);

                score = -PrincipalVariation(depth - 1, -alpha - 1, -alpha);
                 if (score>alpha&&score<beta)
                {
                    best = -PrincipalVariation(depth - 1, -beta, -score);
                    if (depth==searchDepth)
                    {
                        bestStep = gameManager.movingOfChess.moveList[depth, i];
                    }
                }
                else if(score>best)
                {
                    best = score;
                    if (depth == searchDepth)
                    {
                        bestStep = gameManager.movingOfChess.moveList[depth, i];
                    }
                }
                UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            }
        }
        return best;
    }

    private int MergeSortAlphaBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing, chessID;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }

        if (depth <= 0)
        {
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        MergeSort(gameManager.movingOfChess.moveList, count, depth);
        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -AlphBeta(depth - 1, -beta, -alpha);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score >= beta)
            {
                return beta;
            }
            if (score > alpha)
            {
                alpha = score;
                if (depth == searchDepth)
                {
                    bestStep = gameManager.movingOfChess.moveList[depth, i];
                    AddHistoryScore(bestStep, depth);
                }
            }
        }
        return alpha;
    }
    private Dictionary<ChessReseting.Chess, int> historyDic = new Dictionary<ChessReseting.Chess, int>();

    public void AddHistoryScore(ChessReseting.Chess move, int depth)
    {
        if (historyDic.TryGetValue(move, out int score))
        {
            historyDic[move] += 2 << depth;
        }
        else
        {
            historyDic.Add(move, 2 << depth);
        }
    }

    private int GetHistoryScore(ChessReseting.Chess move)
    {
        historyDic.TryGetValue(move, out int score);
        return score;
    }

    private void MergeSort(ChessReseting.Chess[,] move, int count, int depth)
    {
        ChessReseting.Chess[,] temp = new ChessReseting.Chess[8, 80];
        Sort(move, 0, count, temp, depth);
    }

    private void Sort(ChessReseting.Chess[,] move, int startIndex, int endIndex, ChessReseting.Chess[,] temp, int depth)
    {
        if (startIndex < endIndex)
        {
            int mid = (startIndex + endIndex) / 2;
            Sort(move, startIndex, mid, temp, depth);
            Sort(move, mid + 1, endIndex, temp, depth);
            Merge(move, startIndex, mid, endIndex, temp, depth);
        }
    }

    private void Merge(ChessReseting.Chess[,] move, int startIndex, int mid, int endIndex, ChessReseting.Chess[,] temp, int depth)
    {
        int i = startIndex;
        int j = mid + 1;
        int t = 0;
        while (i <= mid && j <= endIndex)
        {
            if (GetHistoryScore(move[depth, i]) > GetHistoryScore(move[depth, j]))
            {
                temp[depth, t] = move[depth, i];
                i++;
            }
            else
            {
                temp[depth, t] = move[depth, j];
                j++;
            }
            t++;
        }
        while (i <= mid)
        {
            temp[depth, t] = move[depth, i];
            i++;
            t++;
        }
        while (j <= endIndex)
        {
            temp[depth, t++] = move[depth, j++];
        }
        t = 0;
        while (startIndex <= endIndex)
        {
            move[depth, startIndex++] = temp[depth, t++];
        }
    }

    #endregion

    private int Eveluate(int[,] position, bool side)
    {
        int currentPosChessID;
        int targetPosChessID;
        chessValue = new int[10, 9];
        attackPos = new int[10, 9];
        guardPos = new int[10, 9];
        flexPos = new int[10, 9];

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    currentPosChessID = position[i, j];
                    GetRelatePos(position, i, j);
                    for (int k = 0; k < posCount; k++)
                    {
                        targetPosChessID = position[relatePos[k].x, relatePos[k].y];
                        if (targetPosChessID == 0)
                        {
                            flexPos[i, j]++;
                        }
                        else
                        {
                            if (gameManager.rules.IsSameSide(currentPosChessID, targetPosChessID))
                            {
                                guardPos[relatePos[k].x, relatePos[k].y]++;
                            }
                            else
                            {
                                attackPos[relatePos[k].x, relatePos[k].y]++;
                                flexPos[i, j]++;
                                switch (targetPosChessID)
                                {
                                    case 1:
                                        if (side)
                                        {
                                            return 18888;
                                        }
                                        break;
                                    case 8:
                                        if (!side)
                                        {
                                            return 18888;
                                        }
                                        break;
                                    default:
                                        attackPos[relatePos[k].x, relatePos[k].y] += ((baseValue[targetPosChessID] - baseValue[currentPosChessID]) / 10 + 30) / 10;
                                        break;
                                }
                            }
                        }
                    }

                }
            }
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    currentPosChessID = position[i, j];
                    chessValue[i, j]++;
                    chessValue[i, j] += flexValue[currentPosChessID] * flexPos[i, j];
                    chessValue[i, j] += GetBingValue(i, j, position);
                }
            }
        }

        int delta;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    currentPosChessID = position[i, j];
                    delta = baseValue[currentPosChessID] / 16;
                    chessValue[i, j] += baseValue[currentPosChessID];
                    //red
                    if (gameManager.rules.isRed(currentPosChessID))
                    {
                        if (attackPos[i, j] != 0)
                        {
                            if (side)
                            {
                                if (currentPosChessID == 8)
                                {
                                    chessValue[i, j] -= 20;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 2;
                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta;
                                    }
                                }
                            }
                            else
                            {
                                if (currentPosChessID == 8)
                                {
                                    return 18888;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 10;

                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta * 9;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (guardPos[i, j] != 0)
                            {
                                chessValue[i, j] += 5;
                            }
                        }
                    }
                    //black
                    else
                    {
                        if (attackPos[i, j] != 0)
                        {
                            if (side)
                            {
                                if (currentPosChessID == 1)
                                {
                                    chessValue[i, j] -= 20;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 2;
                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta;
                                    }
                                }
                            }
                            else
                            {
                                if (currentPosChessID == 1)
                                {
                                    return 18888;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 10;
                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta * 9;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (guardPos[i, j] != 0)
                            {
                                chessValue[i, j] += 5;
                            }
                        }
                    }
                }
            }
        }

        int redValue = 0, blackValue = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                currentPosChessID = position[i, j];
                if (currentPosChessID != 0)
                {
                    if (gameManager.rules.isRed(currentPosChessID))
                    {
                        redValue += chessValue[i, j];
                    }
                    else
                    {
                        blackValue += chessValue[i, j];
                    }
                }
            }
        }

        if (side)
        {
            return redValue - blackValue;
        }
        else
        {
            return blackValue - redValue;
        }

    }

    private int MakeMove(ChessReseting.Chess move)
    {
        int chessID = 0;
        chessID = unrealBoard[move.to.x, move.to.y];
        unrealBoard[move.to.x, move.to.y] = unrealBoard[move.from.x, move.from.y];
        unrealBoard[move.from.x, move.from.y] = 0;

        return chessID;
    }

    private void UnMakeMove(ChessReseting.Chess move, int chessID)
    {
        unrealBoard[move.from.x, move.from.y] = unrealBoard[move.to.x, move.to.y];
        unrealBoard[move.to.x, move.to.y] = chessID;
    }

    private int IsGameOver(int[,] position, int depth)
    {
        bool redAlive = false;
        bool blackAlive = false;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (position[i, j] == 1)
                {
                    blackAlive = true;
                }
            }
        }

        for (int i = 7; i < 10; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (position[i, j] == 8)
                {
                    redAlive = true;
                }
            }
        }
        int num = (searchDepth - depth + 1) % 2;
        if (!redAlive)
        {
            if (num != 0)
            {
                return 19990;
            }
            else
            {
                return -19990;
            }
        }
        if (!blackAlive)
        {
            if (num != 0)
            {
                return -19990;
            }
            else
            {
                return 19990;
            }
        }
        return 0;
    }

    private int GetRelatePos(int[,] position, int FromX, int FromY)
    {
        posCount = 0;
        int chessID;
        bool flag = false;
        int x, y;
        chessID = position[FromX, FromY];
        switch (chessID)
        {
            case 1://将
                for (x = 0; x < 3; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, FromX, FromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 8://帅
                for (x = 7; x < 10; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, FromX, FromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 2:
            case 9://車
                x = FromX;
                y = FromY + 1;
                while (y < 9)
                {
                    if (position[x, y] == 0)
                    {
                        AddPos(x, y);
                    }
                    else
                    {
                        AddPos(x, y);
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
                        AddPos(x, y);
                    }
                    else
                    {
                        AddPos(x, y);
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
                        AddPos(x, y);
                    }
                    else
                    {
                        AddPos(x, y);
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
                        AddPos(x, y);
                    }
                    else
                    {
                        AddPos(x, y);
                        break;
                    }
                    x--;
                }
                break;
            case 3:
            case 10://馬
                x = FromX + 2;
                y = FromY + 1;
                if ((x < 10 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX - 2;
                y = FromY + 1;
                if ((x >= 0 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX + 2;
                y = FromY - 1;
                if ((x < 10 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX - 2;
                y = FromY - 1;
                if ((x >= 0 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX + 1;
                y = FromY + 2;
                if ((x < 10 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX - 1;
                y = FromY + 2;
                if ((x >= 0 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX + 1;
                y = FromY - 2;
                if ((x < 10 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX - 1;
                y = FromY - 2;
                if ((x >= 0 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                break;
            case 4:
            case 11://炮
                x = FromX;
                y = FromY + 1;
                flag = false;
                while (y < 9)
                {
                    if (position[x, y] == 0)
                    {
                        if (!flag)
                        {
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
                            break;
                        }
                    }
                    x--;
                }
                break;
            case 5:
                for (x = 0; x < 3; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, FromX, FromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 12://士
                for (x = 7; x < 10; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, FromX, FromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 6:
            case 13://象
                x = FromX + 2;
                y = FromY + 2;
                if (x < 10 && y < 9 && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX - 2;
                y = FromY + 2;
                if (x >= 0 && y < 9 && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX + 2;
                y = FromY - 2;
                if (x < 10 && y >= 0 && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                x = FromX - 2;
                y = FromY - 2;
                if (x >= 0 && y >= 0 && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                break;
            case 7:
                x = FromX + 1;
                y = FromY;
                if (x < 10)
                {
                    AddPos(x, y);
                }
                if (FromX > 4)
                {
                    x = FromX;
                    y = FromY + 1;
                    if (y < 9)
                    {
                        AddPos(x, y);
                    }
                    y = FromY - 1;
                    if (y >= 10)
                    {
                        AddPos(x, y);
                    }
                }
                break;
            case 14://兵
                x = FromX - 1;
                y = FromY;
                if (x > 0)
                {
                    AddPos(x, y);
                }
                if (FromX < 5)
                {
                    x = FromX;
                    y = FromY + 1;
                    if (y < 9)
                    {
                        AddPos(x, y);
                    }
                    y = FromY - 1;
                    if (y >= 10)
                    {
                        AddPos(x, y);
                    }
                }
                break;
            default:
                break;
        }
        return posCount;
    }

    private void AddPos(int x, int y)
    {
        relatePos[posCount].x = x;
        relatePos[posCount].y = y;
        posCount++;
    }

    private bool CanReach(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        return gameManager.rules.IsVaild(position[FromX, FromY], position, FromX, FromY, ToX, ToY);
    }

    private int GetBingValue(int x, int y, int[,] position)
    {
        //兵
        if (position[x, y] == 14)
        {
            return b_bingValue[x, y];
        }
        //卒
        else if (position[x, y] == 7)
        {
            return r_bingValue[x, y];
        }
        return 0;
    }

}
