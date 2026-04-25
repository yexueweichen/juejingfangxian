using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStand : MonoBehaviour
{
    [Header("攻击力")]
    public int atk = 10;

    [Header("攻击范围")]
    public float atkRange = 10f;

    [Header("攻击间隔")]
    public float fireInterval = 1f;

    [Header("子弹速度")]
    public float bulletSpeed = 5f;

    [Header("枪口位置")]
    public Transform gunPoint;

    private bool isActivated = false;
    private bool isPlayerNear = false;
    private Monster targetObj;
    private float offsetTime;
    private Collider towerCollider;

    void Start()
    {
        towerCollider = GetComponent<Collider>();
        if (towerCollider != null)
        {
            towerCollider.isTrigger = true;
        }
    }

    void Update()
    {
        if (!PoolMgr.Instance.HasPool("towerball"))
        {
            PoolMgr.Instance.RegisterPool<towerballPool>("towerball", 50);
        }

        if (isPlayerNear && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateTower();
        }

        if (!isActivated) return;

        // 目标无效时寻找新目标
        if (targetObj == null || targetObj.isDead)
        {
            Transform monsterTransform = GameLevelMgr.Instance.FindMonster(this.transform.position, (int)atkRange);
            targetObj = monsterTransform?.GetComponent<Monster>();
        }
        else
        {
            // 检查目标是否还在攻击范围内
            float distance = Vector3.Distance(transform.position, targetObj.transform.position);
            if (distance > atkRange)
            {
                // 目标超出范围，寻找新目标
                Transform monsterTransform = GameLevelMgr.Instance.FindMonster(this.transform.position, (int)atkRange);
                targetObj = monsterTransform?.GetComponent<Monster>();
            }
        }

        if (targetObj == null) return;

        // 计算朝向目标的方向（包含Y轴）
        Vector3 dir = targetObj.transform.position - transform.position;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (Time.time - offsetTime >= fireInterval)
        {
            Fire();
            offsetTime = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isPlayerNear = true;
            UIMgr.Instance.ShowPanel<TipUI>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (!isActivated)
            {
                UIMgr.Instance.HidePanel<TipUI>();
            }
        }
    }

    private void ActivateTower()
    {
        if (isActivated) return;

        if (PoolMgr.Instance != null && PoolMgr.Instance.HasPool("towerball"))
        {
            GameObject tempObj = PoolMgr.Instance.GetObj("towerball");
            if (tempObj != null)
            {
                PoolMgr.Instance.ReturnObj(tempObj, "towerball");
            }
        }

        isActivated = true;
        UIMgr.Instance.HidePanel<TipUI>();
        MusicMgr.Instance.PlaySound("Tower");
    }

    private void Fire()
    {
        if (targetObj == null || targetObj.isDead) return;
        if (gunPoint == null) return;

        MusicMgr.Instance.PlaySound("archer");

        if (PoolMgr.Instance != null && PoolMgr.Instance.HasPool("towerball"))
        {
            GameObject ballObj = PoolMgr.Instance.GetObj("towerball");
            if (ballObj != null)
            {
                // 计算从枪口到目标的方向
                Vector3 dir = (targetObj.transform.position - gunPoint.position).normalized;
                
                ballObj.transform.position = gunPoint.position;
                ballObj.transform.rotation = Quaternion.LookRotation(dir);

                TowerBall ball = ballObj.GetComponent<TowerBall>();
                if (ball != null)
                {
                    // 传递目标引用和攻击力
                    ball.InitInfo(atk, targetObj);
                }
            }
        }
    }
}
