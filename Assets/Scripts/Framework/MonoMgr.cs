using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// 公共Mono模块管理器
/// 提供统一的Update、FixedUpdate、LateUpdate事件管理和协程管理
/// 让不继承MonoBehaviour的类也能使用Unity生命周期函数和协程功能
/// </summary>
public class MonoMgr : SingleAutoMonoMgr<MonoMgr>
{
    #region 字段

    /// <summary>
    /// Update事件委托
    /// </summary>
    private event UnityAction UpdateEvent;

    /// <summary>
    /// FixedUpdate事件委托
    /// </summary>
    private event UnityAction FixedUpdateEvent;

    /// <summary>
    /// LateUpdate事件委托
    /// </summary>
    private event UnityAction LateUpdateEvent;

    #endregion

    #region Update相关方法

    /// <summary>
    /// 添加Update帧更新事件监听器
    /// </summary>
    /// <param name="updateFun">要添加的Update更新函数</param>
    public void AddUpdateListener(UnityAction updateFun)
    {
        UpdateEvent += updateFun;
    }

    /// <summary>
    /// 移除Update帧更新事件监听器
    /// </summary>
    /// <param name="updateFun">要移除的Update更新函数</param>
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        UpdateEvent -= updateFun;
    }

    /// <summary>
    /// 添加FixedUpdate帧更新事件监听器
    /// </summary>
    /// <param name="updateFun">要添加的FixedUpdate更新函数</param>
    public void AddFixedUpdateListener(UnityAction updateFun)
    {
        FixedUpdateEvent += updateFun;
    }

    /// <summary>
    /// 移除FixedUpdate帧更新事件监听器
    /// </summary>
    /// <param name="updateFun">要移除的FixedUpdate更新函数</param>
    public void RemoveFixedUpdateListener(UnityAction updateFun)
    {
        FixedUpdateEvent -= updateFun;
    }

    /// <summary>
    /// 添加LateUpdate帧更新事件监听器
    /// </summary>
    /// <param name="updateFun">要添加的LateUpdate更新函数</param>
    public void AddLateUpdateListener(UnityAction updateFun)
    {
        LateUpdateEvent += updateFun;
    }

    /// <summary>
    /// 移除LateUpdate帧更新事件监听器
    /// </summary>
    /// <param name="updateFun">要移除的LateUpdate更新函数</param>
    public void RemoveLateUpdateListener(UnityAction updateFun)
    {
        LateUpdateEvent -= updateFun;
    }

    #endregion

    #region Unity生命周期

    private void Update()
    {
        UpdateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateEvent?.Invoke();
    }

    #endregion
}

