using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beginmonster : MonoBehaviour
{
    Animator anim;
    public float walkDuration = 7f;  
    public float waitDuration = 5f;   

    
    private bool isDestroyed;

    async void Start()
    {
        anim = GetComponent<Animator>();
        await UniTask.Delay((int)(waitDuration * 1000));
        LoopWalk().Forget();
    }

    private void OnDestroy()
    {
        // 当对象销毁时设置标记
        isDestroyed = true;
    }

    async UniTaskVoid LoopWalk()
    {
        while (!isDestroyed)
        {
            // 如果 Animator 丢失，尽量重新获取，获取不到则退出循环
            if (anim == null)
            {
                if (isDestroyed) break;
                anim = GetComponent<Animator>();
                if (anim == null) break;
            }

            // 1. 朝左走
            anim.SetBool("walk", true);
            await UniTask.Delay((int)(walkDuration * 1000));
            if (isDestroyed) break;

            // 2. 停下等待
            anim.SetBool("walk", false);
            await UniTask.Delay((int)(waitDuration * 1000));
            if (isDestroyed) break;

            // 3. 转身朝右
            transform.rotation = Quaternion.Euler(0, 180, 0);

            // 4. 朝右走
            anim.SetBool("walk", true);
            await UniTask.Delay((int)(walkDuration * 1000));
            if (isDestroyed) break;

            // 5. 停下等待
            anim.SetBool("walk", false);
            await UniTask.Delay((int)(waitDuration * 1000));
            if (isDestroyed) break;

            // 6. 转身朝左，回到初始方向
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}


