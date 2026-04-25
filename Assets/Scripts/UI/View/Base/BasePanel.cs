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

    private bool isInitialized = false;

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
        if (!isInitialized)
        {
            Init();
            isInitialized = true;
        }
    }

    public virtual void ShowMe()
    {
        //确保初始化被调用
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        
        //确保 Init 被调用，即使 Start 还没执行
        if (!isInitialized)
        {
            Init();
            isInitialized = true;
        }
        
        canvasGroup.alpha = 0;
        isShowing = true;
    }

    public virtual void HideMe(UnityAction callback)
    {
        canvasGroup.alpha = 1;
        isShowing = false;
        callAction = callback;
    }

   public virtual void Update()
    {
      
        float dt = Time.unscaledDeltaTime;

        //如果isShowing为true，alph不等于零执行
        if (isShowing && canvasGroup.alpha != 1)
        {
           //增加透明度（使用 unscaledDeltaTime）
            canvasGroup.alpha += alphaSpeed * dt;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        //如果isShowing为false，alph不等于0执行
        else if (!isShowing && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= alphaSpeed * dt;
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