using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 规则
/// </summary>
public class Rules
{

    /// <summary>
    /// 检查当前移动是符合法
    /// </summary>
    /// <param name="position">当前棋盘状况</param>
    /// <param name="FromX">起始x索引</param>
    /// <param name="FromY">起始y索引</param>
    /// <param name="ToX">终点x索引</param>
    /// <param name="ToY">终点y索引</param>
    /// <returns></returns>
    public bool IsValidMove(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        int moveChessID, targetID;
        moveChessID = position[FromX, FromY];
        targetID = position[ToX, ToY];
        if (IsSameSide(moveChessID, targetID))
        {
            return false;
        }

        return IsVaild(moveChessID, position, FromX, FromY, ToX, ToY);
    }

    /// <summary>
    /// 判断选中的两个目标是否同为空格或同为一色棋子
    /// </summary>
    /// <returns></returns>
    public bool IsSameSide(int x, int y)
    {
        if (isBlack(x) && isBlack(y) || isRed(x) && isRed(y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isBlack(int x)
    {
        if (x > 0 && x < 8)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isRed(int x)
    {
        if (x >= 8 && x < 15)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 所有种类棋子的走法是否合法
    /// </summary>
    /// <param name="moveChessID"></param>
    /// <param name="postion"></param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="ToX"></param>
    /// <param name="ToY"></param>
    public bool IsVaild(int moveChessID, int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        if (FromX == ToX && FromY == ToY)//原地踏步
        {
            return false;
        }
        if (!KingKill(position, FromX, FromY, ToX, ToY))
        {
            return false;
        }

        int i = 0, j = 0;
        switch (moveChessID)
        {
            //红黑棋子分别处理
            case 1://黑将
                if (ToX > 2 || ToY > 5 || ToY < 3)//出九宫格
                {
                    return false;
                }
                if ((Mathf.Abs(ToX - FromX) + Mathf.Abs(ToY - FromY)) > 1)
                {
                    return false;
                }
                break;
            case 8://红帅
                if (ToX < 7 || ToY > 5 || ToY < 3)//出九宫格
                {
                    return false;
                }
                if ((Mathf.Abs(ToX - FromX) + Mathf.Abs(ToY - FromY)) > 1)
                {
                    return false;
                }
                break;
            case 5://黑士
                if (ToX > 2 || ToY > 5 || ToY < 3)//出九宫格
                {
                    return false;
                }
                if (Mathf.Abs(FromX - ToX) != 1 || Mathf.Abs(FromY - ToY) != 1)//走斜线
                {
                    return false;
                }
                break;
            case 12://红仕
                if (ToX < 7 || ToY > 5 || ToY < 3)//出九宫格
                {
                    return false;
                }
                if (Mathf.Abs(FromX - ToX) != 1 || Mathf.Abs(FromY - ToY) != 1)
                {
                    return false;
                }
                break;
            case 6://黑象
                if (ToX > 4)//过河
                {
                    return false;
                }
                if (Mathf.Abs(FromX - ToX) != 2 || Mathf.Abs(FromY - ToY) != 2)//走田字
                {
                    return false;
                }
                //塞象眼
                if (position[(FromX + ToX) / 2, (FromY + ToY) / 2] != 0)
                {
                    return false;
                }
                break;
            case 13://红相
                if (ToX < 5)//过河
                {
                    return false;
                }
                if (Mathf.Abs(FromX - ToX) != 2 || Mathf.Abs(FromY - ToY) != 2)//走田字
                {
                    return false;
                }
                //塞象眼
                if (position[(FromX + ToX) / 2, (FromY + ToY) / 2] != 0)
                {
                    return false;
                }
                break;
            case 7://黑卒
                if (ToX < FromX)//永不后退！
                {
                    return false;
                }
                if (FromX < 5 && FromX == ToX)
                {
                    return false;
                }
                if (ToX - FromX + Mathf.Abs(ToY - FromY) > 1)
                {
                    return false;
                }
                break;
            case 14://红兵
                if (ToX > FromX)//永不后退！
                {
                    return false;
                }
                if (FromX > 4 && FromX == ToX)
                {
                    return false;
                }
                if (FromX - ToX + Mathf.Abs(ToY - FromY) > 1)
                {
                    return false;
                }
                break;
            //红黑棋子共用
            case 2:
            case 9://红黑車
                if (FromX != ToX && FromY != ToY)//直走
                {
                    return false;
                }
                //判断当前移动路径上是否有其他棋子
                if (FromX == ToX)
                {
                    if (FromY < ToY)//右走
                    {
                        for (i = FromY + 1; i < ToY; i++)
                        {
                            if (position[FromX, i] != 0)//移动路径上有其他棋子
                            {
                                return false;
                            }
                        }
                    }
                    else//左走
                    {
                        for (i = ToY + 1; i < FromY; i++)
                        {
                            if (position[FromX, i] != 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (FromX < ToX)//下走
                    {
                        for (j = FromX + 1; j < ToX; j++)
                        {
                            if (position[j, FromY] != 0)
                            {
                                return false;
                            }
                        }
                    }
                    else//上走
                    {
                        for (j = ToX + 1; j < FromX; j++)
                        {
                            if (position[j, FromY] != 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                break;
            case 3:
            case 10://红黑马
                //马走日
                //竖日
                if (!((Mathf.Abs(ToY - FromY) == 1 && Mathf.Abs(ToX - FromX) == 2) ||
                    //横日
                    (Mathf.Abs(ToY - FromY) == 2 && Mathf.Abs(ToX - FromX) == 1)))
                {
                    return false;
                }
                //马蹩腿
                if (ToY - FromY == 2)//右日
                {
                    i = FromY + 1;
                    j = FromX;
                }
                else if (FromY - ToY == 2)//左日
                {
                    i = FromY - 1;
                    j = FromX;
                }
                else if (ToX - FromX == 2)//下日
                {
                    i = FromY;
                    j = FromX + 1;
                }
                else if (FromX - ToX == 2)//上日
                {
                    i = FromY;
                    j = FromX - 1;
                }
                if (position[j, i] != 0)
                {
                    return false;
                }
                break;
            case 4:
            case 11://红黑炮
                //走直线
                if (FromY != ToY && FromX != ToX)
                {
                    return false;
                }
                //移动或吃子
                //移动
                if (position[ToX, ToY] == 0)
                {
                    //横线
                    if (FromX == ToX)
                    {
                        if (FromY < ToY)//右走
                        {
                            for (i = FromY + 1; i < ToY; i++)
                            {
                                if (position[FromX, i] != 0)//移动路径上有其他棋子
                                {
                                    return false;
                                }
                            }
                        }
                        else//左走
                        {
                            for (i = ToY + 1; i < FromY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    //竖线
                    else
                    {
                        if (FromX < ToX)//下走
                        {
                            for (j = FromX + 1; j < ToX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                        else//上走
                        {
                            for (j = ToX + 1; j < FromX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                //吃子
                else
                {
                    int count = 0;
                    //横线
                    if (FromX == ToX)
                    {
                        //右
                        if (FromY < ToY)
                        {
                            for (i = FromY; i < ToY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    count++;
                                }
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                        //左
                        else
                        {
                            for (i = ToY; i < FromY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    count++;
                                }
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                    }
                    //竖线
                    else
                    {
                        //下
                        if (FromX < ToX)
                        {
                            for (j = FromX + 1; j < ToX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    count++;
                                }
                                if (count != 1)
                                {
                                    return false;
                                }
                            }
                        }
                        //上
                        else
                        {
                            for (j = ToX + 1; j < FromX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    count++;
                                }
                                if (count != 1)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }

        return true;

    }

    /// <summary>
    /// 判断将帅是否在同一直线上
    /// </summary>
    /// <param name="position"></param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="ToX"></param>
    /// <param name="ToY"></param>
    /// <returns></returns>
    public bool KingKill(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        int jiangX = 0, jiangY = 0, shuaiX = 0, shuaiY = 0;
        int count = 0;
        int[,] position1 = new int[10, 9];//假设一个虚拟棋盘测试将帅是否即将在同一直线上
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                position1[i, j] = position[i, j];
            }
        }
        position1[ToX, ToY] = position1[FromX, FromY];
        position1[FromX, FromY] = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (position1[i, j] == 1)
                {
                    jiangX = i;
                    jiangY = j;
                }
            }
        }
        for (int i = 7; i < 10; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (position1[i, j] == 8)
                {
                    shuaiX = i;
                    shuaiY = j;
                }
            }
        }
        if (jiangY == shuaiY)
        {
            for (int i = jiangX + 1; i < shuaiX; i++)
            {
                if (position1[i, jiangY] != 0)
                {
                    count++;
                }
            }
        }
        else
        {
            count = -1;
        }
        if (count == 0)//不合法
        {
            return false;
        }
        return true;

    }


}
