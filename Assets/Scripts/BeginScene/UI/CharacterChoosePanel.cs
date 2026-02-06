using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterChoosePanel : BasePanel
{
    public Button startButton;
    public Button backButton;
    public Button leftButtton;
    public Button rightButton;
    public Button payButton;
    public TextMeshProUGUI moneyNmub;
    public TextMeshProUGUI payMoney;
    private Transform heroPos;
    public int nowIndex = 0;
    public GameObject nowHero;
    public TextMeshProUGUI nameText;
    public RoleInfos.RoleData nowRoleInfo;



    protected override void Init()
    {
        heroPos = GameObject.Find("HeroPos").transform;
        moneyNmub.text = GameDataMgr.Instance.playerData.ownMoney.ToString();
        ShowModle();
       
        startButton.onClick.AddListener(OnStartButtonClick);
       
        backButton.onClick.AddListener(OnBackButtonClick);
       
        leftButtton.onClick.AddListener(OnLeftButtonClick);
       
        rightButton.onClick.AddListener(OnRightButtonClick);
       
        payButton.onClick.AddListener(() =>
        {
           PlayerData data= GameDataMgr.Instance.playerData;
            if(data.ownMoney >= nowRoleInfo.lockMoney)
            {
                data.ownMoney -= nowRoleInfo.lockMoney;
                data.ownRoles.Add(nowRoleInfo.id);
                moneyNmub.text = data.ownMoney.ToString();
                GameDataMgr.Instance.SavePlayerData();
                Updatepay();
                TipPanel tipPanel=  UIMgr.Instance.ShowPanel<TipPanel>();
               tipPanel.SetInfo("购买成功，恭喜获得" + nowRoleInfo.tips + "角色！");
            }
       
            else
            {
                UIMgr.Instance.ShowPanel<TipPanel>();
            }



        });
    }

   //开始
    private void OnStartButtonClick()
    {
        GameDataMgr.Instance.roleSeInfo = nowRoleInfo;
        UIMgr.Instance.HidePanel<CharacterChoosePanel>();
        UIMgr.Instance.ShowPanel<LevelPanel>();


    }
    //返回
    private void OnBackButtonClick()
    {
        Camera.main.GetComponent<CameraMove>().Turnright(() =>
        {
            UIMgr.Instance.ShowPanel<BeginPanel>();
            UIMgr.Instance.HidePanel<CharacterChoosePanel>();
            GameObject.Destroy(nowHero);
            nowHero = null;
        });
    }
   //左一个
    private void OnLeftButtonClick()
    {
        --nowIndex;
        if (nowIndex < 0)
            nowIndex = GameDataMgr.Instance.roleInfos.roleDatas.Count - 1;
        ShowModle();
    }

    //右一个
    private void OnRightButtonClick()
    {

        ++nowIndex;
        if (nowIndex > GameDataMgr.Instance.roleInfos.roleDatas.Count - 1)
            nowIndex = 0;
        ShowModle();
    }
    //显示角色
    public void ShowModle()
    {
        if (nowHero != null)
        {
            GameObject.Destroy(nowHero);
            nowHero = null;
        }
        nowRoleInfo = GameDataMgr.Instance.roleInfos.roleDatas[nowIndex];
        nowHero = Instantiate(Resources.Load<GameObject>(nowRoleInfo.res), heroPos.position, heroPos.rotation);
        nameText.text = nowRoleInfo.tips;
        Destroy(nowHero.GetComponent<Player>());
        Updatepay();
    }

    //购买按钮更新
    public void Updatepay()
    {
        if (!GameDataMgr.Instance.playerData.ownRoles.Contains(nowRoleInfo.id) && nowRoleInfo.lockMoney > 0)
        {
            payButton.gameObject.SetActive(true);
            payMoney.text = "¥" + nowRoleInfo.lockMoney;
            startButton.gameObject.SetActive(false);
        }
        else
        {
            payButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }


    }
}
