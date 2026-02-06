using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 继承MonoBehaviour的单例模式基类（自动挂载式 - 解决重复挂载问题）
/// </summary>
[DisallowMultipleComponent] // 防止同一个GameObject上挂载多个相同脚本
public class SingleAutoMonoMgr<T> : MonoBehaviour where T : SingleAutoMonoMgr<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试从场景中查找现有实例
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    // 如果没有，则创建一个新的GameObject并挂载脚本
                    GameObject obj = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(obj); // 确保在场景切换时不被销毁
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
           
        }
        else if (instance != this)
        {
            // 如果已经存在一个实例，并且当前实例不是那个，则销毁当前实例
            Destroy(gameObject);
        }
    }
}


