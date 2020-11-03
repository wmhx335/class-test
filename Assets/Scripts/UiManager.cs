using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    public GameObject[] panels;//0.MainMenu 1.Player 2.Model 3. Level 

    public Text tipUIText;

    public Text[] tipUITexts;
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

    #region Panel

    /// <summary>
    /// Player
    /// </summary>
    public void StandaloneMode()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// PVE
    /// </summary>
    public void PVEMode()
    {
        gameManager.chessPeople = 1;
        panels[2].SetActive(false);
        panels[3].SetActive(true);
    }

    /// <summary>
    /// PVP
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

    #region GameLoad
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

    #region UI

    public void UnDo()
    {
        gameManager.chessReseting.ResetChess();
    }

    public void Replay()
    {
        gameManager.Replay();
    }
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
