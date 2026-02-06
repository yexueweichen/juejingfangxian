using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
   public Button quitButton;
    public Button stopButton;
    public Button startButton;
    public Image hpImage;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moenyText;
    public TextMeshProUGUI enmeyText;
    public float hpWidth = 600f;
    public Transform Root;
    public List<TowerTips> towerButtons = new List<TowerTips>();



    protected override void Init()
    {
     startButton.gameObject.SetActive(false);
        Root.gameObject.SetActive(false);
        //退出游戏
        quitButton.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<GamePanel>();
           SceneManager.LoadScene(0);
        });
        //暂停游戏
        stopButton.onClick.AddListener(() =>
    {
        Time.timeScale = 0;
        startButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);


    });
        //开始游戏
        startButton.onClick.AddListener(() =>
    {
        Time.timeScale = 1;
        startButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    });




    }

    //更新血量UI
    public void UpdateTowerHp(int hp,int maxHP)
    {
        hpText.text = hp + "/" + maxHP;
        hpImage.rectTransform.sizeDelta = new Vector2(hpWidth * (float)hp / maxHP, hpImage.rectTransform.sizeDelta.y);
    }

    //更新金钱UI
    public void UpdateMoney(int money)
    {
        moenyText.text = money.ToString();
    }
    //更新敌人波数UI
    public void UpdateEnemy(int enemy,int enemyMax)
    {
        hpText.text = enemy+ "/" + enemyMax;
        
    }











}
