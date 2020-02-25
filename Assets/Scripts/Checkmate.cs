using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 检测是否将军
/// </summary>
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
        //将不存在
        if (gameManager.chessBoard[jiangX,jiangY]!=1)
        {
            uiManager.ShowTip("红方胜利");
            AudioSourceManager.Instance.PlaySound(6);
            if (gameManager.chessPeople==3)
            {
                gameManager.gameCodeReceive[0] = 1;
                UiManager.Instance.CanClickStartButton(true);
            }
            gameManager.gameOver = true;
            return;
        }
        //帅不存在
        else if (gameManager.chessBoard[shuaiX,shuaiY]!=8)
        {
            uiManager.ShowTip("黑方胜利");
            if (gameManager.chessPeople==1)
            {
                AudioSourceManager.Instance.PlaySound(7);
            }
            else if(gameManager.chessPeople==2)
            {
                AudioSourceManager.Instance.PlaySound(6);
            }
            else
            {
                if (gameManager.isServer)
                {
                    AudioSourceManager.Instance.PlaySound(7);
                }
                else
                {
                    AudioSourceManager.Instance.PlaySound(6);
                }
            }

            if (gameManager.chessPeople == 3)
            {
                gameManager.gameCodeReceive[0] = 1;
                UiManager.Instance.CanClickStartButton(true);
            }
            gameManager.gameOver = true;
            return;
        }
        //将军判定
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
                            uiManager.ShowTip("帅被車将军");
                        }
                        break;
                    case 3:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("帅被马将军");
                        }
                        break;
                    case 4:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("帅被炮将军");
                        }
                        break;
                    case 7:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("帅被卒将军");
                        }
                        break;
                    case 9:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("将被車将军");
                        }
                        break;
                    case 10:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("将被马将军");
                        }
                        break;
                    case 11:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("将被炮将军");
                        }
                        break;
                    case 14:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            AudioSourceManager.Instance.PlaySound(4);
                            uiManager.ShowTip("将被兵将军");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }

    /// <summary>
    /// 获取将帅位置
    /// </summary>
    private void GetKingPosition()
    {
        //获取黑将
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

        //获取红帅
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
