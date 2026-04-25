using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject对象池管理器
/// 统一管理所有GameObject类型对象池
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    private Dictionary<string, GameObjectPool> _poolDict = new Dictionary<string, GameObjectPool>();
    private GameObject _rootObj;
    public static bool isOpenLayout = true;

    private PoolMgr()
    {
        if (isOpenLayout)
        {
            _rootObj = new GameObject("GameObjPoolRoot");
            UnityEngine.Object.DontDestroyOnLoad(_rootObj);
        }
    }

    public GameObject GetObj(string poolName)
    {
        if (!_poolDict.ContainsKey(poolName))
        {
            Debug.LogError($"对象池 '{poolName}' 不存在！");
            return null;
        }
        return _poolDict[poolName].GetObj();
    }

    public void ReturnObj(GameObject obj)
    {
        if (obj == null) return;

        string poolName = obj.name;
        if (!_poolDict.ContainsKey(poolName))
        {
            Debug.LogWarning($"对象池 '{poolName}' 不存在，对象将被销毁。");
            UnityEngine.Object.Destroy(obj);
            return;
        }
        _poolDict[poolName].ReturnObj(obj);
    }

    public void ReturnObj(GameObject obj, string poolName)
    {
        if (obj == null) return;

        if (!_poolDict.ContainsKey(poolName))
        {
            Debug.LogError($"PoolMgr.ReturnObj: 对象池 '{poolName}' 不存在！");
            UnityEngine.Object.Destroy(obj);
            return;
        }
        _poolDict[poolName].ReturnObj(obj);
    }

    /// <summary>
    /// 注册一个新的对象池
    /// </summary>
    /// <param name="pool">要注册的对象池实例</param>
    /// <remarks>
    /// 如果对象池已存在，将被替换。
    /// 如果对象池不存在，将被添加到字典中。
    /// </remarks>
    public void RegisterPool(GameObjectPool pool)
    {
        if (pool == null)
        {
            Debug.LogError("对象池不能为空！");
            return;
        }

        if (_poolDict.ContainsKey(pool.PoolName))
        {
            Debug.LogWarning($"对象池 '{pool.PoolName}' 已存在，将被替换。");
            _poolDict[pool.PoolName] = pool;
        }
        else
        {
            _poolDict.Add(pool.PoolName, pool);
        }
    }

    /// <summary>
    /// 注册一个新的对象池
    /// </summary>
    /// <typeparam name="T">对象池类型，必须继承自GameObjectPool</typeparam>
    /// <param name="poolName">对象池名称</param>
    /// <param name="maxNum">对象池最大对象数量，默认100</param>
    /// <returns>注册的对象池实例</returns>
    public T RegisterPool<T>(string poolName, int maxNum = 100) where T : GameObjectPool
    {
        if (_poolDict.ContainsKey(poolName))
        {
            return _poolDict[poolName] as T;
        }

        T pool = Activator.CreateInstance(typeof(T), poolName, _rootObj, maxNum) as T;
        _poolDict.Add(poolName, pool);
        return pool;
    }

    /// <summary>
    /// 检查是否存在指定名称的对象池
    /// </summary>
    /// <param name="poolName">对象池名称</param>
    /// <returns>如果存在则返回true，否则返回false</returns>
    public bool HasPool(string poolName)
    {
        return _poolDict.ContainsKey(poolName);
    }

    /// <summary>
    /// 清除所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in _poolDict.Values)
        {
            pool.Clear();
        }
        _poolDict.Clear();

        if (_rootObj != null)
        {
            UnityEngine.Object.Destroy(_rootObj);
            _rootObj = null;
        }
    }

}
