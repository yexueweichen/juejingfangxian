using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverPanel : BasePanel
{
    public TextMeshProUGUI finalText;
    public TextMeshProUGUI scoreText;
    public Button sureButton;

    protected override void Init()
    {
        sureButton.onClick.AddListener(() =>
        {

            UIMgr.Instance.HidePanel<GameOverPanel>();
            UIMgr.Instance.HidePanel<GamePanel>();
            SceneManager.LoadScene(0);
            GameLevelMgr.Instance.ClearInfo();
            //ABMgr.Instance.ClearAB();
            MusicMgr.Instance.ClearSound();

        });
    }

    public void InitInfo(int money, bool isWin)
    {

        finalText.text = isWin ? "你赢了" : "你输了";

        scoreText.text = isWin?"获得胜利奖励¥：" +"\n" + money:
            "获得失败奖励¥：" + "\n" + money;

     SaveMgr.Instance.playerData.ownMoney += money;
        SaveMgr.Instance.SavePlayerData();

    }

    public override void ShowMe()
    {
        base.ShowMe();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



}
