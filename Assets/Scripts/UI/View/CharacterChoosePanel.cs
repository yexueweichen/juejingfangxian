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
    //购买按钮
    public Button payButton;
    public TextMeshProUGUI moneyNmub;
    //当前角色需要的金币
    public TextMeshProUGUI payMoney;
    public TextMeshProUGUI nameText;
    public RoleInfos.RoleData nowRoleInfo;



    protected override void Init()
    {
        CharacterModle.Instance.Init();
        startButton.onClick.AddListener(() => EventCenter.Instance.EventTrigger(E_EventType.E_UI_Char_Start));
        backButton.onClick.AddListener(() => EventCenter.Instance.EventTrigger(E_EventType.E_UI_Char_Back));
        leftButtton.onClick.AddListener(() => EventCenter.Instance.EventTrigger(E_EventType.E_UI_Char_Prev));
        rightButton.onClick.AddListener(() => EventCenter.Instance.EventTrigger(E_EventType.E_UI_Char_Next));
        payButton.onClick.AddListener(() => EventCenter.Instance.EventTrigger(E_EventType.E_UI_Char_Buy));

        EventCenter.Instance.AddEventListener<int>(E_EventType.E_UI_UpdateMoney, OnUpdateMoney);
        EventCenter.Instance.AddEventListener<RoleInfos.RoleData>(E_EventType.E_UI_UpdateRoleInfo, OnUpdateRoleInfo);
        EventCenter.Instance.AddEventListener<string>(E_EventType.E_UI_ShowTip, OnShowTip);
         EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateMoney, SaveMgr.Instance.playerData.ownMoney);


    }

    private void OnDestroy()
    {
        // 注销事件，防止内存泄漏
        if (EventCenter.Instance != null)
        {
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_UI_UpdateMoney, OnUpdateMoney);
            EventCenter.Instance.RemoveEventListener<RoleInfos.RoleData>(E_EventType.E_UI_UpdateRoleInfo, OnUpdateRoleInfo);
            EventCenter.Instance.RemoveEventListener<string>(E_EventType.E_UI_ShowTip, OnShowTip);
            CharacterModle.Instance.RemoveEvent();
        }
    }

    // EventCenter 推送：更新金钱显示
    private void OnUpdateMoney(int money)
    {
        if (moneyNmub != null)
            moneyNmub.text = money.ToString();
    }

    // EventCenter 推送：基础角色信息（名字/是否可购买/价格），视图据此更新按钮显示
    private void OnUpdateRoleInfo(RoleInfos.RoleData role)
    {
        if (role == null) return;
        nameText.text = role.tips;
        if (!SaveMgr.Instance.playerData.ownRoles.Contains(role.id) && role.lockMoney > 0)//未购买且需要金币购买
        {
            payButton.gameObject.SetActive(true);
            payMoney.text = "¥" + role.lockMoney;
            startButton.gameObject.SetActive(false);
        }
        else
        {
            payButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
    }

   

    private void OnShowTip(string msg)
    {
        if (string.IsNullOrEmpty(msg)) return;
        // 使用回调获取 TipPanel 并设置信息
        UIMgr.Instance.ShowPanel<TipPanel>((panel) =>
        {
            panel.SetInfo(msg);
        });
    }
}

