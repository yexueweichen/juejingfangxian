using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameLevelMgr:BaseManager<GameLevelMgr>

{
    private GameLevelMgr()
    {
    }

    public Player player;

    //所有的出怪点
    private List<MonsterPoint> points = new List<MonsterPoint>();
    //记录当前 还有多少波怪物
    private int haveBoCi = 0;
    //记录 一共有多少波怪物
    private int ZongBoCi = 0;

    //记录当前场景上的怪物数量
    private int nowMonsterNum = 0;


    //初始化游戏场景
    public void InitInfo(SceneInfos info)
    {
        //显示游戏界面
        UIMgr.Instance.ShowPanel<GamePanel>();

        //玩家创建
        //获取玩家选择时记录的当前选中的玩家数据
        RoleInfos.RoleData roleInfo = ConfigMgr.Instance.roleSeInfo;
        //获取到场景当中 玩家的出生位置
        Transform heroPos = GameObject.Find("HeroBornPos").transform;
        //实例化玩家预设体 然后把它的位置角度 设置为 场景当中出生点一致
        GameObject heroObj = GameObject.Instantiate(Resources.Load<GameObject>(roleInfo.res), heroPos.position, heroPos.rotation);
        //对玩家对象进行初始化
        player = heroObj.GetComponent<Player>();
        //初始化玩家的基础属性
    //    player.InitPlayerInfo(roleInfo.atk, info.money);

        //让摄像机 看向动态创建出来的玩家
        Camera.main.GetComponent<CameraFollow>().SetTarget(heroObj.transform);

        //初始化 中央 保护区域的血量
     //   MainTower.Instance.UpdateHp(info.towerHp, info.towerHp);
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
        haveBoCi= ZongBoCi;
        //更新界面
        UIMgr.Instance.GetPanel<GamePanel>().UpdateEnemy(haveBoCi, ZongBoCi);
    }

    //改变当前还有多少波怪
    public void ChangeHaveBoCi(int num)
    {
       haveBoCi -= num;
        //更新界面
        UIMgr.Instance.GetPanel<GamePanel>().UpdateEnemy(haveBoCi, ZongBoCi);
    }

    //检查游戏是否胜利
    public bool CheckOver()
    {
        for (int i = 0; i < points.Count; i++)
        {
            //只要有一个出怪点 还没有出完怪  那么就证明还没有胜利
            if (!points[i].CheckOver())
                return false;
        }

        if (nowMonsterNum > 0)
            return false;

        Debug.Log("游戏胜利");
        return true; }


     public void ChangeMonsterNum(int num)
    {
        nowMonsterNum += num;
    }


    public void ClearInfo()
    {
        points.Clear();
        ZongBoCi=haveBoCi=nowMonsterNum= 0;
        player = null;

    }


}








