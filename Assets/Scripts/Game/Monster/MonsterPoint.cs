using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterPoint : MonoBehaviour
{
    [Header("怪物总波次")]
    public int zongBoCi;
    [Header("每波怪物有多少只")]
    public int meiBoShuLiang;
    private int nowLessNum;

    [Header("怪物ID")]
    public List<int> monsterIDs;
    private int nowID;

    [Header("单只怪物创建间隔时间")]
    public float createOffsetTime;

    [Header("波与波之间的间隔时间")]
    public float delayTime;

    [Header("第一波怪物创建的开始时间")]
    public float firstDelayTime;

    private bool isDestroyed = false;


    async void Start()
    {
        GameLevelMgr.Instance.AddMonsterPoint(this);
        if (zongBoCi > 0)
        {
            await UniTask.Yield();
            GameLevelMgr.Instance.UpdateZongBoCi(zongBoCi);
        }
        await UniTask.Delay((int)(firstDelayTime * 1000));
        CreateBoCi();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
        CancelInvoke();
    }

    private void CreateBoCi()
    {
        if (isDestroyed) return;
        if (monsterIDs == null || monsterIDs.Count == 0)
        {
            Debug.LogError($"MonsterPoint.CreateBoCi: monsterIDs为空，无法创建怪物！");
            return;
        }

        nowID = monsterIDs[Random.Range(0, monsterIDs.Count)];
        nowLessNum = meiBoShuLiang;
        CreateMonster();
        --zongBoCi;
        GameLevelMgr.Instance.ChangeHaveBoCi(1);
    }

    private void CreateMonster()
    {
        if (isDestroyed) return;

        // 通过 id 查找 MonsterInfo
        MonsterInfos.MonsterInfo info = GetMonsterInfoById(nowID);
        if (info == null)
        {
            Debug.LogError($"找不到 MonsterInfo，id={nowID}");
            return;
        }

        ABMgr.Instance.LoadResAsync<GameObject>("modle", info.res, (obj) =>
        {
            if (obj == null || isDestroyed) return;

            GameObject monsterObj = Instantiate(obj, this.transform.position, Quaternion.identity);
            Monster monster = monsterObj.GetComponent<Monster>();
            if (monster == null)
                monster = monsterObj.AddComponent<Monster>();

            monster.MonsterInfoInit(info);
            GameLevelMgr.Instance.AddMonster(monster);
            GameLevelMgr.Instance.ChangeMonsterNum(1);

            --nowLessNum;
            if (nowLessNum == 0)
            {
                if (zongBoCi > 0 && !isDestroyed)
                    Invoke(nameof(CreateBoCi), delayTime);
            }
            else
            {
                if (!isDestroyed)
                    Invoke(nameof(CreateMonster), createOffsetTime);
            }
        }).Forget();
    }

    private MonsterInfos.MonsterInfo GetMonsterInfoById(int id)
    {
        var list = ConfigMgr.Instance?.monsterInfos?.MonsterDatas;
        if (list == null || list.Count == 0) return null;

        if (id > 0 && id <= list.Count && list[id - 1].id == id)
            return list[id - 1];

        if (id >= 0 && id < list.Count)
            return list[id];

        for (int i = 0; i < list.Count; i++)
            if (list[i].id == id) return list[i];

        return null;
    }

    public bool CheckOver()
    {
        return nowLessNum == 0 && zongBoCi == 0;
    }
}
