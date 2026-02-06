using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


        //显示开始界面
        UIMgr.Instance.ShowPanel<BeginPanel>();
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
