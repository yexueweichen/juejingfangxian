﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //玩家动画组件
    private Animator PlayerAnimator;
    //玩家角色控制器
    private CharacterController PlayerController;
    //血量（暂时没用到）
   // private int hp;
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
    //输入轴值
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerController = GetComponent<CharacterController>();
       
        InputMgr.Instance.StartOrCloseInputMgr(true);
        InitKeyCode();
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Shift, ShiftEvent);
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Space, SpaceEvent);
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_ShiftUp, ShiftUpEvent);
        EventCenter.Instance.AddEventListener(E_EventType.E_Mouse_0, Mouse0Event);
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Mouse_X, MouseXEvent);
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, HorizontalEvent);
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Vertical, VerticalEvent);
        MonoMgr.Instance.AddUpdateListener(HandleMovement);
    }

   
    /// <summary>
    /// 处理移动逻辑（每帧调用）
    /// </summary>
    private void HandleMovement()
    {
        //没有输入时归零动画参数
        if (horizontalInput == 0 && verticalInput == 0)
        {
            PlayerAnimator.SetFloat("Xspeed", 0);
            PlayerAnimator.SetFloat("Yspeed", 0);
            return;
        }

        //计算移动方向向量（组合前后和左右方向）
        Vector3 moveDir = transform.forward * verticalInput + transform.right * horizontalInput;
        
        //归一化防止斜向移动速度过快
        moveDir.Normalize();

        //更新动画参数
        PlayerAnimator.SetFloat("Xspeed", horizontalInput);
        PlayerAnimator.SetFloat("Yspeed", verticalInput);

        //移动
        PlayerController.SimpleMove(moveDir * moveSpeed );
    }
    /// <summary>
    /// 初始化玩家按键
    /// </summary>
    private void InitKeyCode()
    {
       InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Space, KeyCode.Space, InputInfo.E_InputType.Down);
       InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_ShiftUp, KeyCode.LeftShift, InputInfo.E_InputType.Up);
       InputMgr.Instance.ChangeMouseInfo(E_EventType.E_Mouse_0, 0, InputInfo.E_InputType.Down);
       InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Shift, KeyCode.LeftShift, InputInfo.E_InputType.Down);

    }
   
   private void ShiftEvent()
    {
       
      PlayerAnimator.SetLayerWeight(1, 1);
        
      
    }
    private void ShiftUpEvent()
    {
       
    PlayerAnimator.SetLayerWeight(1, 0);
        
    }
    private void MouseXEvent(float x)
    {
        float rotateAngle=x* aroundspeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateAngle);
    }
    /// <summary>
    /// 水平轴事件 - 只更新输入状态
    /// </summary>
    private void HorizontalEvent(float h)
    {
        horizontalInput = h;
    }
   
    /// <summary>
    /// 垂直轴事件 - 只更新输入状态
    /// </summary>
    private void VerticalEvent(float v)
    {
        verticalInput = v;
    }
    
    /// <summary>
    /// 鼠标0事件  射击
    /// </summary>
    private void Mouse0Event()
    {
     PlayerAnimator.SetTrigger("Fire"); 
    }
   
    /// <summary>
    /// 空格事件  翻滚
    /// </summary>
    private void SpaceEvent()
    {
         PlayerAnimator.SetTrigger("Roll"); 
    }
   
    //初始化玩家信息由GameLevelMgr调用
    public void InitPlayerInfo(int atk, int money)
    {
        this.atk=atk;
        this.money = money;
        //更新面板
        UPdateMoney();
    }
  

    //近战攻击动画事件
    public void KnifeEvent()
    {
   
   MathUtil.OverlapSphere<Monster>(transform.position+transform.forward+transform.up, 1,1<<LayerMask.NameToLayer("Monster"),(monster)=>
   {
       //得到碰撞到的对象上的怪物脚本 让其受伤
       monster.Wound(this.atk);
   });
  
    }

    //射击动画事件
    public void ShootEvent()
    {
      Ray ray = new(FirePoint.position, FirePoint.forward);
       MathUtil.RayCast<Monster>(ray, (monster) =>
        {
            //得到碰撞到的对象上的怪物脚本 让其受伤
            monster.Wound(this.atk);
            
        }, 100f, 1 << LayerMask.NameToLayer("Monster"));
      

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
        //获取已显示的面板并更新金钱
        GamePanel panel = UIMgr.Instance.GetPanel<GamePanel>();
        if (panel != null)
        {
            panel.UpdateMoney(money);
        }
    }

    //增加钱
    public void AddMoney(int money)
    {
        this.money += money;
        UPdateMoney();
    }

    //销毁时移除事件监听
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Shift, ShiftEvent);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Space, SpaceEvent);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_ShiftUp, ShiftUpEvent);
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Mouse_0, Mouse0Event);
        EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Mouse_X, MouseXEvent);
        EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Horizontal, HorizontalEvent);
        EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Vertical, VerticalEvent);
        MonoMgr.Instance.RemoveUpdateListener(HandleMovement);
    }

}