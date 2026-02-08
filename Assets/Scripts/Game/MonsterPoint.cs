using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPoint : MonoBehaviour
{
    //怪物总波次
    public int zongBoCi;
    //每波怪物有多少只
    public int meiBoShuLiang;
    //用于记录 当前波的怪物还有多少只没有创建
    private int nowLessNum;

    //怪物ID 允许有多个 这样就可以随机创建不同的怪物 更具多样性
    public List<int> monsterIDs;
    //用于记录 当前波 要创建什么ID的怪物
    private int nowID;

    //单只怪物创建间隔时间
    public float createOffsetTime;

    //波与波之间的间隔时间
    public float delayTime;

    //第一波怪物创建的开始时间
    public float firstDelayTime;
    void Start()
    {
        Invoke("CreateBoCi", firstDelayTime);
        GameLevelMgr.Instance.AddMonsterPoint(this);
        //更新最大波数
        GameLevelMgr.Instance.UpdateZongBoCi(zongBoCi);
    }

    private void CreateBoCi()
    {
        //得到当前波怪物的ID是什么
        nowID = monsterIDs[Random.Range(0, monsterIDs.Count)];
        //当前波怪物有多少只
        nowLessNum = meiBoShuLiang;
        //创建怪物
        CreateMonster();
        //减少波数
        --zongBoCi;
        GameLevelMgr.Instance.ChangeHaveBoCi(1);
    }

private void CreateMonster()
{
    //直接创建怪物
    //取出怪物数据
    MonsterInfo info = GameDataMgr.Instance.monsterInfos[nowID];

    //创建怪物预设体
    GameObject obj = Instantiate(Resources.Load<GameObject>(info.res), this.transform.position, Quaternion.identity);
    //为我们创建出的怪物预设体 添加怪物脚本 进行初始化
    Monster monsterObj = obj.AddComponent<Monster>();
    monsterObj.MonsterInfoInit(info);
        GameLevelMgr.Instance.ChangeMonsterNum(1);
        //创建完一只怪物后 减去要创建的怪物数量1
        --nowLessNum;
    if (nowLessNum == 0)
    {
        if (zongBoCi> 0)
            Invoke("CreateBoCi", delayTime);
    }
    else
    {
        Invoke("CreateMonster", createOffsetTime);
    }
}

/// <summary>
/// 出怪点是否出怪结束
/// </summary>
/// <returns></returns>
public bool CheckOver()
{
    return nowLessNum == 0 && zongBoCi== 0;
}


}
