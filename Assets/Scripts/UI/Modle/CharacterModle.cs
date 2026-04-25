using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModle :BaseManager<CharacterModle>
{

// 角色信息配置表
    RoleInfos roleInfos;
// 玩家数据
     PlayerData playerData;
   // 当前选择的角色索引
    int nowIndex = 0;
   // 当前选择的角色信息
    public RoleInfos.RoleData nowRoleInfo;
    public Transform heroPos;
// 当前选择的角色游戏对象
    public GameObject nowRole;



    private CharacterModle()
    {
    }

public void Init()
    {
        roleInfos = ConfigMgr.Instance.roleInfos;
        playerData = SaveMgr.Instance.playerData;
        
        heroPos = GameObject.Find("HeroPos").transform;
        EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateMoney, playerData.ownMoney);
        if (heroPos == null)
        {
            Debug.LogError("找不到 HeroPos 对象！");
            return;
        }
        
        // 确保 nowIndex 在有效范围内
        if (roleInfos == null || roleInfos.roleDatas == null || roleInfos.roleDatas.Count == 0)
        {
            Debug.LogError("角色配置数据为空！");
            return;
        }
        
        // 销毁之前创建的角色
        if (nowRole != null)
        {
            GameObject.Destroy(nowRole);
            nowRole = null;
        }
        
        nowIndex = 0;
        nowRoleInfo = roleInfos.roleDatas[nowIndex];
        ABMgr.Instance.LoadResAsync<GameObject>("modle", nowRoleInfo.showRes, (res) =>
        {
            if (res != null && heroPos != null)
            {
                nowRole = GameObject.Instantiate(res, heroPos.position, heroPos.rotation);
                if (roleInfos.roleDatas.Count > 0)
                {
                    EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateRoleInfo, roleInfos.roleDatas[nowIndex]);
                }
            }
        }).Forget();
      
        EventCenter.Instance.AddEventListener(E_EventType.E_UI_Char_Start, Start);
        EventCenter.Instance.AddEventListener(E_EventType.E_UI_Char_Back, Back);
        EventCenter.Instance.AddEventListener(E_EventType.E_UI_Char_Prev, Prev);
        EventCenter.Instance.AddEventListener(E_EventType.E_UI_Char_Next, Next);
        EventCenter.Instance.AddEventListener(E_EventType.E_UI_Char_Buy, Buy);
    }
    // 开始游戏，进入关卡选择界面
    public void Start()
    {
     UIMgr.Instance.ShowPanel<LevelPanel>();
     UIMgr.Instance.HidePanel<CharacterChoosePanel>();
      ConfigMgr.Instance.roleSeInfo = nowRoleInfo;
    }

    // 返回主界面
    public void Back()
    {

        Camera.main.GetComponent<CameraMove>().Turnright(() =>
        {
            UIMgr.Instance.ShowPanel<BeginPanel>();
            UIMgr.Instance.HidePanel<CharacterChoosePanel>();
           if(nowRole != null)
            GameObject.Destroy(nowRole);

        });

    }

    public void Prev()
    {
        if(nowRole != null)
        {
            GameObject.Destroy(nowRole);
        }
        nowIndex--;
        if (nowIndex < 0)
        {
            nowIndex = roleInfos.roleDatas.Count - 1;
        }
        
        if (nowIndex >= 0 && nowIndex < roleInfos.roleDatas.Count)
        {
            nowRoleInfo = roleInfos.roleDatas[nowIndex];
            if (nowRoleInfo != null)
            {
                ABMgr.Instance.LoadResAsync<GameObject>("modle", nowRoleInfo.showRes, (res) =>
                {
                    if (res != null && heroPos != null)
                    {
                        nowRole = GameObject.Instantiate(res, heroPos.position, heroPos.rotation);
                        EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateRoleInfo, nowRoleInfo);
                    }
                }).Forget();
            }
        }
    }


    public void Next()
    {
        if(nowRole != null)
        {
            GameObject.Destroy(nowRole);
        }
        nowIndex++;
        if (nowIndex >= roleInfos.roleDatas.Count)
        {
            nowIndex = 0;
        }
        
        if (nowIndex >= 0 && nowIndex < roleInfos.roleDatas.Count)
        {
            nowRoleInfo = roleInfos.roleDatas[nowIndex];
            if (nowRoleInfo != null)
            {
                ABMgr.Instance.LoadResAsync<GameObject>("modle", nowRoleInfo.showRes, (res) =>
                {
                    if (res != null && heroPos != null)
                    {
                        nowRole = GameObject.Instantiate(res, heroPos.position, heroPos.rotation);
                        EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateRoleInfo, nowRoleInfo);
                    }
                }).Forget();
            }
        }
    }



    public void Buy()
    {
        if (nowRoleInfo == null)
        {
            Debug.LogError("当前角色信息为空！");
            EventCenter.Instance.EventTrigger(E_EventType.E_UI_ShowTip, "请先选择角色");
            return;
        }
        
        if (playerData == null)
        {
            Debug.LogError("玩家数据为空！");
            return;
        }
        
        if(playerData.ownMoney >= nowRoleInfo.lockMoney)
        {
            playerData.ownMoney -= nowRoleInfo.lockMoney;
            playerData.ownRoles.Add(nowRoleInfo.id);
            EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateMoney, playerData.ownMoney);
           EventCenter.Instance.EventTrigger(E_EventType.E_UI_UpdateRoleInfo, nowRoleInfo);
            EventCenter.Instance.EventTrigger(E_EventType.E_UI_ShowTip, "购买成功");
            SaveMgr.Instance.SavePlayerData();
        }
        else
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_UI_ShowTip, "余额不足");
        }
    }


public void RemoveEvent()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UI_Char_Start, Start);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UI_Char_Back, Back);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UI_Char_Prev, Prev);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UI_Char_Next, Next);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_UI_Char_Buy, Buy);
    }






}
