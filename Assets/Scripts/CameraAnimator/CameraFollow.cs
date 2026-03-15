using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   //摄像机跟随目标
   public Transform target;

    [Header("摄像机偏移位置")]
    [SerializeField]
    private Vector3 offsetPos;

    [Header("摄像机看的高度")]
    [SerializeField]
    private float bodyHeight;
    //移动旋转速度
    [Header("移动旋转速度")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    private Quaternion cameraRotation;

    void Update()
    {
      //摄像机跟随
        if (target == null)
            return;
      
        Vector3 cameraPos = target.position + target.forward * offsetPos.z;
        cameraPos+= Vector3.up*offsetPos.y;
        cameraPos+= target.right*offsetPos.x;
        transform.position = Vector3.Lerp(transform.position, cameraPos, moveSpeed * Time.deltaTime);

        cameraRotation= Quaternion.LookRotation(target.position + Vector3.up * bodyHeight - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, rotateSpeed * Time.deltaTime);
    }


    public void SetTarget(Transform targetTrans)
    {
        target = targetTrans;
    }






}
