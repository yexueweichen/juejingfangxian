using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            // 如果正在退出或场景正在卸载，避免动态创建新对象
            if (applicationIsQuitting)
                return null;

            if (instance == null)
            {
                // 动态创建 动态挂载
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        // 当单例本身销毁时也标记
        applicationIsQuitting = true;
        instance = null;
    }
}
