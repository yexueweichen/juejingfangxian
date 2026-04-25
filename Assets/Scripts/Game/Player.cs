using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    //玩家动画组件
    private Animator PlayerAnimator;
    //玩家角色控制器
    private UnityEngine.CharacterController NowPlayerController;
    //攻击力
    private int atk;
    //生命值
    private int hp = 100;
    //转向速度
    [SerializeField]
    private float aroundspeed = 100f;
    //移动速度
    [SerializeField]
    private float moveSpeed = 6f;
    //射线发射点
    public Transform FirePoint;

    public Transform effPoint;
    //输入轴值
    private float horizontalInput;
    private float verticalInput;

    public RoleInfos.RoleData nowRoleInfo;

    public float offestTime=5f;
    public float atkOffestTime=2f;

    public float velocity = 0;
    public float rollOffestTime=3f;

    private bool isDead=false;

    public float DurationTime = 1.5f;

    private AudioSource moveSoundSource;



    void Awake()
{
    nowRoleInfo=ConfigMgr.Instance.roleSeInfo;
    atk=nowRoleInfo.atk;
    hp=nowRoleInfo.hp;
    moveSpeed=nowRoleInfo.moveSpeed;
    velocity=nowRoleInfo.Velocity;
}


 void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        NowPlayerController = GetComponent<UnityEngine.CharacterController>();
       
         //创建射线发射点
         GameObject firePointObj = new GameObject("FirePoint");
         firePointObj.transform.SetParent(this.transform);
         firePointObj.transform.localPosition = new Vector3((float)0.1f, (float)1.6f, 0.5f);
         firePointObj.transform.localRotation = Quaternion.identity;
         FirePoint = firePointObj.transform;

        if (PlayerAnimator == null)
        {
            Debug.LogError("Animator 组件缺失！");
        }
        if (NowPlayerController == null)
        {
            Debug.LogError("CharacterController 组件缺失！");
        }
       
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
        MonoMgr.Instance.AddUpdateListener(AtkOffsetEvent);
        MonoMgr.Instance.AddUpdateListener(RollOffestEvent);
    }

//处理玩家移动
    private void HandleMovement()
    {
        if (PlayerAnimator == null || NowPlayerController == null)
        {
            return;
        }
        
        if (horizontalInput == 0 && verticalInput == 0)
        {
            PlayerAnimator.SetFloat("Xspeed", 0);
            PlayerAnimator.SetFloat("Yspeed", 0);
            if (moveSoundSource != null && moveSoundSource.isPlaying)
            {
                MusicMgr.Instance.StopSound(moveSoundSource);
                moveSoundSource = null;
            }
            return;
        }

        Vector3 moveDir = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDir.Normalize();
        PlayerAnimator.SetFloat("Xspeed", horizontalInput);
        PlayerAnimator.SetFloat("Yspeed", verticalInput);
        
        if (moveSoundSource == null || !moveSoundSource.isPlaying)
        {
            MusicMgr.Instance.PlaySound("Move", false, false, (source) =>
            {
                moveSoundSource = source;
            });
        }
        
        NowPlayerController.SimpleMove(moveDir * moveSpeed);
    }

//初始化按键输入
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
    private void HorizontalEvent(float h)
    {
        horizontalInput = h;
    }
    private void VerticalEvent(float v)
    {
        verticalInput = v;
    }
    private void Mouse0Event()
    {
    
    if(atkOffestTime>nowRoleInfo.offsetTime)
    {
      PlayerAnimator.SetTrigger("Fire"); 
    }
    else
    {
        return;
    }
    
    }
    private void SpaceEvent()
    {
        if(rollOffestTime>=3f)
        PlayerAnimator.SetTrigger("Roll"); 
        rollOffestTime=0;
        DurationTime=1.5f;
        moveSpeed = nowRoleInfo.moveSpeed * velocity;
        

    }
    public void Wound(int damage)
    {
       if(isDead) return;
        
        hp -= damage;
        UIMgr.Instance.GetPanel<GamePanel>().UpdatePlayerHp(hp, nowRoleInfo.hp);
        PlayerAnimator.SetTrigger("Wound");
        MusicMgr.Instance.PlaySound(nowRoleInfo.hurtSound);
        if (hp <= 0) 
        {
            hp = 0;
            isDead=true;
        PlayerAnimator.SetTrigger("Death");
        MusicMgr.Instance.PlaySound(nowRoleInfo.deathSound);
        NowPlayerController.enabled = false;
        
        if (QuestMgr.Instance != null)
        {
            QuestMgr.Instance.NotifyLevelFailed();
        }
        
         UIMgr.Instance.ShowPanel<GameOverPanel>((panel) =>
            {
                panel.InitInfo((int)(SaveMgr.Instance.playerData.award * 0.5f), false);
            });}
    }

    public void KnifeEvent()
    {
       MathUtil.OverlapSphere<Monster>(transform.position+transform.forward+transform.up, 1,1<<LayerMask.NameToLayer("Monster"),(monster)=>
       {
           MusicMgr.Instance.PlaySound(nowRoleInfo.hitmusic);
           monster.Wound(this.atk);
       });
    }

public void AtkOffsetEvent()
{
    atkOffestTime += Time.deltaTime;

}

public void RollOffestEvent()
{ 
rollOffestTime += Time.deltaTime;

}

    private void Update()
    {
        if(rollOffestTime<3f)
        {
            DurationTime -= Time.deltaTime;
        }
        if (DurationTime <= 0)
        {
            moveSpeed = nowRoleInfo.moveSpeed;
        }
    }

    public void ShootEvent()
    {
          atkOffestTime = 0;
        if (FirePoint == null)
        {
            Debug.LogError("空");
            return;
        }
        
        Ray ray = new Ray(FirePoint.position, FirePoint.forward);
      
      if(nowRoleInfo.id!=5)
      {
       MathUtil.RayCast<Monster>(ray, (monster) =>
        {
            monster.Wound(this.atk);
        }, 100f, 1 << LayerMask.NameToLayer("Monster"));
        MusicMgr.Instance.PlaySound(nowRoleInfo.hitmusic);
        if (effPoint != null)
        {
            ABMgr.Instance.LoadResAsync<GameObject>("eff",nowRoleInfo.hitEff, (bullet) =>
            {
              GameObject fire1= Instantiate(bullet, effPoint.position, effPoint.rotation);
               Destroy(fire1, 0.2f);
            }).Forget();
        }
      }
       else
       {
        //终极英雄攻击射线检测到的所有怪物
         MathUtil.RayCastAll<Monster>(ray, (monster) =>
        {
            monster.Wound(this.atk);
        }, 50f, 1 << LayerMask.NameToLayer("Monster"));
        MusicMgr.Instance.PlaySound(nowRoleInfo.hitmusic);
        if (effPoint != null)
        {
            ABMgr.Instance.LoadResAsync<GameObject>("eff",nowRoleInfo.hitEff, (bullet) =>
            {
              GameObject fire1= Instantiate(bullet, effPoint.position, effPoint.rotation);
               Destroy(fire1, 0.2f);
            }).Forget();
        }


       }
       
    }

    //销毁时移除事件监听
    private void OnDestroy()
    {
        if (EventCenter.Instance != null)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Shift, ShiftEvent);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Space, SpaceEvent);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_ShiftUp, ShiftUpEvent);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Mouse_0, Mouse0Event);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Mouse_X, MouseXEvent);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Horizontal, HorizontalEvent);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Vertical, VerticalEvent);
        }

        if (MonoMgr.Instance != null)
        {
            MonoMgr.Instance.RemoveUpdateListener(HandleMovement);
            MonoMgr.Instance.RemoveUpdateListener(AtkOffsetEvent);
            MonoMgr.Instance.RemoveUpdateListener(RollOffestEvent);
        }
    }
    
    }
