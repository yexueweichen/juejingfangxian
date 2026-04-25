using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AB包管理器
/// </summary>
public class ABMgr : SingletonAutoMono<ABMgr>
{
    //主包
    private AssetBundle mainAB = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    private readonly Dictionary<string, AssetBundle> abDic = new ();

    /// <summary>
    /// 获取AB包加载路径
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {

            return "AB";
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainAB()
    {
        if( mainAB == null )
        {
            mainAB = AssetBundle.LoadFromFile( PathUrl + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">AB包名</param>
    /// <param name="resName">资源名</param>
    /// <param name="callBack">加载完成回调</param>
    /// <param name="isSync">是否同步加载</param>
    public async UniTask LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T:Object
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载过该AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if(isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    await req;
                    //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                    abDic[strs[i]] = req.assetBundle;
                }
            }
            //就证明 字典中已经记录了一个AB包相关信息了
            else
            {
                //如果字典中记录的信息是null 那就证明正在加载中
                while (abDic[strs[i]] == null)
                {
                    
                    await UniTask.Yield();
                }
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                await req;
                //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                abDic[abName] = req.assetBundle;
            }
        }
        else
        {
           
            while (abDic[abName] == null)
            {
                await UniTask.Yield();
            }
        }

        //同步加载AB包中的资源
        if(isSync)
        {
            //即使是同步加载 也需要使用回调函数传给外部进行使用
            T res = abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        //异步加载包中资源
        else
        {
            AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
            await abq;

            callBack(abq.asset as T);
        }
    }

    //卸载指定AB包的方法
    public void UnLoadAB(string name, UnityAction<bool> callBackResult)
    {
        if( abDic.ContainsKey(name) )
        {
            if (abDic[name] == null)
            {
                //代表正在异步加载 没有卸载成功
                callBackResult(false);
                return;
            }
            abDic[name].Unload(false);
            abDic.Remove(name);
            //卸载成功
            callBackResult(true);
        }
    }

    //清空AB包的方法
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        //卸载主包
        mainAB = null;
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        Resources.UnloadUnusedAssets();
    }
}
