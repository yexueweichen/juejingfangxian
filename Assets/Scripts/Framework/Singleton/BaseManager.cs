using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T:class
{
    private static T instance;

    //判断单例模式对象 是否为null
    protected bool InstanceisNull => instance == null;

    //用于加锁的对象
    protected static readonly object lockObj = new object();

    //属性的方式
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        //反射得到无参私有的构造函数
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                    null,
                                                                    Type.EmptyTypes,
                                                                    null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("没有得到对应的无参构造函数");

                        
                    }
                }
            }
            return instance;
        }
    }
}
