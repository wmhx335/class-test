using System.Collections;
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
    private bool hasLoad;//当前游戏是否已经加载

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

    /// <summary>
    /// 联网
    /// </summary>
    //客户端
    public GameCilient gameCilient;
    //服务器
    public GameServer gameServer;
    //当前运行程序是否为服务器端程序
    public bool isServer;
    //消息序列
    public int[] gameCodeReceive;
    //双方准备开关
    private bool redHasReady;
    private bool blackHasReady;
    //收到新消息的开关
    public bool receiveNewCode;
    //当前棋盘是否加载的开关
    private bool netModeHasLoad;

    private void Awake()
    {
        Instance = this;
        gameOver = true;
        gameCodeReceive = new int[6];
    }


    // Update is called once per frame
    void Update()
    {
        if (receiveNewCode)
        {
            receiveNewCode = false;
            ParseGameCode();
        }
    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    public void ResetGame()
    {
        gameOver = false;
        chessMove = true;
        if (chessPeople==3)
        {
            if (netModeHasLoad)
            {
                return;
            }
        }
        else
        {
            if (hasLoad)
            {
                return;
            }
        }

        //防止二次加载游戏
        if (hasLoad)
        {
            return;
        }
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
        for (int i = 0; i < 40; i++)
        {
            canMoveUIStack.Push(Instantiate(canMovePosUIGo));
        }
        currentCanMoveUIStack = new Stack<GameObject>();
        searchEngine = new SearchEngine();
        //已经加载过游戏了
        hasLoad = true;
        netModeHasLoad = true;
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

    /// <summary>
    /// 生成棋子游戏物体
    /// </summary>
    /// <param name="gridItem">作为父对象的格子</param>
    /// <param name="name">棋子名称</param>
    /// <param name="chessIcon">棋子标志样式</param>
    /// <param name="ifRed">是否为红色方</param>
    private void CreateChess(GameObject gridItem, string name, Sprite chessIcon, bool ifRed)
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
        while (currentCanMoveUIStack.Count > 0)
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
        for (int i = chessReseting.resetCount; i > 0; i--)
        {
            chessReseting.ResetChess();
        }
    }

    #region 联网相关方法
    /// <summary>
    /// 联网方法
    /// </summary>
    public void PlayerConnected()
    {
        gameCilient = new GameCilient("127.0.0.1", 8888);
        //判断客户端启动是否成功
        if (!gameCilient.Start())
        {
            //不成功则没有服务器，需要把当前运行程序作为服务器
            gameCilient.Close();
            gameCilient = null;
            gameServer = new GameServer("127.0.0.1", 8888);
            gameServer.Start();
            isServer = true;
        }
        else
        {
            isServer = false;
        }
    }

    /// <summary>
    /// 双方玩家的准备方法
    /// </summary>
    public void BeReady()
    {
        if (isServer)
        {
            //服务器红方
            gameServer.SendMsg(new int[6] { 2, 1, 0, 0, 0, 0 });
            redHasReady = true;
        }
        else
        {
            //客户端黑方
            gameCilient.SendMsg(new int[6] { 2, 1, 0, 0, 0, 0 });
            blackHasReady = true;
        }
        StartGame();
 
    }

    /// <summary>
    /// 双方都准备好，开始游戏
    /// </summary>
    private void StartGame()
    {
        if (redHasReady && blackHasReady)
        {
            redHasReady = blackHasReady = false;
            UiManager.Instance.CanClickStartButton(false);
            ResetGame();
        }
    }

    /// <summary>
    /// 认输
    /// </summary>
    public void GiveUp()
    {
        gameOver = true;
        if (isServer)
        {
            gameServer.SendMsg(new int[6] { 1, 1, 0, 0, 0, 0 });
        }
        else
        {
            gameServer.SendMsg(new int[6] { 1, 0, 0, 0, 0, 0 });
        }
        UiManager.Instance.CanClickStartButton(true);
    }
    /// <summary>
    /// 解析游戏编码为当前着法
    /// </summary>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <param name="toX"></param>
    /// <param name="toY"></param>
    /// <returns></returns>
    private ChessReseting.Chess CodeToStep(int fromX,int fromY,int toX,int toY)
    {
        ChessReseting.Chess chessStep = new ChessReseting.Chess();
        GameObject gridOne = boardGrid[fromX, fromY];
        GameObject gridTwo = boardGrid[toX, toY];
        chessStep.gridOne = gridOne;
        chessStep.gridTwo = gridTwo;
        GameObject firstChess = gridOne.transform.GetChild(0).gameObject;
        chessStep.chessOne = firstChess;
        chessStep.chessOneID = chessBoard[fromX, fromY];
        chessStep.chessTwoID = chessBoard[toX, toY];
        //吃子
        if (gridTwo.transform.childCount!=0)
        {
            GameObject secondChess = gridTwo.transform.GetChild(0).gameObject;
            chessStep.chessTwo = secondChess;
        }
        return chessStep;
    } 

    /// <summary>
    /// 解析接收到的游戏编码
    /// </summary>
    public void ParseGameCode()
    {
        //当前游戏开始且未结束时，需要走棋
        if (!gameOver)
        {
            //根据信息解析为来去位置走棋
            movingOfChess.HaveGoodMove(CodeToStep(gameCodeReceive[2],
                gameCodeReceive[3], gameCodeReceive[4], gameCodeReceive[5]));
            //限制一方不能走棋
            chessMove = !chessMove;
            if (chessMove)
            {
                UiManager.Instance.ShowTip("红方走");
            }
            else
            {
                UiManager.Instance.ShowTip("黑方走");
            }
        }
        int gameState = gameCodeReceive[0];
        switch (gameState)
        {
            case 0:
                gameOver = false;
                break;
            case 1:
                gameOver = true;
                chessMove = !chessMove;
                //黑方
                if (gameCodeReceive[1]==0)
                {
                    UiManager.Instance.ShowTip("红方胜利");
                }
                else if(gameCodeReceive[1] == 1)
                {
                    UiManager.Instance.ShowTip("黑方胜利");
                }
                UiManager.Instance.CanClickStartButton(true);
                break;
            case 2:
                if (isServer)
                {
                    blackHasReady = true;
                    StartGame();
                }
                else
                {
                    redHasReady = true;
                    StartGame();
                }
                break;
            case 3:
                UiManager.Instance.CanClickStartButton(true);
                break;
            default:
                break;
        }

    }


    #endregion
}