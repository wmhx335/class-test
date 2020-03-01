using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessReseting 
{

    public GameManager gameManager;

    //计数器 走了几步棋
    public int resetCount;
    //悔棋数组，用来存放所有已经走过的步数
    public Chess[] chessSteps;

    public ChessReseting()
    {
        gameManager=GameManager.Instance;
    }

    /// <summary>
    /// 记录每一步棋的具体内容的结构体
    /// </summary>
    public struct Chess
    {
        public ChessSteps from;
        public ChessSteps to;
        public GameObject gridOne;//来的位置所在格子
        public GameObject gridTwo;//去的位置所在格子
        public GameObject chessOne;
        public GameObject chessTwo;
        public int chessOneID;
        public int chessTwoID;
    }

    /// <summary>
    /// 棋子位置
    /// </summary>
    public struct ChessSteps
    {
        public int x,y;
    }

    /// <summary>
    /// 悔棋方法
    /// </summary>
    public void ResetChess()
    {
        gameManager.HideLastPositionUI();
        gameManager.HideClickUI();
        gameManager.HideCanEatUI();
        //pve悔棋两步
        if (gameManager.chessPeople==1)
        {
            if (resetCount<=1)
            {
                return;
            }
            int f = resetCount - 1;
            int s = resetCount - 2;
            //获取黑棋来去的位置
            int oneID = chessSteps[f].chessOneID;
            int twoID = chessSteps[f].chessTwoID;
            //获取红棋来去的位置
            int threeID = chessSteps[s].chessOneID;
            int fourID = chessSteps[s].chessTwoID;
            //悔棋第一步 黑方
            GameObject b_gridOne, b_gridTwo, b_chessOne, b_chessTwo;
            b_gridOne = chessSteps[f].gridOne;
            b_gridTwo = chessSteps[f].gridTwo;
            b_chessOne = chessSteps[f].chessOne;
            b_chessTwo = chessSteps[f].chessTwo;
            //悔棋第二步 红方
            GameObject r_gridOne, r_gridTwo, r_chessOne, r_chessTwo;
            r_gridOne = chessSteps[s].gridOne;
            r_gridTwo = chessSteps[s].gridTwo;
            r_chessOne = chessSteps[s].chessOne;
            r_chessTwo = chessSteps[s].chessTwo;
            //黑方吃子
            if (b_chessTwo!=null)
            {
                b_chessOne.transform.SetParent(b_gridOne.transform);
                b_chessTwo.transform.SetParent(b_gridTwo.transform);
                b_chessOne.transform.localPosition = Vector3.zero;
                b_chessTwo.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = twoID;

                //红方吃子
                if (r_chessTwo!=null)
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessTwo.transform.SetParent(r_gridTwo.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    r_chessTwo.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = fourID;

                }
                //红方走子
                else
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = 0;
                }
            }
            //黑方走子
            else
            {
                b_chessOne.transform.SetParent(b_gridOne.transform);
                b_chessOne.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = 0;

                //红方吃子
                if (r_chessTwo != null)
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessTwo.transform.SetParent(r_gridTwo.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    r_chessTwo.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = fourID;

                }
                //红方走子
                else
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = 0;
                }
            }
            gameManager.chessMove = true;
            resetCount -= 2;
            gameManager.gameOver = false;
            UiManager.Instance.ShowTip("赤の番");
            gameManager.checkmate.JudgeIfCheckmate();
            chessSteps[f] = new Chess();
            chessSteps[s] = new Chess();
        }
        //pvp悔棋一步
        else if(gameManager.chessPeople==2||gameManager.chessPeople==3)
        {
            if (resetCount <= 0)
            {
                return;
            }
            int f = resetCount - 1;//索引从0开始
            int oneID = chessSteps[f].chessOneID;//棋子原来位置的ID
            int twoID = chessSteps[f].chessTwoID;//棋子移动到的位置的ID
            GameObject gridOne, gridTwo, chessOne, chessTwo;
            gridOne = chessSteps[f].gridOne;
            gridTwo = chessSteps[f].gridTwo;
            chessOne = chessSteps[f].chessOne;
            chessTwo = chessSteps[f].chessTwo;
            //吃子
            if (chessTwo!=null)
            {
                chessOne.transform.SetParent(gridOne.transform);
                chessTwo.transform.SetParent(gridTwo.transform);
                chessOne.transform.localPosition = Vector3.zero;
                chessTwo.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = twoID;

            }
            //移动
            else
            {
                chessOne.transform.SetParent(gridOne.transform);
                chessOne.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y]=oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = 0;
            }
            //黑色轮次，红色悔棋
            if (gameManager.chessMove==false)
            {
                UiManager.Instance.ShowTip("赤の番");
            }
            //红色轮次，黑色悔棋
            else
            {
                UiManager.Instance.ShowTip("黒の番");
            }
            resetCount -= 1;
            chessSteps[f] = new Chess();
        }
    }

    /// <summary>
    /// 添加悔棋步数
    /// </summary>
    /// <param name="resetStepNum">具体悔棋步数索引</param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="ToX"></param>
    /// <param name="ToY"></param>
    /// <param name="ID1">对应悔棋的那一步的第一个棋子ID</param>
    /// <param name="ID2">对应悔棋的那一步的第二个棋子或格子ID</param>
    public void AddChess(int resetStepNum,int FromX,int FromY,int ToX,int ToY,int ID1,int ID2)
    {
        //当前需要记录的这步棋的数据存入chess结构体
        //然后存进结构体数组
        GameObject item1 = gameManager.boardGrid[FromX, FromY];
        GameObject item2 = gameManager.boardGrid[ToX, ToY];
        chessSteps[resetStepNum].from.x = FromX;
        chessSteps[resetStepNum].from.y = FromY;
        chessSteps[resetStepNum].to.x = ToX;
        chessSteps[resetStepNum].to.y = ToY;
        chessSteps[resetStepNum].gridOne = item1;
        chessSteps[resetStepNum].gridTwo = item2;
        gameManager.HideCanEatUI();
        gameManager.HideClickUI();
        GameObject firstChess = item1.transform.GetChild(0).gameObject;
        chessSteps[resetStepNum].chessOne = firstChess;
        chessSteps[resetStepNum].chessOneID = ID1;
        chessSteps[resetStepNum].chessTwoID = ID2;
        //如果是吃子
        if (item2.transform.childCount!=0)
        {
            GameObject secondChess = item2.transform.GetChild(0).gameObject;
            chessSteps[resetStepNum].chessTwo = secondChess;
        }
        resetCount++;
    }
}
