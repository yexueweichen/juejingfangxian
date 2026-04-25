using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : MonoBehaviour
{
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           playerInside = true;
           UIMgr.Instance.ShowPanel<TipUI>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            UIMgr.Instance.HidePanel<TipUI>();
        }
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIMgr.Instance.ShowPanel<ProgressPanel>();
               UIMgr.Instance.HidePanel<TipUI>();
            SceneMgr.Instance.LoadSceneAsync("BeginScene", () =>
            {
                UIMgr.Instance.HidePanel<ProgressPanel>();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }).Forget();
        }
    }
}
