using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class UIMgr : BaseManager<UIMgr>
{
    private Transform canvasTrans;
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    
    private UIMgr()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            canvas = GameObject.Instantiate(Resources.Load<GameObject>("Canvas"));
        }
        canvasTrans = canvas.transform;
        GameObject.DontDestroyOnLoad(canvas);
    }

  
    public void ShowPanel<T>(UnityAction<T> PanelLoad = null,bool isSync = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;

        // 如果面板已存在，直接通过回调返回
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            PanelLoad?.Invoke(panelDic[panelName] as T);
            return;
        }

        // 异步加载 UI 预设体
        ABMgr.Instance.LoadResAsync<GameObject>("ui", panelName, (obj) =>
        {
            if (obj == null)
            {
                Debug.LogError($"加载失败: {panelName}");
                PanelLoad?.Invoke(null);
                return;
            }
            
            GameObject panelObj = GameObject.Instantiate(obj);
            panelObj.transform.SetParent(canvasTrans, false);
            
            T panel = panelObj.GetComponent<T>();
            if (panel == null)
            {
                Debug.LogError($"面板{panelName} 没有脚本{typeof(T).Name}");
                GameObject.Destroy(panelObj);
                PanelLoad?.Invoke(null);
                return;
            }
            
            if (!panelDic.ContainsKey(panelName))
            {
                panelDic.Add(panelName, panel);
            }
            
            // 确保面板是激活状态
            panelObj.SetActive(true);
            
            // 等待一帧确保被调用
            panel.ShowMe();

            // 加载完成后，通过回调返回面板
            PanelLoad?.Invoke(panel);
        },isSync).Forget();
    }

    
    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            if (isFade)
            {
                panelDic[panelName].HideMe(() =>
                {
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }
    }

    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        return panelDic.ContainsKey(panelName) ? panelDic[panelName] as T : null;
    }

    public Transform GetCanvasTransform()
    {
        return canvasTrans;
    }


}




