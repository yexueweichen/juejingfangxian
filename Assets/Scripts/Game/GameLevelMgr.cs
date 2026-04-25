﻿﻿using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;


public class GameLevelMgr:BaseManager<GameLevelMgr>

{
    private GameLevelMgr()
    {
    }

    //玩家角色
    public Player player;
    
    //主塔
    public MainTower mainTower;

    //所有的出怪点
    private List<MonsterPoint> points = new ();
    //记录当前 还有多少波怪物
    private int haveBoCi = 0;
    //记录 一共有多少波怪物
    private int ZongBoCi = 0;
    //保存当前场景信息
    public SceneInfos.SceneInfo currentSceneInfo;

    //记录当前场景上的怪物数量
    private int nowMonsterNum = 0;

    public List<Monster> aliveMonsters = new ();


    //初始化游戏场景
    public void InitInfo(SceneInfos.SceneInfo sceneInfo)
    {
        //先清理之前的游戏状态
        ClearInfo();
        
        currentSceneInfo = sceneInfo;
        
        if (currentSceneInfo == null)
        {
            Debug.LogError("sceneInfo 为空！");
            return;
        }
        
        //初始化 MainTower
         mainTower = MainTower.Instance;
        if (mainTower != null)
        {
            mainTower.Init(currentSceneInfo.towerHp, currentSceneInfo.towerHp);
        }
        else
        {
            Debug.LogError("找不到 MainTower 实例！");
        }
        
        EventCenter.Instance.EventTrigger(E_EventType.E_UpdateTowerHp);
        EventCenter.Instance.EventTrigger(E_EventType.E_UpdateTEnergy);
        //玩家创建
        //获取玩家选择时记录的当前选中的玩家数据
        RoleInfos.RoleData roleInfo = ConfigMgr.Instance.roleSeInfo;
        if (roleInfo == null)
        {
            Debug.LogError("GameLevelMgr.InitInfo: roleInfo 为空设置玩家数据！");
            return;
        }
        
        if (string.IsNullOrEmpty(roleInfo.res))
        {
            Debug.LogError("GameLevelMgr.InitInfo: roleInfo.res 为空检查玩家配置！");
            return;
        }
        
        //获取到场景当中 玩家的出生位置
        GameObject heroPosObj = GameObject.Find("HeroBornPos");
        if (heroPosObj == null)
        {
            Debug.LogError("HeroBornPos 对象！");
            return;
        }
        
        Transform heroPos = heroPosObj.transform;
        
        //实例化玩家预设体 然后把它的位置角度 设置为 场景当中出生点一致
        ABMgr.Instance.LoadResAsync<GameObject>("modle", roleInfo.res, async (obj) =>
        {
            if (obj == null)
            {
                Debug.LogError($"加载玩家预制体失败！资源名：{roleInfo.res}");
                return;
            }
            
            GameObject heroObj = GameObject.Instantiate(obj, heroPos.position, heroPos.rotation);
            //实例化完成后 对玩家对象进行初始化
            player = heroObj.GetComponent<Player>();
            if (player == null)
            {
                player = heroObj.AddComponent<Player>();
            }
    
            //等待一帧确保 Player 的 Start 方法已经执行
            await UniTask.Yield();
            
           CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow == null)
         {
    cameraFollow = Camera.main.gameObject.AddComponent<CameraFollow>();
         }
cameraFollow.SetTarget(heroObj.transform);
        }).Forget();


    
    }

    //增加一个出怪点
    public void AddMonsterPoint(MonsterPoint point)
    {
        points.Add(point);
    }

    /// <summary>
    /// 更新一共有多少波怪
    /// </summary>
    /// <param name="num"></param>
    public void UpdateZongBoCi(int num)
    {
        ZongBoCi += num;
        haveBoCi += num;
        //更新界面
        GamePanel panel = UIMgr.Instance.GetPanel<GamePanel>();
        if (panel != null)
        {
            panel.UpdateEnemy(haveBoCi, ZongBoCi);
        }
    }

    //改变当前还有多少波怪
    public void ChangeHaveBoCi(int num)
    {
       haveBoCi -= num;
        //更新界面
        GamePanel panel = UIMgr.Instance.GetPanel<GamePanel>();
        if (panel != null)
        {
            panel.UpdateEnemy(haveBoCi, ZongBoCi);
        }
    }

    //检查游戏是否胜利
    public bool CheckOver()
    {
        for (int i = 0; i < points.Count; i++)
        {
            //只要有一个出怪点 还没有出完怪  那么就证明还没有胜利
            if (!points[i].CheckOver())
            {
                return false;
            }
        }

        if (nowMonsterNum > 0)
        {
            return false;
        }

        Debug.Log("游戏胜利条件全部满足");
        
        // 通知任务系统关卡完成
        if (QuestMgr.Instance != null)
        {
            QuestMgr.Instance.NotifyLevelCompleted();
        }
        
        return true; 
    }


     public void ChangeMonsterNum(int num)
    {
        nowMonsterNum += num;
    }


    public void ClearInfo()
    {
        points.Clear();
        ZongBoCi=haveBoCi=nowMonsterNum= 0;
        player = null;
        PoolMgr.Instance.ClearAllPools();
    }

    //添加一个怪物到炮塔查找怪物的列表中
    public void AddMonster(Monster obj)
    {
        aliveMonsters.Add(obj);
    }
    
    //移除一个怪物从炮塔查找怪物的列表中
    public void RemoveMonster(Monster obj)
    {
        aliveMonsters.Remove(obj);
    }

    /// <summary>
    /// 查找范围内最近的一个怪物
    /// </summary>
    /// <param name="pos">中心位置</param>
    /// <param name="range">范围半径</param>
    /// <returns>最近的怪物 Transform，如果没有找到返回 null</returns>
    public Transform FindMonster(Vector3 pos, int range)
    {
        Transform nearestMonster = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < aliveMonsters.Count; i++)
        {
            // 添加空值检查，防止访问已销毁的对象
            if (aliveMonsters[i] == null)
            {
                // 移除已销毁的对象引用
                aliveMonsters.RemoveAt(i);
                i--; // 调整索引
                continue;
            }
            
            if (!aliveMonsters[i].isDead)
            {
                float distance = Vector3.Distance(pos, aliveMonsters[i].transform.position);
                if (distance <= range && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestMonster = aliveMonsters[i].transform;
                }
            }
        }
        return nearestMonster;
    }

    public List<Monster> FindMonsters(Vector3 pos, int range)
    {
        List<Monster> list = new List<Monster>();
        for (int i = 0; i < aliveMonsters.Count; i++)
        {
            // 添加空值检查，防止访问已销毁的对象
            if (aliveMonsters[i] == null)
            {
                // 移除已销毁的对象引用
                aliveMonsters.RemoveAt(i);
                i--; // 调整索引
                continue;
            }
            
            if (!aliveMonsters[i].isDead && Vector3.Distance(pos, aliveMonsters[i].transform.position) <= range)
            {
                list.Add(aliveMonsters[i]);
            }
        }
        return list;
    }
}








