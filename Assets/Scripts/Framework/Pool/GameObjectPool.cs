using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject对象池抽象基类
/// </summary>
public abstract class GameObjectPool
{
    /// <summary>
    /// 冷对象栈
    /// </summary>
    protected Stack<GameObject> _unuseStack = new Stack<GameObject>();

    /// <summary>
    /// 热对象链表
    /// </summary>
    protected LinkedList<GameObject> _usedList = new LinkedList<GameObject>();

    /// <summary>
    /// 对象到链表节点的映射
    /// </summary>
    protected Dictionary<GameObject, LinkedListNode<GameObject>> _gameObj2NodeDict = new Dictionary<GameObject, LinkedListNode<GameObject>>();

    /// <summary>
    /// 对象池上限，场景上同时存在的对象的最大个数
    /// </summary>
    protected readonly int MaxNum;

    /// <summary>
    /// 抽屉根对象，用于进行布局管理的对象
    /// </summary>
    protected GameObject _rootObj;

    /// <summary>
    /// 对象池名称
    /// </summary>
    protected string _poolName;

    /// <summary>
    /// 对象预制体
    /// </summary>
    protected GameObject _prefab;

    #region 属性

    /// <summary>
    /// 获取容器中未使用对象的数量
    /// </summary>
    public int UnuseCount => _unuseStack.Count;

    /// <summary>
    /// 获取正在使用中的对象数量
    /// </summary>
    public int UsedCount => _usedList.Count;

    /// <summary>
    /// 获取对象池名称
    /// </summary>
    public string PoolName => _poolName;

    /// <summary>
    /// 判断是否需要创建新对象
    /// </summary>
    public bool NeedCreate => UnuseCount == 0 && _usedList.Count < MaxNum;

    #endregion

    #region 构造函数

    /// <summary>
    /// 初始化构造函数
    /// </summary>
    /// <param name="poolName">对象池名称</param>
    /// <param name="root">缓存池父对象</param>
    /// <param name="maxNum">最大对象数量，默认100</param>
    protected GameObjectPool(string poolName, GameObject root, int maxNum = 200)
    {
        _poolName = poolName;
        _rootObj = root;
        MaxNum = maxNum;
    }

    #endregion

    #region 抽象方法

    /// <summary>
    /// 加载GameObject预制体
    /// </summary>
    /// <returns>加载的GameObject预制体</returns>
    protected abstract GameObject LoadPrefab();

    #endregion

    #region 虚方法

    /// <summary>
    /// 从对象池中取出对象时的初始化
    /// 子类可以重写此方法来执行自定义初始化
    /// </summary>
    /// <param name="obj">处理取出的GameObject对象</param>
    protected virtual void OnGetObject(GameObject obj)
    {

    }

    /// <summary>
    /// 将对象放回对象池时的清理
    /// 子类可以重写此方法来执行自定义清理
    /// </summary>
    /// <param name="obj">处理放回的GameObject对象</param>
    protected virtual void OnReturnObject(GameObject obj)
    {

    }

    /// <summary>
    /// 对象被抢占时的回调
    /// 当池满时，会抢占最早使用的对象
    /// </summary>
    /// <param name="obj">处理被抢占的对象</param>
    protected virtual void OnActiveObject(GameObject obj)
    {

    }

    #endregion


    /// <summary>
    /// 从对象池中取出对象
    /// </summary>
    /// <returns>取出的GameObject对象</returns>
    public GameObject GetObj()
    {
        GameObject obj;

        if (UnuseCount > 0)
        {
            obj = _unuseStack.Pop();
        }
        else if (_usedList.Count < MaxNum)
        {
            obj = CreateNewObject();
        }
        else
        {
            obj = _usedList.First.Value;
            _usedList.RemoveFirst();
            _gameObj2NodeDict.Remove(obj);
            OnActiveObject(obj);
        }

        LinkedListNode<GameObject> node = _usedList.AddLast(obj);
        _gameObj2NodeDict[obj] = node;

        // 激活对象
        obj.SetActive(true);
        // 断开父子关系
        if (PoolMgr.isOpenLayout)
        {
            obj.transform.SetParent(null);
        }

        // 执行初始化
        OnGetObject(obj);

        return obj;
    }

    /// <summary>
    /// 将对象归还到对象池中
    /// </summary>
    /// <param name="obj">要归还的对象</param>
    public void ReturnObj(GameObject obj)
    {
        if (obj == null)
            return;

        OnReturnObject(obj);

        // 失活对象
        obj.SetActive(false);
        // 放入对应抽屉的根物体中，建立父子关系
        if (PoolMgr.isOpenLayout && _rootObj != null)
        {
            obj.transform.SetParent(_rootObj.transform);
        }

        // 从使用列表中移除
        if (_gameObj2NodeDict.TryGetValue(obj, out LinkedListNode<GameObject> node))
        {
            _usedList.Remove(node);
            _gameObj2NodeDict.Remove(obj);
        }

        // 压入未使用栈（LIFO）
        _unuseStack.Push(obj);
    }

    /// <summary>
    /// 创建新对象
    /// </summary>
    /// <returns>创建的对象</returns>
    protected GameObject CreateNewObject()
    {
        if (_prefab == null)
        {
            _prefab = LoadPrefab();
        }

        if (_prefab == null)
        {
            Debug.LogError($"预制体加载失败！poolName: {_poolName}");
            return null;
        }

        GameObject obj = UnityEngine.GameObject.Instantiate(_prefab);
        obj.name = _poolName;
        return obj;
    }


    /// <summary>
    /// 清除对象池中的所有对象
    /// </summary>
    public void Clear()
    {
        foreach (var obj in _unuseStack)
        {
            if (obj != null) UnityEngine.GameObject.Destroy(obj);
        }
        _unuseStack.Clear();

        foreach (var obj in _usedList)
        {
            if (obj != null) UnityEngine.GameObject.Destroy(obj);
        }
        _usedList.Clear();
        _gameObj2NodeDict.Clear();
    }
}