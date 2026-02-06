using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //玩家动画组件
    private Animator PlayerAnimator;
    //玩家角色控制器
    private CharacterController PlayerController;
    //血量（暂时没用到）
    private int hp;
    //攻击力
    private int atk;
    //钱
    public int money;
    //转向速度
    [SerializeField]
    private float aroundspeed;
   //移动速度
    [SerializeField]
    private float moveSpeed;
    //射线发射点
    public Transform FirePoint;

    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerController = GetComponent<CharacterController>();
    }

    //初始化玩家信息由GameLevelMgr调用
    public void InitPlayerInfo(int atk, int money)
    {
        this.atk=atk;
        this.money = money;
        //更新面板
        UPdateMoney();
    }
    void Update()
    {
       //移动
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
      
        Vector3 moveDir=transform.forward * vertical + transform.right * horizontal;
        moveDir.Normalize();

        PlayerAnimator.SetFloat("Xspeed", horizontal);
        PlayerAnimator.SetFloat("Yspeed", vertical);
        PlayerController.SimpleMove(moveDir*moveSpeed);
      
       //转头
        float rotateAngle=horizontal * aroundspeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateAngle);
        transform.Rotate(Input.GetAxis("Mouse X") * Vector3.up * aroundspeed * Time.deltaTime);

        //滚
        if (Input.GetKeyDown(KeyCode.Space))
        { PlayerAnimator.SetTrigger("Roll"); }
       //蹲
        if(Input.GetKeyDown(KeyCode.LeftShift))
        { 
       PlayerAnimator.SetLayerWeight(1, 1);

        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            PlayerAnimator.SetLayerWeight(1, 0);
        }

        //射击
        if (Input.GetMouseButtonDown(0))
        { PlayerAnimator.SetTrigger("Fire"); }



    }

    //近战攻击动画事件
    public void KnifeEvent()
    {
   Collider[] colliders=Physics.OverlapSphere(transform.position+transform.forward+transform.up, 1,1<<LayerMask.NameToLayer("Monster"));

        for (int i = 0; i < colliders.Length; i++)
        {
            //得到碰撞到的对象上的怪物脚本 让其受伤
            Monster monster = colliders[i].gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.Wound(this.atk);
                break;
            }
        }
    }

    //射击动画事件
    public void ShootEvent()
    {
      RaycastHit[] hits=  Physics.RaycastAll(new Ray(FirePoint.position,FirePoint.forward), 1000, 1 << LayerMask.NameToLayer("Monster"));

        for (int i = 0; i < hits.Length; i++)
        {
            //得到对象上的怪物脚本 让其受伤
            //得到碰撞到的对象上的怪物脚本 让其受伤
            Monster monster = hits[i].collider.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                monster.Wound(this.atk);
                break;
            }
        }

    }
    //受伤(暂时没用)
    //public void TakeDamage(int damage)
   // {
       // health -= damage;
      //  if(health <= 0)
      //  {
       //     Die();
      //  }
   // }

    //更新面板钱数
    public void UPdateMoney()
    {
        //调用UIMgr的更新方法传入玩家钱数
        UIMgr.Instance.ShowPanel<GamePanel>().UpdateMoney(money);
    }

    //增加钱
    public void AddMoney(int money)
    {
        this.money += money;
        UPdateMoney();
    }



}