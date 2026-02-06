using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   public Transform target;
    //摄像机偏移位置
    public Vector3 offsetPos;
    //摄像机看的高度
    public float bodyHeight;
    //移动旋转速度
    public float moveSpeed;
    public float rotateSpeed;
    private Vector3 cameraPos;
    private Quaternion cameraRotation;
    void Start()
    {
        
    }

    
    void Update()
    {
      //摄像机跟随
        if (target == null)
            return;
        cameraPos = target.position + target.forward * offsetPos.z;
        cameraPos+= Vector3.up*offsetPos.y;
        cameraPos+= target.right*offsetPos.x;
        transform.position = Vector3.Lerp(transform.position, cameraPos, moveSpeed * Time.deltaTime);

        cameraRotation= Quaternion.LookRotation((target.position + Vector3.up * bodyHeight) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, rotateSpeed * Time.deltaTime);
    }


    public void SetTarget(Transform targetTrans)
    {
        target = targetTrans;
    }






}
