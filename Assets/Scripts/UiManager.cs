using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制页面之间的显示与跳转，按钮的触发方法，在gameManager之后实例化
/// </summary>



public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    public GameObject[] panels;//0.主菜单 1.单机 2.模式选择 3. 难度选择 4.单机游戏 5.联网游戏

    public Text tipUIText;//当前需要具体改变文本的显示UI

    public Text[] tipUITexts;//两个对应显示UI的引用
    [HideInInspector]
    public GameManager gameManager;

    public Button netModePlayButton;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region 页面跳转

    /// <summary>
    /// 单机模式
    /// </summary>
    public void StandaloneMode()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(true);
    }

    /// <summary>
    /// 联网模式
    /// </summary>
    public void NetWorkingMode()
    {
        panels[0].SetActive(false);
        panels[5].SetActive(true);
        gameManager.PlayerConnected();
        gameManager.chessPeople = 3;
        tipUIText = tipUITexts[1];
        CanClickStartButton(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 人机模式
    /// </summary>
    public void PVEMode()
    {
        gameManager.chessPeople = 1;
        panels[2].SetActive(false);
        panels[3].SetActive(true);
    }

    /// <summary>
    /// 双人模式
    /// </summary>

    public void PVPMode()
    {
        tipUIText = tipUITexts[0];
        gameManager.chessPeople = 2;

        LoadGame();

    }

    public void LevelMode(int Level)
    {
        gameManager.currentLevel = Level;
        tipUIText = tipUITexts[0];
        LoadGame();
    }

    #endregion

    #region 加载游戏
    private void LoadGame()
    {
        gameManager.ResetGame();
        SetUI();
        panels[4].SetActive(true);
    }

    private void SetUI()
    {
        panels[2].SetActive(true);
        panels[3].SetActive(false);
        panels[1].SetActive(false);
        panels[0].SetActive(true);
    }

    #endregion

    #region 游戏中的UI方法
    /// <summary>
    /// 悔棋
    /// </summary>
    public void UnDo()
    {
        gameManager.chessReseting.ResetChess();
    }

    /// <summary>
    /// 重玩
    /// </summary>
    public void Replay()
    {
        gameManager.Replay();
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void ReturnToMain()
    {
        panels[4].SetActive(false);
        gameManager.Replay();
        gameManager.gameOver = true;
    }

    /// <summary>
    /// Tip
    /// </summary>
    public void ShowTip(string str)
    {
        
        tipUIText.text = str;
    }

    /// <summary>
    /// 开始联网
    /// </summary>
    public void StartNetWorkingMode()
    {
        gameManager.BeReady();
    }

    /// <summary>
    /// 认输
    /// </summary>
    public void GiveUp()
    {
        gameManager.GiveUp();
    }

    /// <summary>
    /// 是否可以点击开始按钮
    /// </summary>
    public void CanClickStartButton(bool canClick)
    {
        if (canClick)
        {
            netModePlayButton.interactable = true;
        }
        else
        {
            netModePlayButton.interactable = false;
        }
    }
    #endregion


}
