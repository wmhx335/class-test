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
    //记录棋子相关位置的数组
    private ChessReseting.ChessSteps[] relatePos = new ChessReseting.ChessSteps[30];
    //记录棋子相关位置的个数
    private int posCount;

    public SearchEngine()
    {
        gameManager = GameManager.Instance;
    }

    //棋子的子力价值表
    private int[] baseValue = new int[15]
    {
        //将 车 马 炮 士 象 兵
        0,10000,900,400,450,200,200,100,
          10000,900,400,450,200,200,100
    };
    //棋子的灵活值表
    private int[] flexValue = new int[15]
    {
     //将 车 马 炮 士 象 兵
        0, 0, 6, 12, 6, 1, 1, 1,
           0, 6, 12, 6, 1, 1, 1
    };
    //每个位置威胁信息（危！）
    private int[,] attackPos;
    //每个位置的保护值
    private int[,] guardPos;
    //每个位置的灵活值
    private int[,] flexPos;
    //存放每个位置的棋子总价值（根据上述公式计算得出总分，
    //后续所有棋子价值总得分用来估算整个局势得分，即评估函数返回值）
    private int[,] chessValue;

    //红兵攻击位置附加值
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
    //黑卒攻击位置附加值
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
        //当前棋局下移动的棋子对应ID
        int chessID;
        if (willKillKing != 0)
        {
            //棋局结束，将帅有阵亡
            return willKillKing;
        }
        //最底层叶的棋局得分
        if (depth <= 0)
        {
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);

        for (int i = 0; i < count; i++)
        {
            //根据着法产生新局面
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            //递归调用负极大值搜索函数搜索下一层节点
            score = -NegaMax(depth - 1);
            //恢复当前局面
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
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

    /// <summary>
    /// 评估函数
    /// </summary>
    /// <param name="position"></param>
    /// <param name="side">当前是哪方视角,false是偶数AI层黑方</param>
    /// <returns></returns>
    private int Eveluate(int[,] position, bool side)
    {
        //当前位置棋子ID
        int currentPosChessID;
        //目标位置棋子ID
        int targetPosChessID;
        chessValue = new int[10, 9];
        attackPos = new int[10, 9];
        guardPos = new int[10, 9];
        flexPos = new int[10, 9];

        #region 第一次扫描，找出所有棋子相关位置并赋值得分（对棋子相关位置的处理）
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                //扫描到该位置是棋子时
                if (position[i, j] != 0)
                {
                    //取得当前位置的棋子ID
                    currentPosChessID = position[i, j];
                    //找出该棋子的相关位置
                    GetRelatePos(position, i, j);
                    //对每个位置进行后续处理
                    for (int k = 0; k < posCount; k++)
                    {
                        //获取每个相关位置的棋子ID
                        targetPosChessID = position[relatePos[k].x, relatePos[k].y];
                        //相关位置是否为空格子
                        if (targetPosChessID == 0)
                        {
                            //空格子，灵活值增加
                            flexPos[i, j]++;
                        }
                        else
                        {
                            //己方棋子 保护值增加
                            if (gameManager.rules.IsSameSide(currentPosChessID, targetPosChessID))
                            {
                                guardPos[relatePos[k].x, relatePos[k].y]++;
                            }
                            //敌方棋子 威胁值增加，且灵活值增加
                            else
                            {
                                attackPos[relatePos[k].x, relatePos[k].y]++;
                                flexPos[i, j]++;
                                switch (targetPosChessID)
                                {
                                    case 1:
                                        if (side)
                                        {
                                            //红方轮次返回极大值
                                            return 18888;
                                        }
                                        break;
                                    case 8:
                                        if (!side)
                                        {
                                            //黑方轮次返回极大值
                                            return 18888;
                                        }
                                        break;
                                    //其他棋子
                                    default:
                                        //棋子相关位置威胁值增加
                                        attackPos[relatePos[k].x, relatePos[k].y] += ((baseValue[targetPosChessID] - baseValue[currentPosChessID]) / 10 + 30) / 10;
                                        break;
                                }
                            }
                        }
                    }

                }
            }
        }
        #endregion 

        #region 第二次扫描,对棋盘上每个棋子的自身基础值做处理
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    //取得当前位置的棋子ID
                    currentPosChessID = position[i, j];
                    //如果是棋子，则该位置价值增加
                    chessValue[i, j]++;
                    //把每个棋子的灵活性价值加进基础值
                    chessValue[i, j] += flexValue[currentPosChessID] * flexPos[i, j];
                    //如果是兵，则基础值加上兵的位置附加值
                    chessValue[i, j] += GetBingValue(i, j, position);
                }
            }
        }
        #endregion

        #region 第三次扫描 计算当前棋子所在位置的基础值总分
        int delta;//威胁保护增量
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    //取得当前位置的棋子ID
                    currentPosChessID = position[i, j];
                    //棋子子力价值的1/16作为威胁/保护增量
                    delta = baseValue[currentPosChessID] / 16;
                    //基础值加上每个棋子的子力价值
                    chessValue[i, j] += baseValue[currentPosChessID];
                    //红方
                    if (gameManager.rules.isRed(currentPosChessID))
                    {
                        //红棋被威胁
                        if (attackPos[i, j] != 0)
                        {
                            //红方轮次
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
                            //黑方轮次
                            else
                            {
                                if (currentPosChessID == 8)
                                {
                                    return 18888;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 10;
                                    //如果受到保护

                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta * 9;
                                    }
                                }
                            }
                        }
                        //未受到威胁
                        else
                        {
                            //该位置受到保护，保护值增加
                            if (guardPos[i, j] != 0)
                            {
                                chessValue[i, j] += 5;
                            }
                        }
                    }
                    //黑方
                    else
                    {
                        //黑棋被威胁
                        if (attackPos[i, j] != 0)
                        {
                            //黑方轮次
                            if (side)
                            {
                                //黑将
                                if (currentPosChessID == 1)
                                {
                                    //棋子价值降低20
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
                            //红方轮次
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
                        //未受到威胁
                        else
                        {
                            //该位置受到保护，保护值增加
                            if (guardPos[i, j] != 0)
                            {
                                chessValue[i, j] += 5;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 第四次扫描 计算红方与黑方的总得分，返回评估值
        int redValue=0, blackValue=0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                currentPosChessID = position[i, j];
                if (currentPosChessID!=0)
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
        #endregion
      
    }

    /// <summary>
    /// 根据传入的走法改变棋盘
    /// </summary>
    /// <param name="move">具体的着法</param>
    /// <returns></returns>
    private int MakeMove(ChessReseting.Chess move)
    {
        int chessID = 0;
        //取到目标棋子
        chessID = unrealBoard[move.to.x, move.to.y];
        //把棋子移动到目标位置
        unrealBoard[move.to.x, move.to.y] = unrealBoard[move.from.x, move.from.y];
        //把原位置清空
        unrealBoard[move.from.x, move.from.y] = 0;

        return chessID;
    }

    /// <summary>
    /// 还原之前走的着法
    /// </summary>
    /// <param name="move">之前的着法</param>
    /// <param name="chessID">所走棋子的ID</param>
    private void UnMakeMove(ChessReseting.Chess move, int chessID)
    {
        //将原来位置的棋子ID还原
        unrealBoard[move.from.x, move.from.y] = unrealBoard[move.to.x, move.to.y];
        //恢复目标位置的棋子
        unrealBoard[move.to.x, move.to.y] = chessID;
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
            if (num != 0)
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

    /// <summary>
    /// 取指定棋子的相关位置
    /// </summary>
    /// <param name="position"></param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <returns></returns>
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
                //右
                x = FromX;
                y = FromY + 1;
                while (y < 9)
                {
                    //判断空格子
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
                //左
                x = FromX;
                y = FromY - 1;
                while (y >= 0)
                {
                    //判断空格子
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
                //下
                x = FromX + 1;
                y = FromY;
                while (x < 10)
                {
                    //判断空格子
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
                //上
                x = FromX - 1;
                y = FromY;
                while (x >= 0)
                {
                    //判断空格子
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
            case 10://马
                x = FromX + 2;
                y = FromY + 1;
                if ((x < 10 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //右上
                x = FromX - 2;
                y = FromY + 1;
                if ((x >= 0 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //左下
                x = FromX + 2;
                y = FromY - 1;
                if ((x < 10 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //左上
                x = FromX - 2;
                y = FromY - 1;
                if ((x >= 0 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //横
                //右下
                x = FromX + 1;
                y = FromY + 2;
                if ((x < 10 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //右上
                x = FromX - 1;
                y = FromY + 2;
                if ((x >= 0 && y < 9) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //左下
                x = FromX + 1;
                y = FromY - 2;
                if ((x < 10 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //左上
                x = FromX - 1;
                y = FromY - 2;
                if ((x >= 0 && y >= 0) && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                break;
            case 4:
            case 11://炮
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
                            AddPos(x, y);
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
                        else
                        {
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                            AddPos(x, y);
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
                //右上
                x = FromX - 2;
                y = FromY + 2;
                if (x >= 0 && y < 9 && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //左下
                x = FromX + 2;
                y = FromY - 2;
                if (x < 10 && y >= 0 && CanReach(position, FromX, FromY, x, y))
                {
                    AddPos(x, y);
                }
                //左上
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
                    y = FromY + 1;//右
                    if (y < 9)
                    {
                        AddPos(x, y);
                    }
                    y = FromY - 1;//左
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
                //过河后
                if (FromX < 5)
                {
                    x = FromX;
                    y = FromY + 1;//右
                    if (y < 9)
                    {
                        AddPos(x, y);
                    }
                    y = FromY - 1;//左
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

    /// <summary>
    /// 把当前传递进来的一个相关位置信息记录进相关位置数组里
    /// </summary>
    /// <returns></returns>
    private void AddPos(int x, int y)
    {
        relatePos[posCount].x = x;
        relatePos[posCount].y = y;
        posCount++;
    }

    /// <summary>
    /// 当前这个相关位置针对指定棋子是否可以到达
    /// </summary>
    /// <returns></returns>
    private bool CanReach(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        return gameManager.rules.IsVaild(position[FromX, FromY], position, FromX, FromY, ToX, ToY);
    }

    /// <summary>
    /// 小兵位置附加值计算方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private int GetBingValue(int x, int y, int[,] position)
    {
        //红兵
        if (position[x, y] == 14)
        {
            return b_bingValue[x, y];
        }
        //黑卒
        else if (position[x, y] == 7)
        {
            return r_bingValue[x, y];
        }
        return 0;
    }

}
