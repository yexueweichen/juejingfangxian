using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster: MonoBehaviour
{
    //怪物寻路agent
    private NavMeshAgent zombieAgent;
   //动画组件
    private Animator animator;
    private int hp;
    private int atk;
   //持有的怪物信息
    private MonsterInfo thisMonsterInfo;
    //是否死亡
    public bool isDead=false;
   //开始攻击的时间
    private float frontTime;


   void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    //初始化数据
    public void MonsterInfoInit(MonsterInfo info)
    {
     thisMonsterInfo=info;
        //加载动画控制器
        animator.runtimeAnimatorController= Resources.Load<RuntimeAnimatorController>(info.animator);
      hp = info.hp;
      atk = info.atk;
      zombieAgent.speed =zombieAgent.acceleration= info.moveSpeed;
      zombieAgent.angularSpeed = info.roundSpeed;

    }

    //受伤害
    public void Wound(int damage)
    {
        hp -= damage;

        // 如果已经死亡，不再播放受伤动画
        if (isDead) return;

        if (hp <= 0)
        {
            Dead(); // 直接播放死亡动画
        }
        else
        {
            animator.SetTrigger("Dmg"); // 仅在不死亡时播放受伤动画
        }
    }

    //死亡
    public void Dead()
    {
        if (isDead) return; // 防止重复调用

        isDead = true;
        zombieAgent.isStopped = true;

        // 重置其他动画状态，确保死亡动画能正确播放
        animator.ResetTrigger("Dmg");
        animator.SetBool("Run", false);
        animator.SetBool("Death", true);

        // 立即停止所有运动
        zombieAgent.velocity = Vector3.zero;
    }
    //死亡动画事件
    public void DeadEvent()
    {
        //死亡动画播放完毕后移除对象数量减一
        GameLevelMgr.Instance.ChangeMonsterNum(-1);
        //在场景中移除已经死亡的对象
        Destroy(this.gameObject,5);
        
        //怪物死亡时 游戏胜利
       if( GameLevelMgr.Instance.CheckOver())
        {
            UIMgr.Instance.ShowPanel<GameOverPanel>().
                InitInfo(GameLevelMgr.Instance.player.money,true);
        }



    }
    //出生动画事件
    public void BornOver()
    {
zombieAgent.SetDestination(MainTower.Instance.transform.position);
        animator.SetBool("Run", true);

    }

    
    void Update()
    {
        if (isDead)
            return;
        {
            animator.SetBool("Run", zombieAgent.velocity!=Vector3.zero);

            if (Vector3.Distance(this.transform.position, MainTower.Instance.transform.position) < 6&&
              Time.time - frontTime >= thisMonsterInfo.atkTime)
            {
                //记录这次攻击时的时间
                frontTime = Time.time;
                animator.SetTrigger("Atk");
            }

        }
    }

    //攻击动画事件
    public void AtkEvent()
    {
        //范围检测 进行伤害判断
        Collider[] colliders = Physics.OverlapSphere(this.transform.position + this.transform.forward + this.transform.up, 1, 1 << LayerMask.NameToLayer("MainTower"));
        for (int i = 0; i < colliders.Length; i++)
        {
            if (MainTower.Instance.gameObject == colliders[i].gameObject)
            {
                //让保护区域受到伤害
                MainTower.Instance.Wound(thisMonsterInfo.atk);
            }
        }
    }






}
