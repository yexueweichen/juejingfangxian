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
        
        // 如果目标还存在，追踪目标
        if (target != null && !target.isDead)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                transform.forward = dir;
            }
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 目标已死亡，按当前方向继续飞行
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
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
