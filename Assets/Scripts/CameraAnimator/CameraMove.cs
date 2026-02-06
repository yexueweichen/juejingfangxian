using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraMove : MonoBehaviour
{
   Animator animator;
    private UnityAction callAction;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

   public void Turnleft(UnityAction call)
    {
        animator.SetTrigger("left");
        callAction = call;
    }

    public void Turnright(UnityAction call)
     {
          animator.SetTrigger("right");
          callAction = call;
    }

    public void OnShow()
    {
               callAction?.Invoke();
               callAction = null;
    }
}
