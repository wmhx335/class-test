using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int chessPeople;// 1.single 2.double 
    public int currentLevel;// 1.easy 2.normal 3.hard

    public int[,] chessBoard;
    public GameObject[,] boardGrid;
    private const float gridWidth = 69.9f;
    private const float gridHeight = 72f;
    private const int gridTotalNum = 90;

    /// <summary>
    /// switch
    /// </summary>
    public bool chessMove;//red:true black:false
    public bool gameOver;
    private bool hasLoad;


    public GameObject gridGo;
    public Sprite[] sprites;
    public GameObject chessGo;
    public GameObject canMovePosUIGo;


    [HideInInspector]
    public GameObject boardGo;
    public GameObject[] boardGos;
    [HideInInspector]
    public ChessOrGrid lastChessOrGrid;
    public Rules rules;
    public MovingOfChess movingOfChess;
    public Checkmate checkmate;
    public ChessReseting chessReseting;
    public SearchEngine searchEngine;
    public GameObject eatChessPool;
    public GameObject clickChessUIGo;
    public GameObject lastPosUIGo;
    public GameObject canEatPosUIGo;
    private Stack<GameObject> canMoveUIStack;
    private Stack<GameObject> currentCanMoveUIStack;

    private void Awake()
    {
        Instance = this;
        gameOver = true;
    }


    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// reset
    /// </summary>
    public void ResetGame()
    {
        gameOver = false;
        chessMove = true;
            if (hasLoad)
            {
                return;
            }

        if (hasLoad)
        {
            return;
        }
        chessBoard = new int[10, 9]
        {
            {2,3,6,5,1,5,6,3,2},
            {0,0,0,0,0,0,0,0,0},
            {0,4,0,0,0,0,0,4,0},
            {7,0,7,0,7,0,7,0,7},
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0},
            {14,0,14,0,14,0,14,0,14},
            {0,11,0,0,0,0,0,11,0},
            {0,0,0,0,0,0,0,0,0},
            {9,10,13,12,8,12,13,10,9}
        };
        boardGrid = new GameObject[10, 9];
        if (chessPeople == 1 || chessPeople == 2)
        {
            boardGo = boardGos[0];
        }
        else
        {
            boardGo = boardGos[1];
        }
        InitGrid();
        InitChess();
        rules = new Rules();
        movingOfChess = new MovingOfChess(this);
        checkmate = new Checkmate();
        chessReseting = new ChessReseting();
        chessReseting.resetCount = 0;
        chessReseting.chessSteps = new ChessReseting.Chess[400];
        canMoveUIStack = new Stack<GameObject>();
        for (int i = 0; i < 40; i++)
        {
            canMoveUIStack.Push(Instantiate(canMovePosUIGo));
        }
        currentCanMoveUIStack = new Stack<GameObject>();
        searchEngine = new SearchEngine();
        hasLoad = true;
    }

    private void InitGrid()
    {
        float posX = 0, posY = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject itemGo = Instantiate(gridGo);
                itemGo.transform.SetParent(boardGo.transform);
                itemGo.name = "Item[" + i.ToString() + "," + j.ToString() + "]";
                itemGo.transform.localPosition = new Vector3(posX, posY, 0);
                posX += gridWidth;
                if (posX >= gridWidth * 9)
                {
                    posY -= gridHeight;
                    posX = 0;
                }
                itemGo.GetComponent<ChessOrGrid>().xIndex = i;
                itemGo.GetComponent<ChessOrGrid>().yIndex = j;
                boardGrid[i, j] = itemGo;
            }
        }
    }

    private void InitChess()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject item = boardGrid[i, j];
                switch (chessBoard[i, j])
                {
                    case 1:
                        CreateChess(item, "b_jiang", sprites[0], false);
                        break;
                    case 2:
                        CreateChess(item, "b_ju", sprites[1], false);
                        break;
                    case 3:
                        CreateChess(item, "b_ma", sprites[2], false);
                        break;
                    case 4:
                        CreateChess(item, "b_pao", sprites[3], false);
                        break;
                    case 5:
                        CreateChess(item, "b_shi", sprites[4], false);
                        break;
                    case 6:
                        CreateChess(item, "b_xiang", sprites[5], false);
                        break;
                    case 7:
                        CreateChess(item, "b_bing", sprites[6], false);
                        break;
                    case 8:
                        CreateChess(item, "r_shuai", sprites[7], true);
                        break;
                    case 9:
                        CreateChess(item, "r_ju", sprites[8], true);
                        break;
                    case 10:
                        CreateChess(item, "r_ma", sprites[9], true);
                        break;
                    case 11:
                        CreateChess(item, "r_pao", sprites[10], true);
                        break;
                    case 12:
                        CreateChess(item, "r_shi", sprites[11], true);
                        break;
                    case 13:
                        CreateChess(item, "r_xiang", sprites[12], true);
                        break;
                    case 14:
                        CreateChess(item, "r_bing", sprites[13], true);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CreateChess(GameObject gridItem, string name, Sprite chessIcon, bool ifRed)
    {
        GameObject item = Instantiate(chessGo);
        item.transform.SetParent(gridItem.transform);
        item.name = name;
        item.GetComponent<Image>().sprite = chessIcon;
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        item.GetComponent<ChessOrGrid>().isRed = ifRed;

    }

    public void BeEat(GameObject itemGo)
    {
        itemGo.transform.SetParent(eatChessPool.transform);
        itemGo.transform.localPosition = Vector3.zero;
    }


    #region UI
    public void ShowClickUI(Transform targetTrans)
    {
        clickChessUIGo.transform.SetParent(targetTrans);
        clickChessUIGo.transform.localPosition = Vector3.zero;
    }
    public void HideClickUI()
    {
        clickChessUIGo.transform.SetParent(eatChessPool.transform);
        clickChessUIGo.transform.localPosition = Vector3.zero;
    }

    public void ShowLastPositionUI(Vector3 showPosition)
    {
        lastPosUIGo.transform.position = showPosition;
    }
    public void HideLastPositionUI()
    {
        lastPosUIGo.transform.position = new Vector3(1000, 1000, 1000);
    }

    public void HideCanEatUI()
    {
        canEatPosUIGo.transform.SetParent(eatChessPool.transform);
        canEatPosUIGo.transform.localPosition = Vector3.zero;
    }

    public GameObject PopCanMoveUI()
    {
        GameObject itemGo = canMoveUIStack.Pop();
        currentCanMoveUIStack.Push(itemGo);
        itemGo.SetActive(true);
        return itemGo;
    }
    public void PushCanMoveUI(GameObject itemGo)
    {
        canMoveUIStack.Push(itemGo);
        itemGo.transform.SetParent(eatChessPool.transform);
        itemGo.SetActive(false);
    }
    public void ClearCurrentCanMoveUIStack()
    {
        while (currentCanMoveUIStack.Count > 0)
        {
            PushCanMoveUI(currentCanMoveUIStack.Pop());

        }
    }

    #endregion

    public void Replay()
    {
        HideLastPositionUI();
        HideClickUI();
        HideCanEatUI();
        ClearCurrentCanMoveUIStack();
        lastChessOrGrid = null;
        UiManager.Instance.ShowTip("赤の番");
        for (int i = chessReseting.resetCount; i > 0; i--)
        {
            chessReseting.ResetChess();
        }
    }

}