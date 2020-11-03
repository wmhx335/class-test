using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChessOrGrid : MonoBehaviour
{
    public int xIndex, yIndex;
    public bool isRed;
    public bool isGrid;
    public GameManager gameManager;
    private GameObject gridGo;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gridGo = gameObject;
    }

    public void ClickCheck()
    {
        if (gameManager.gameOver)
        {
            return;
        }
        int itemColorId;
        if (isGrid)
        {
            itemColorId = 0;
        }
        else
        {
            gridGo = transform.parent.gameObject;
            ChessOrGrid chessOrGrid = gridGo.GetComponent<ChessOrGrid>();
            xIndex = chessOrGrid.xIndex;
            yIndex = chessOrGrid.yIndex;
            if (isRed)
            {
                itemColorId = 2;
            }
            else
            {
                itemColorId = 1;
            }
        }
        GridOrChessBehavior(itemColorId,xIndex,yIndex);

    }

    private void GridOrChessBehavior(int itemColorID,int x,int y)
    {

        int FromX, FromY, ToX, ToY;
        gameManager.HideCanEatUI();
        switch (itemColorID)
        {
            case 0:
                gameManager.ClearCurrentCanMoveUIStack();
                ToX = x;
                ToY = y;
                if(gameManager.lastChessOrGrid==null)
                {
                    gameManager.lastChessOrGrid = this;
                    return;
                }
                if (gameManager.chessMove)//redTurn
                {
                    if (gameManager.lastChessOrGrid.isGrid)
                    {
                        return;
                    }
                    if(!gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = null;
                        return;
                    }

                    FromX =gameManager.lastChessOrGrid.xIndex;
                    FromY =gameManager.lastChessOrGrid.yIndex;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard,FromX,FromY,ToX,ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    //棋子移动
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount,FromX,FromY,ToX,ToY,chessOneID,chessTwoID);
                    gameManager.movingOfChess.IsMove(gameManager.lastChessOrGrid.gameObject,gridGo,FromX,FromY,ToX,ToY);
                    UiManager.Instance.ShowTip("黒の番");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.chessMove = false;
                    gameManager.lastChessOrGrid = this;
                    gameManager.HideClickUI();
                    if (gameManager.gameOver)
                    {
                        return;
                    }
                    if (gameManager.chessPeople==2)
                    {
                        return;
                    }
                    if (!gameManager.chessMove)
                    {
                        StartCoroutine("Robot");
                    }
                }
                else//blackTurn
                {
                    if (gameManager.lastChessOrGrid.isGrid)
                    {
                        return;
                    }
                    if(gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = null;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard, FromX, FromY, ToX, ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount, FromX, FromY, ToX, ToY, chessOneID, chessTwoID);
                    gameManager.movingOfChess.IsMove(gameManager.lastChessOrGrid.gameObject, gridGo, FromX, FromY, ToX, ToY);
                    UiManager.Instance.ShowTip("赤の番");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.chessMove = true;
                    gameManager.lastChessOrGrid = this;
                    gameManager.HideClickUI();
                }
                break;

            case 1:
                gameManager.ClearCurrentCanMoveUIStack();
                if (!gameManager.chessMove)//blackTurn
                {
                    FromX = x;
                    FromY = y;
                    gameManager.movingOfChess.ClickChess(FromX, FromY);
                    gameManager.lastChessOrGrid = this;
                    gameManager.ShowClickUI(transform);
                }
                else//redTurn
                {
                    if (gameManager.lastChessOrGrid==null)
                    {
                        return;
                    }
                    if(!gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = this;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    ToX = x;
                    ToY = y;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard, FromX, FromY, ToX, ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount, FromX, FromY, ToX, ToY, chessOneID, chessTwoID);
                    gameManager.movingOfChess.IsEat(gameManager.lastChessOrGrid.gameObject,gameObject,FromX, FromY, ToX, ToY);
                    gameManager.chessMove = false;
                    UiManager.Instance.ShowTip("黒の番");
                    gameManager.lastChessOrGrid = null;
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.HideClickUI();
                    if (gameManager.gameOver)
                    {
                        return;
                    }
                    if (gameManager.chessPeople == 2)
                    {
                        return;
                    }
                    //黑方轮次 AI下棋
                    if (!gameManager.chessMove)
                    {
                        StartCoroutine("Robot");
                    }
                }
                break;

            case 2:
                gameManager.ClearCurrentCanMoveUIStack();
                if (gameManager.chessMove)//redTurn
                {

                    FromX = x;
                    FromY = y;

                    gameManager.movingOfChess.ClickChess(FromX, FromY);
                    gameManager.lastChessOrGrid = this;
                    gameManager.ShowClickUI(transform);
                }
                else
                {
                    if (gameManager.lastChessOrGrid == null)
                    {
                        return;
                    }
                    if (gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = this;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    ToX = x;
                    ToY = y;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard, FromX, FromY, ToX, ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount, FromX, FromY, ToX, ToY, chessOneID, chessTwoID);
                    gameManager.movingOfChess.IsEat(gameManager.lastChessOrGrid.gameObject, gameObject, FromX, FromY, ToX, ToY);
                    gameManager.chessMove = true;
                    UiManager.Instance.ShowTip("赤の番");
                    gameManager.lastChessOrGrid = null;
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.HideClickUI();
                }
                break;

            default:
                break;
        }


    }
    /// <summary>
    /// AI
    /// </summary>
    IEnumerator Robot()
    {
        UiManager.Instance.ShowTip("考え中");
        yield return new WaitForSeconds(0.2f);
        RobortMove();
    }

    private void RobortMove()
    {
        gameManager.movingOfChess.HaveGoodMove(
        gameManager.searchEngine.SearchGoodMove(gameManager.chessBoard));
        gameManager.chessMove = true;
        UiManager.Instance.ShowTip("赤の番");
        gameManager.checkmate.JudgeIfCheckmate();
    }
}
