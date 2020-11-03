using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkmate
{
    private GameManager gameManager;
    private UiManager uiManager;

    private int jiangX, jiangY, shuaiX, shuaiY;

    public Checkmate()
    {
        gameManager = GameManager.Instance;
        uiManager = UiManager.Instance;
    }
    public void JudgeIfCheckmate()
    {
        GetKingPosition();
        if (gameManager.chessBoard[jiangX,jiangY]!=1)
        {
            uiManager.ShowTip("赤の勝利");
            AudioSourceManager.Instance.PlaySound(6);
            gameManager.gameOver = true;
            return;
        }
        else if (gameManager.chessBoard[shuaiX,shuaiY]!=8)
        {
            uiManager.ShowTip("黒の勝利");
            if (gameManager.chessPeople==1)
            {
                AudioSourceManager.Instance.PlaySound(7);
            }
            else if(gameManager.chessPeople==2)
            {
                AudioSourceManager.Instance.PlaySound(6);
            }
            gameManager.gameOver = true;
            return;
        }
        bool ifCheckmate;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                switch (gameManager.chessBoard[i,j])
                {
                    case 2:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard,i,j,shuaiX,shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 3:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 4:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 7:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 9:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 10:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 11:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    case 14:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("チェックメイト");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }

    private void GetKingPosition()
    {

        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (gameManager.chessBoard[i, j] ==1)
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
                if (gameManager.chessBoard[i,j]==8)
                {
                    shuaiX = i;
                    shuaiY = j;
                }
            }
        }
    }
}
