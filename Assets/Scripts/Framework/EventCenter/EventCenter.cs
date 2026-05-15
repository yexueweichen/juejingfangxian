using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventInfoBase{ }

/// <summary>
/// 记录带参数的委托
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T>:EventInfoBase
{
    
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

/// <summary>
/// 记录无参无返回值委托
/// </summary>
public class EventInfo: EventInfoBase
{
    public UnityAction actions;
     
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// 事件中心模块 
/// </summary>
public class EventCenter: BaseManager<EventCenter>
{
    //用于记录对应事件 关联的 对应的逻辑
    private readonly Dictionary<E_EventType, EventInfoBase> eventDic = new();

    private EventCenter() { }

    /// <summary>
    /// 触发事件 
    /// </summary>
    /// <param name="eventName">事件名字</param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        //存在监听的事件通知别人去处理逻辑
        if(eventDic.ContainsKey(eventName))
        {
            EventInfo<T> eventInfo = eventDic[eventName] as EventInfo<T>;
            if (eventInfo != null && eventInfo.actions != null)
            {
                eventInfo.actions.Invoke(info);
            }
        }
    }

    /// <summary>
    /// 触发事件 无参数
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName)
    {
    
        if (eventDic.ContainsKey(eventName))
        {
            EventInfo eventInfo = eventDic[eventName] as EventInfo;
            if (eventInfo != null && eventInfo.actions != null)
            {
                eventInfo.actions.Invoke();
            }
        }
    }


    /// <summary>
    /// 添加事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        //如果已经存在关心事件的委托记录 直接添加即可
        if (eventDic.ContainsKey(eventName))
        {
            EventInfo<T> eventInfo = eventDic[eventName] as EventInfo<T>;
            if (eventInfo != null)
            {
                eventInfo.actions += func;
            }
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }

    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        
        if (eventDic.ContainsKey(eventName))
        {
            EventInfo eventInfo = eventDic[eventName] as EventInfo;
            if (eventInfo != null)
            {
                eventInfo.actions += func;
            }
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(func));
        }
    }

    /// <summary>
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            EventInfo<T> eventInfo = eventDic[eventName] as EventInfo<T>;
            if (eventInfo != null)
            {
                eventInfo.actions -= func;
            }
        }
    }

    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            EventInfo eventInfo = eventDic[eventName] as EventInfo;
            if (eventInfo != null)
            {
                eventInfo.actions -= func;
            }
        }
    }

    /// <summary>
    /// 清空所有事件的监听
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 清除指定某一个事件的所有监听
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }

}
