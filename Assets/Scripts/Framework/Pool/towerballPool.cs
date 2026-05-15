using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class towerballPool : GameObjectPool
{
    private string abName;
    private string resName;
    private GameObject loadedPrefab;

    public towerballPool(string poolName, GameObject root, int maxNum = 100) : base(poolName, root, maxNum)
    {
        abName = "eff";
        resName = "towerball";
    }

    protected override GameObject LoadPrefab()
    {
        if (loadedPrefab != null)
        {
            return loadedPrefab;
        }

        // 同步加载预制体
        ABMgr.Instance.LoadResAsync<GameObject>(abName, resName, (obj) =>
        {
            loadedPrefab = obj;
        }, true).Forget();

               if (loadedPrefab == null)
        {
            Debug.LogError($"towerballPool: 预制体加载失败！abName={abName}, resName={resName}");
        }
        return loadedPrefab;
    }

    protected override void OnGetObject(GameObject obj)
    {
        // 重置旋转方向，确保子弹出栈时方向正确
        obj.transform.rotation = Quaternion.identity;
    }

    protected override void OnReturnObject(GameObject obj)
    {
        
    }
}
