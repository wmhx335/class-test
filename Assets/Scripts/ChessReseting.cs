using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessReseting 
{

    public GameManager gameManager;
    public int resetCount;
    public Chess[] chessSteps;

    public ChessReseting()
    {
        gameManager=GameManager.Instance;
    }

    public struct Chess
    {
        public ChessSteps from;
        public ChessSteps to;
        public GameObject gridOne;
        public GameObject gridTwo;
        public GameObject chessOne;
        public GameObject chessTwo;
        public int chessOneID;
        public int chessTwoID;
    }
    public struct ChessSteps
    {
        public int x,y;
    }
    public void ResetChess()
    {
        gameManager.HideLastPositionUI();
        gameManager.HideClickUI();
        gameManager.HideCanEatUI();
        //pve
        if (gameManager.chessPeople==1)
        {
            if (resetCount<=1)
            {
                return;
            }
            int f = resetCount - 1;
            int s = resetCount - 2;
            //black position
            int oneID = chessSteps[f].chessOneID;
            int twoID = chessSteps[f].chessTwoID;
            //red position
            int threeID = chessSteps[s].chessOneID;
            int fourID = chessSteps[s].chessTwoID;
            //black
            GameObject b_gridOne, b_gridTwo, b_chessOne, b_chessTwo;
            b_gridOne = chessSteps[f].gridOne;
            b_gridTwo = chessSteps[f].gridTwo;
            b_chessOne = chessSteps[f].chessOne;
            b_chessTwo = chessSteps[f].chessTwo;
            //red
            GameObject r_gridOne, r_gridTwo, r_chessOne, r_chessTwo;
            r_gridOne = chessSteps[s].gridOne;
            r_gridTwo = chessSteps[s].gridTwo;
            r_chessOne = chessSteps[s].chessOne;
            r_chessTwo = chessSteps[s].chessTwo;
            if (b_chessTwo!=null)
            {
                b_chessOne.transform.SetParent(b_gridOne.transform);
                b_chessTwo.transform.SetParent(b_gridTwo.transform);
                b_chessOne.transform.localPosition = Vector3.zero;
                b_chessTwo.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = twoID;

                if (r_chessTwo!=null)
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessTwo.transform.SetParent(r_gridTwo.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    r_chessTwo.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = fourID;

                }
                else
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = 0;
                }
            }
            else
            {
                b_chessOne.transform.SetParent(b_gridOne.transform);
                b_chessOne.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = 0;

                if (r_chessTwo != null)
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessTwo.transform.SetParent(r_gridTwo.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    r_chessTwo.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = fourID;

                }
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
        //pvp
        else if(gameManager.chessPeople==2||gameManager.chessPeople==3)
        {
            if (resetCount <= 0)
            {
                return;
            }
            int f = resetCount - 1;
            int oneID = chessSteps[f].chessOneID;
            int twoID = chessSteps[f].chessTwoID;
            GameObject gridOne, gridTwo, chessOne, chessTwo;
            gridOne = chessSteps[f].gridOne;
            gridTwo = chessSteps[f].gridTwo;
            chessOne = chessSteps[f].chessOne;
            chessTwo = chessSteps[f].chessTwo;
            if (chessTwo!=null)
            {
                chessOne.transform.SetParent(gridOne.transform);
                chessTwo.transform.SetParent(gridTwo.transform);
                chessOne.transform.localPosition = Vector3.zero;
                chessTwo.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = twoID;

            }
            else
            {
                chessOne.transform.SetParent(gridOne.transform);
                chessOne.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y]=oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = 0;
            }
            //blackTurn
            if (gameManager.chessMove==false)
            {
                UiManager.Instance.ShowTip("赤の番");
            }
            //redTurn
            else
            {
                UiManager.Instance.ShowTip("黒の番");
            }
            resetCount -= 1;
            chessSteps[f] = new Chess();
        }
    }

    public void AddChess(int resetStepNum,int FromX,int FromY,int ToX,int ToY,int ID1,int ID2)
    {
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
        if (item2.transform.childCount!=0)
        {
            GameObject secondChess = item2.transform.GetChild(0).gameObject;
            chessSteps[resetStepNum].chessTwo = secondChess;
        }
        resetCount++;
    }
}
