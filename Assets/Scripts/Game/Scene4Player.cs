using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene4Player : MonoBehaviour
{
    //玩家动画组件
    private Animator PlayerAnimator;
    //玩家角色控制器
    private UnityEngine.CharacterController NowPlayerController;
    //转向速度
    [SerializeField]
    private float aroundspeed = 100f;
    //移动速度
    [SerializeField]
    private float moveSpeed = 6f;
    //输入轴值
    private float horizontalInput;
    private float verticalInput;
    //脚步声音频源
    private AudioSource moveSoundSource;

    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        NowPlayerController = GetComponent<UnityEngine.CharacterController>();

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
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Mouse_X, MouseXEvent);
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, HorizontalEvent);
        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Vertical, VerticalEvent);
        MonoMgr.Instance.AddUpdateListener(HandleMovement);
    }

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


    private void InitKeyCode()
    {
        InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Space, KeyCode.Space, InputInfo.E_InputType.Down);
        InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_ShiftUp, KeyCode.LeftShift, InputInfo.E_InputType.Up);
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
        float rotateAngle = x * aroundspeed * Time.deltaTime;
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
    private void SpaceEvent()
    {
        PlayerAnimator.SetTrigger("Roll");
    }


    //销毁时移除事件监听
    private void OnDestroy()
    {
        if (EventCenter.Instance != null)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Shift, ShiftEvent);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_Space, SpaceEvent);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_Input_ShiftUp, ShiftUpEvent);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Mouse_X, MouseXEvent);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Horizontal, HorizontalEvent);
            EventCenter.Instance.RemoveEventListener<float>(E_EventType.E_Input_Vertical, VerticalEvent);
        }

        if (MonoMgr.Instance != null)
        {
            MonoMgr.Instance.RemoveUpdateListener(HandleMovement);
        }
    }
}
