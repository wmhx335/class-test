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
    public bool IsValidMove(int [,] position,int FromX,int FromY,int ToX,int ToY)
    {
        int moveChessId, targetID;
        moveChessId = position[FromX, FromY];
        targetID = position[ToX, ToY];
        if(true)
        {

        }

        return true;
    }
}
