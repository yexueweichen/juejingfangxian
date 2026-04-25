using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backsetting : MonoBehaviour
{
   
void Update()
{
 if (Input.GetKeyDown(KeyCode.Escape))
 {
    UIMgr.Instance.ShowPanel<BackPanel>();
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    Time.timeScale = 0;
 }

}
}
