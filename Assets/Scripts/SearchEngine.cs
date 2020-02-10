using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取最优解的搜索引擎
/// </summary>
public class SearchEngine
{
    public int searchDepth;//搜索深度
    private GameManager gameManager;
    private ChessReseting.Chess bestStep;//最优解
    private int[,] unrealBoard = new int[10, 9];//虚拟棋盘


    public SearchEngine()
    {
        gameManager = GameManager.Instance;
    }

    /// <summary>
    /// 获取最优解的方法
    /// </summary>
    /// <returns></returns>
    public ChessReseting.Chess SearchGoodMove(int[,] position)
    {
        //设置搜索层级
        searchDepth = gameManager.currentLevel;
        //将当前棋盘状况记录进虚拟棋盘
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unrealBoard[i, j] = position[i, j];
            }
        }
        //调用具体搜索算法
        NegaMax(searchDepth);
        //AlphBeta(searchDepth,-20000,20000);
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

        return bestStep;
    }

    /// <summary>
    /// 负极大值算法
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int NegaMax(int depth)
    {
        //负无穷
        int best = -20000;
        //当前调用的得分
        int score;
        //当前局面下一步总共可以走的着法
        int count;
        //当前棋局下将帅是否阵亡
        int willKillKing;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            //棋局结束，将帅有阵亡
            return willKillKing;
        }
        //最底层叶的棋局得分
        if (depth <= 0)
        {
            return Eveluate();
        }
        count = gameManager.movingOfChess.CreatePossibleMove();

        for (int i = 0; i < count; i++)
        {
            //根据着法产生新局面
            MakeMove();
            //递归调用负极大值搜索函数搜索下一层节点
            score = -NegaMax(depth - 1);
            //恢复当前局面
            UnMakeMove();
            if (score > best)
            {
                best = score;
                if (depth == searchDepth)
                {
                    //搜索到达根部时保存最优解
                    bestStep = gameManager.movingOfChess.moveList[depth, i];
                }
            }

        }
        return best;
    }

    /// <summary>
    /// AlphBeta剪枝算法
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    private int AlphBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }

        if (depth <= 0)
        {
            return Eveluate();
        }
        count = gameManager.movingOfChess.CreatePossibleMove();
        for (int i = 0; i < count; i++)
        {
            MakeMove();
            score = -AlphBeta(depth - 1, -beta, -alpha);
            UnMakeMove();
            //发生剪枝
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

    private int Eveluate()
    {
        return 0;
    }

    public int MakeMove()
    {
        return 0;
    }

    public int UnMakeMove()
    {
        return 0;
    }

    /// <summary>
    /// 检查当前着法执行后棋盘中将帅是否存在（模拟）
    /// </summary>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
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
        //判断当前是奇数层或偶数层
        int num = (searchDepth - depth + 1) % 2;
        if (!redAlive)
        {
            if(num!=0)
            {
                //奇数层返回极大值(AI)
                return 19990;
            }
            else
            {
                //偶数层返回极小值(player)
                return -19990;
            }
        }
        if (!blackAlive)
        {
            if (num!=0)
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
}
