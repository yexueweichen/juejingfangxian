using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
   
    private CanvasGroup canvasGroup;
    //alpha值改变速度
    public float alphaSpeed = 2f;
   //是否显示
    public bool isShowing = false;
    private UnityAction callAction;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

    }

    protected abstract void Init();
    protected virtual void Start()
    {
        Init();
    }

    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;
        isShowing = true;
    }

    public virtual void HideMe(UnityAction callback)
    {
        canvasGroup.alpha = 1;
        isShowing = false;
        callAction = callback;
    }

    void Update()
    {
        //如果isShowing为true，alph不等于零执行
        if (isShowing && canvasGroup.alpha != 1)
        {
           //增加透明度
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        //如果isShowing为false，alph不等于0执行
        else if (!isShowing && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //执行回调函数
                callAction?.Invoke();
                callAction = null;
            }


        }



    }
}