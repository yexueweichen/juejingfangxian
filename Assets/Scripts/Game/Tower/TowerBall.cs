using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//防御塔子弹
public class TowerBall : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 10f;
    private int atk;
    private float lifeTime = 5f;
    private Monster target;
    private bool isActive = false;

    public void InitInfo(int attack, Monster targetMonster)
    {
        atk = attack;
        target = targetMonster;
        lifeTime = 5f;
        isActive = true;
        
        // 重置缩放
        transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        // 确保从池取出时状态正确
        isActive = false;
        target = null;
    }

    void Update()
    {
        if (!isActive) return;
        
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            ReturnToPool();
            return;
        }
        
        // 目标无效时立即回收
        if (target == null || target.isDead)
        {
            ReturnToPool();
            return;
        }
        
        // 追踪目标
        Vector3 dir = (target.transform.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            transform.forward = dir;
        }
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null && !monster.isDead)
            {
                monster.Wound(atk);
            }
            isActive = false;
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        isActive = false;
        if (PoolMgr.Instance != null && PoolMgr.Instance.HasPool("towerball"))
        {
            PoolMgr.Instance.ReturnObj(gameObject, "towerball");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
