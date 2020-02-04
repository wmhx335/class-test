using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 棋子或者格子的脚本
/// </summary>

public class ChessOrGrid : MonoBehaviour
{
    //格子索引
    public int xIndex, yIndex;
    //红黑棋判断
    public bool isRed;
    //是否为格子
    public bool isGrid;
    //游戏管理的引用
    public GameManager gameManager;
    //移动时需要设置的父对象，如果当前对象是棋子，则得到它的父对象而不是它本身
    private GameObject gridGo;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gridGo = gameObject;
    }


    /// <summary>
    /// 点击棋子或格子时触发的检测方法
    /// </summary>
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
            gridGo = transform.parent.gameObject;//得到他的父容器
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
    /// <summary>
    /// 格子与棋子的行为逻辑
    /// </summary>
    /// <param name="itemColorID">格子颜色ID</param>
    /// <param name="x">当前格子x索引</param>
    /// <param name="y">当前格子y索引</param>
    private void GridOrChessBehavior(int itemColorID,int x,int y)
    {

        int FromX, FromY, ToX, ToY;
        gameManager.HideCanEatUI();
        switch (itemColorID)
        {
            //空格子
            case 0:
                gameManager.ClearCurrentCanMoveUIStack();
                ToX = x;
                ToY = y;
                //第一次点空格子
                if(gameManager.lastChessOrGrid==null)
                {
                    gameManager.lastChessOrGrid = this;
                    return;
                }
                if (gameManager.chessMove)//红方轮次
                {
                    if (gameManager.lastChessOrGrid.isGrid)//上一次选中是否为格子
                    {
                        return;
                    }
                    if(!gameManager.lastChessOrGrid.isRed)//上次选中是否为黑色
                    {
                        gameManager.lastChessOrGrid = null;
                        return;
                    }
                    FromX =gameManager.lastChessOrGrid.xIndex;
                    FromY =gameManager.lastChessOrGrid.yIndex;
                    //判断合法
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard,FromX,FromY,ToX,ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    //棋子移动
                    gameManager.movingOfChess.IsMove(gameManager.lastChessOrGrid.gameObject,gridGo,FromX,FromY,ToX,ToY);
                    UiManager.Instance.ShowTip("黑方走");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.chessMove = false;
                    gameManager.lastChessOrGrid = this;
                    gameManager.HideClickUI();
                }
                else//黑方轮次
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
                    gameManager.movingOfChess.IsMove(gameManager.lastChessOrGrid.gameObject, gridGo, FromX, FromY, ToX, ToY);
                    UiManager.Instance.ShowTip("红方走");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.chessMove = true;
                    gameManager.lastChessOrGrid = this;
                    gameManager.HideClickUI();
                }
                break;

            //黑棋
            case 1:
                gameManager.ClearCurrentCanMoveUIStack();
                if (!gameManager.chessMove)//黑色轮次
                {
                    FromX = x;
                    FromY = y;
                    //显示所有可移动路径

                    gameManager.movingOfChess.ClickChess(FromX, FromY);
                    gameManager.lastChessOrGrid = this;
                    gameManager.ShowClickUI(transform);
                }
                else//红色轮次
                {
                    //红吃黑
                    if(gameManager.lastChessOrGrid==null)
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
                    gameManager.movingOfChess.IsEat(gameManager.lastChessOrGrid.gameObject,gameObject,FromX, FromY, ToX, ToY);
                    gameManager.chessMove = false;
                    UiManager.Instance.ShowTip("黑方走");
                    gameManager.lastChessOrGrid = null;
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.HideClickUI();
                }
                break;

            //红棋
            case 2:
                gameManager.ClearCurrentCanMoveUIStack();
                if (gameManager.chessMove)//红色轮次
                {
                    FromX = x;
                    FromY = y;

                    gameManager.movingOfChess.ClickChess(FromX, FromY);
                    gameManager.lastChessOrGrid = this;
                    gameManager.ShowClickUI(transform);
                }
                else//黑色轮次
                {
                    //黑吃红
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
                    gameManager.movingOfChess.IsEat(gameManager.lastChessOrGrid.gameObject, gameObject, FromX, FromY, ToX, ToY);
                    gameManager.chessMove = true;
                    UiManager.Instance.ShowTip("红方走");
                    gameManager.lastChessOrGrid = null;
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.HideClickUI();
                }
                break;

            default:
                break;
        }


    }


}
