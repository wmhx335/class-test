using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules
{

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

    public bool IsVaild(int moveChessID, int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        if (FromX == ToX && FromY == ToY)
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
            case 1://将
                if (ToX > 2 || ToY > 5 || ToY < 3)
                {
                    return false;
                }
                if ((Mathf.Abs(ToX - FromX) + Mathf.Abs(ToY - FromY)) > 1)
                {
                    return false;
                }
                break;
            case 8://帅
                if (ToX < 7 || ToY > 5 || ToY < 3)
                {
                    return false;
                }
                if ((Mathf.Abs(ToX - FromX) + Mathf.Abs(ToY - FromY)) > 1)
                {
                    return false;
                }
                break;
            case 5://士
                if (Mathf.Abs(FromX - ToX) != 1 || Mathf.Abs(FromY - ToY) != 1)
                {
                    return false;
                }
                break;
            case 12://仕
                if (Mathf.Abs(FromX - ToX) != 1 || Mathf.Abs(FromY - ToY) != 1)
                {
                    return false;
                }
                break;
            case 6://象
                if (Mathf.Abs(FromX - ToX) != 2 || Mathf.Abs(FromY - ToY) != 2)
                {
                    return false;
                }
                if (position[(FromX + ToX) / 2, (FromY + ToY) / 2] != 0)
                {
                    return false;
                }
                break;
            case 13://相
                if (Mathf.Abs(FromX - ToX) != 2 || Mathf.Abs(FromY - ToY) != 2)
                {
                    return false;
                }
                //塞象眼
                if (position[(FromX + ToX) / 2, (FromY + ToY) / 2] != 0)
                {
                    return false;
                }
                break;
            case 7://卒
                if (ToX < FromX)
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
            case 14://兵
                if (ToX > FromX)
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

            case 2:
            case 9://車
                if (FromX != ToX && FromY != ToY)
                {
                    return false;
                }
                if (FromX == ToX)
                {
                    if (FromY < ToY)
                    {
                        for (i = FromY + 1; i < ToY; i++)
                        {
                            if (position[FromX, i] != 0)
                            {
                                return false;
                            }
                        }
                    }
                    else
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
                    if (FromX < ToX)
                    {
                        for (j = FromX + 1; j < ToX; j++)
                        {
                            if (position[j, FromY] != 0)
                            {
                                return false;
                            }
                        }
                    }
                    else
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
            case 10://馬
                if (!((Mathf.Abs(ToY - FromY) == 1 && Mathf.Abs(ToX - FromX) == 2) ||
                    (Mathf.Abs(ToY - FromY) == 2 && Mathf.Abs(ToX - FromX) == 1)))
                {
                    return false;
                }
                if (ToY - FromY == 2)
                {
                    i = FromY + 1;
                    j = FromX;
                }
                else if (FromY - ToY == 2)
                {
                    i = FromY - 1;
                    j = FromX;
                }
                else if (ToX - FromX == 2)
                {
                    i = FromY;
                    j = FromX + 1;
                }
                else if (FromX - ToX == 2)
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
            case 11://炮
                if (FromY != ToY && FromX != ToX)
                {
                    return false;
                }
                if (position[ToX, ToY] == 0)
                {

                    if (FromX == ToX)
                    {
                        if (FromY < ToY)
                        {
                            for (i = FromY + 1; i < ToY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                        else
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
                        if (FromX < ToX)
                        {
                            for (j = FromX + 1; j < ToX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                        else
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
                else
                {
                    int count = 0;
                    if (FromX == ToX)
                    {

                        if (FromY < ToY)
                        {
                            for (i = FromY+1; i < ToY; i++)
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
                        else
                        {
                            for (i = ToY+1; i < FromY; i++)
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
                    else
                    {
                        if (FromX < ToX)
                        {
                            for (j = FromX + 1; j < ToX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    count++;
                                }
                                
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            for (j = ToX + 1; j < FromX; j++)
                            {
                                if (position[j, FromY] != 0)
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
                }
                break;
            default:
                break;
        }

        return true;

    }

    public bool KingKill(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        int jiangX = 0, jiangY = 0, shuaiX = 0, shuaiY = 0;
        int count = 0;
        int[,] position1 = new int[10, 9];
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
        if (count == 0)
        {
            return false;
        }
        return true;

    }


}
