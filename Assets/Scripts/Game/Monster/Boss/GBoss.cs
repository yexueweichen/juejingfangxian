using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GBoss : MonoBehaviour
{
// 怪物召唤位置
public List<Transform> Parts;
// 技能冷却时间
public float skillOffset = 0f;

// 当前召唤的怪物信息
public MonsterInfos.MonsterInfo nowmonsterInfo;
public Monster bossObj;
public string bossSummonName;

void Awake()
{
   bossObj = this.GetComponent<Monster>();
    MusicMgr.Instance.PlayBKMusic("BossMusic");

// 获取怪物信息
    if (ConfigMgr.Instance != null && ConfigMgr.Instance.monsterInfos != null && ConfigMgr.Instance.monsterInfos.MonsterDatas.Count > 6)
    {
        nowmonsterInfo = ConfigMgr.Instance.monsterInfos.MonsterDatas[6];
    }
}

    void Update()
    {
        skillOffset += Time.deltaTime;
          if(skillOffset >= 15f)
          {
            skillOffset = 0f;
           
            Summon().Forget();
          }
     }


    // 召唤怪物的方法
    private async UniTask Summon()
    {
        if (Parts == null || Parts.Count == 0)
        {
            Debug.LogWarning("Parts未配置或为空，无法召唤怪物。");
            return;
        }

        // 获取召唤怪物名称
        if (bossObj != null && bossObj.thisMonsterInfo != null)
        {
            bossSummonName = bossObj.thisMonsterInfo.BossSummonObj;
        }

        if (string.IsNullOrEmpty(bossSummonName))
        {
            Debug.LogWarning("bossSummonName为空，无法召唤怪物。");
            return;
        }

        // 同步加载预制体
        GameObject prefab = null;
        await ABMgr.Instance.LoadResAsync<GameObject>("modle", bossSummonName, (m) => { prefab = m; }, true);

        if (prefab == null)
        {
            Debug.LogError("加载预制体失败Gaint01");
            return;
        }

        foreach (var part in Parts)
        {
            if (part == null) continue;

            GameObject inst = Instantiate(prefab, part.position, Quaternion.identity);
            if (inst == null) continue;

            Monster monster = inst.GetComponent<Monster>();
            if (monster == null)
            {
                monster = inst.AddComponent<Monster>();
            }

            if (nowmonsterInfo != null)
            {
                monster.MonsterInfoInit(nowmonsterInfo);
            }
            else
            {
                Debug.LogWarning(" nowmonsterInfo 为 null，未初始化怪物属性。");
            }

            GameLevelMgr.Instance.ChangeMonsterNum(1);
            GameLevelMgr.Instance.AddMonster(monster);
        }
    }

    void OnDestroy()
    {
        if (MusicMgr.Instance != null)
        {
            MusicMgr.Instance.PlayBKMusic("BKMusic4");
        }
    }
}
