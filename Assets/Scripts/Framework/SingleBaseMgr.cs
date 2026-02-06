using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// 单例模式基类（解决构造函数唯一性问题和线程安全问题）
/// </summary>
public abstract class SingleBaseMgr<T> where T : class
{
    private static T instance;
    private static readonly object LockObject = new object(); // 用于线程同步的锁对象

    public static T Instance
    {
        get
        {
            // 双重检查锁定，确保线程安全
            if (instance == null)
            {
                lock (LockObject) // 加锁
                {
                    if (instance == null)
                    {
                        // 通过反射获取私有无参构造函数
                        ConstructorInfo constructor = typeof(T).GetConstructor(
                            BindingFlags.Instance | BindingFlags.NonPublic,
                            null,
                            Type.EmptyTypes,
                            null);

                        if (constructor == null)
                        {
                            throw new Exception($"Class {typeof(T).Name} must have a private parameterless constructor.");
                        }
                        instance = constructor.Invoke(null) as T;
                    }
                }
            }
            return instance;
        }
    }

    // 规定继承单例模式基类的类必须显示实现私有无参构造函数
    protected SingleBaseMgr() { }
}

