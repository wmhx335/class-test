using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessReseting 
{

    public GameManager gameManager;

    public ChessReseting()
    {
        gameManager=GameManager.Instance;
    }

    public struct Chess
    {
        //public 
    }

    /// <summary>
    /// 棋子位置
    /// </summary>
    public struct ChessSteps
    {
        public int x,y;
    }

}
