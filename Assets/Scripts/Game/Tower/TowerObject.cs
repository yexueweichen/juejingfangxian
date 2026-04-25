using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    public Transform head;
    public Transform gunPoint;
    [SerializeField]
    private float roundSpeed = 20;
    private TowerInfos.TowerInfo info;
    private Monster targetObj;
    private List<Monster> targetObjs;
    private float offsetTime;
    private Vector3 monsterPos;

    public void InitInfo(TowerInfos.TowerInfo info)
    {
        this.info = info;
    }

    void Update()
    {
        if (info == null) return;

        if (info.type == 1)
        {
            if (targetObj == null || targetObj.isDead || Vector3.Distance(this.transform.position, targetObj.transform.position) > info.atkRange)
            {
                Transform monsterTransform = GameLevelMgr.Instance.FindMonster(this.transform.position, info.atkRange);
                targetObj = monsterTransform?.GetComponent<Monster>();
            }

            if (targetObj == null) return;

            monsterPos = targetObj.transform.position;
            monsterPos.y = head.position.y;
            head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(monsterPos - head.position), roundSpeed * Time.deltaTime);

            if (Vector3.Angle(head.forward, monsterPos - head.position) < 5 && Time.time - offsetTime >= info.offsetTime)
            {
                MusicMgr.Instance.PlaySound("Tower");

                
                Monster currentTarget = targetObj;
                Vector3 spawnPos = gunPoint.position;

                ABMgr.Instance.LoadResAsync<GameObject>("eff", info.eff, (effObj) =>
                {
                    if (currentTarget == null || currentTarget.isDead) return;
                    if (effObj == null) return;
                    
                    Vector3 dir = (currentTarget.transform.position - spawnPos).normalized;
                    GameObject eff = Instantiate(effObj, spawnPos, Quaternion.LookRotation(dir));
                    eff.transform.forward = dir;
                    Destroy(eff, 0.2f);
                    currentTarget.Wound(info.atk);
                }).Forget();

                offsetTime = Time.time;
            }
        }
        else
        {
            targetObjs = GameLevelMgr.Instance.FindMonsters(this.transform.position, info.atkRange);

            if (targetObjs == null || targetObjs.Count == 0) return;

            monsterPos = targetObjs[0].transform.position;
            monsterPos.y = head.position.y;
            head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(monsterPos - head.position), roundSpeed * Time.deltaTime);

            if (targetObjs.Count > 0 && Time.time - offsetTime >= info.offsetTime)
            {
                MusicMgr.Instance.PlaySound("Tower");
                
                Vector3 gunPos = gunPoint.position;
                ABMgr.Instance.LoadResAsync<GameObject>("eff", info.eff, (effObj) =>
                {
                    if (effObj == null) return;
                    GameObject eff = Instantiate(effObj, gunPos, Quaternion.identity);
                    Destroy(eff, 0.2f);
                }).Forget();

                for (int i = 0; i < targetObjs.Count; i++)
                {
                    if (targetObjs[i] != null && !targetObjs[i].isDead)
                    {
                        targetObjs[i].Wound(info.atk);
                        
                        Monster hitTarget = targetObjs[i];
                        Vector3 hitPos = hitTarget.transform.position;
                        ABMgr.Instance.LoadResAsync<GameObject>("eff", info.hitEff, (hitEffObj) =>
                        {
                            if (hitTarget == null || hitTarget.isDead) return;
                            if (hitEffObj == null) return;
                            GameObject hitEff = Instantiate(hitEffObj, hitPos + Vector3.up * 0.5f, Quaternion.identity);
                            Destroy(hitEff, 1f);
                        }).Forget();
                    }
                }

                offsetTime = Time.time;
            }
        }
    }
}

