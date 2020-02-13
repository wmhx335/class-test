﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 存储游戏数据，游戏引用，游戏资源，模式切换与控制
/// </summary>


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int chessPeople;//对战人数 1.单人 2.双人 3.联网
    public int currentLevel;//难度 1.简单 2.普通 3.困难

    /// <summary>
    /// 数据
    /// </summary>
    public int[,] chessBoard;//当前棋盘状况
    public GameObject[,] boardGrid;//棋盘上的所有格子
    private const float gridWidth = 69.9f;
    private const float gridHeight = 72f;
    private const int gridTotalNum = 90;

    /// <summary>
    /// 开关
    /// </summary>
    public bool chessMove;//行动方，红true黑black
    public bool gameOver;//游戏结束

    /// <summary>
    /// 资源
    /// </summary>
    public GameObject gridGo;//格子
    public Sprite[] sprites;//所有棋子的sprite
    public GameObject chessGo;//棋子
    public GameObject canMovePosUIGo;//可以移动到的位置显示

    /// <summary>
    /// 引用
    /// </summary>
    [HideInInspector]
    public GameObject boardGo;
    public GameObject[] boardGos;//0.单机 1.联网
    [HideInInspector]
    public ChessOrGrid lastChessOrGrid;//上一次点击到的对象
    public Rules rules;//规则类
    public MovingOfChess movingOfChess;//移动类
    public Checkmate checkmate;//将军检测类
    public ChessReseting chessReseting;//悔棋
    public SearchEngine searchEngine;//搜索引擎
    public GameObject eatChessPool;//被吃掉的棋子池
    public GameObject clickChessUIGo;//选中棋子的UI
    public GameObject lastPosUIGo;//棋子移动前的位置
    public GameObject canEatPosUIGo;//可以吃掉的棋子UI显示
    private Stack<GameObject> canMoveUIStack;//移动位置UI显示游戏物体的对象池
    private Stack<GameObject> currentCanMoveUIStack;//当前移动位置UI已经显示的游戏物体栈；
    

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //测试
        chessPeople = 1;
        currentLevel = 3;
        ResetGame();

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    public void ResetGame()
    {
        chessMove = true;
        //初始化棋盘
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
        if (chessPeople == 1|| chessPeople == 2)
        {
            boardGo = boardGos[0];
        }
        else
        {
            boardGo = boardGos[1];
        }
        InitGrid();
        InitChess();
        //规则类对象
        rules = new Rules();
        //移动类对象
        movingOfChess = new MovingOfChess(this);
        //将军检测对象
        checkmate = new Checkmate();
        //悔棋类对象
        chessReseting = new ChessReseting();
        chessReseting.resetCount = 0;
        chessReseting.chessSteps = new ChessReseting.Chess[400];
        //移动UI显示的栈
        canMoveUIStack = new Stack<GameObject>();
        for (int i=0;i<40;i++)
        {
            canMoveUIStack.Push(Instantiate(canMovePosUIGo));
        }
        currentCanMoveUIStack = new Stack<GameObject>();
        searchEngine = new SearchEngine();

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

    /// <summary>
    /// 实例化棋子
    /// </summary>
    private void InitChess()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j=0;j<9;j++)
            {
                GameObject item = boardGrid[i, j];
                switch(chessBoard[i,j])
                {
                    case 1:
                        CreateChess(item, "b_jiang",sprites[0], false);
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

    /// <summary>
    /// 生成棋子游戏物体
    /// </summary>
    /// <param name="gridItem">作为父对象的格子</param>
    /// <param name="name">棋子名称</param>
    /// <param name="chessIcon">棋子标志样式</param>
    /// <param name="ifRed">是否为红色方</param>
    private void CreateChess(GameObject gridItem,string name,Sprite chessIcon,bool ifRed)
    {
        GameObject item = Instantiate(chessGo);
        item.transform.SetParent(gridItem.transform);
        item.name = name;
        item.GetComponent<Image>().sprite = chessIcon;
        item.transform.localPosition = Vector3.zero;//设置棋子位置
        item.transform.localScale = Vector3.one;//设置棋子大小
        item.GetComponent<ChessOrGrid>().isRed = ifRed;

    }
    /// <summary>
    /// 被吃掉棋子的处理方法
    /// </summary>
    /// <param name="itemGo"></param>
    public void BeEat(GameObject itemGo)
    {
        itemGo.transform.SetParent(eatChessPool.transform);
        itemGo.transform.localPosition = Vector3.zero;
    }


    #region 关于游戏进行中UI显示隐藏的方法
    /// <summary>
    /// 显示/隐藏选中棋子UI
    /// </summary>
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

    /// <summary>
    /// 显示隐藏棋子移动前的位置UI
    /// </summary>
    public void ShowLastPositionUI(Vector3 showPosition)
    {
        lastPosUIGo.transform.position = showPosition;
    }
    public void HideLastPositionUI()
    {
        lastPosUIGo.transform.position = new Vector3(1000, 1000, 1000);
    }

    /// <summary>
    /// 隐藏可吃棋子的UI显示
    /// </summary>
    public void HideCanEatUI()
    {
        canEatPosUIGo.transform.SetParent(eatChessPool.transform);
        canEatPosUIGo.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 当前选中棋子可以移动到的位置UI显示隐藏
    /// </summary>
    /// <returns></returns>
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
        while (currentCanMoveUIStack.Count>0)
        {
            PushCanMoveUI(currentCanMoveUIStack.Pop());
            
        }
    }

    #endregion

    /// <summary>
    /// 重玩
    /// </summary>
    public void Replay()
    {
        HideLastPositionUI();
        HideClickUI();
        HideCanEatUI();
        ClearCurrentCanMoveUIStack();
        lastChessOrGrid = null;
        UiManager.Instance.ShowTip("红方走");
        for (int i=chessReseting.resetCount;i>0;i--)
        {
            chessReseting.ResetChess();
        }
    }
}