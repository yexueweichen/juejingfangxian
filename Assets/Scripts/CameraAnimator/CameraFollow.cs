using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   //摄像机跟随目标
   public Transform target;

    [Header("摄像机偏移位置")]
    [SerializeField]
    private Vector3 offsetPos= new (0.56f, 2.43f, -2.65f);

    [Header("摄像机看的高度")]
    [SerializeField]
    private float bodyHeight=1.0f;
    //移动旋转速度
    [Header("移动旋转速度")]
    [SerializeField]
    private float moveSpeed=10f;
    [SerializeField]
    private float rotateSpeed=10f;
    
    [Header("鼠标Y轴灵敏度")]
    [SerializeField]
    private float mouseYSensitivity = 1f;
    
    [Header("俯仰角限制")]
    [SerializeField]
    private float minPitchAngle = -4f;
    [SerializeField]
    private float maxPitchAngle = 12f;
    
    private Quaternion cameraRotation;
    //当前俯仰角
    private float currentPitchAngle = 0f;

    void Update()
    {
        //摄像机跟随
        if (target == null)
            return;
        
        //处理鼠标Y轴输入
        HandleMouseYInput();
        
        //计算摄像机位置
        Vector3 cameraPos = target.position + target.forward * offsetPos.z;
        cameraPos += Vector3.up * offsetPos.y;
        cameraPos += target.right * offsetPos.x;
        transform.position = Vector3.Lerp(transform.position, cameraPos, moveSpeed * Time.deltaTime);

        //计算摄像机旋转
        CalculateCameraRotation();
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 处理鼠标Y轴输入
    /// </summary>
    private void HandleMouseYInput()
    {
        //获取鼠标Y轴输入
        float mouseY = Input.GetAxis("Mouse Y");

        float pitchChange = -mouseY * mouseYSensitivity * 100f * Time.deltaTime;

        currentPitchAngle += pitchChange;
        currentPitchAngle = Mathf.Clamp(currentPitchAngle, minPitchAngle, maxPitchAngle);
    }

    /// <summary>
    /// 计算摄像机旋转（包含俯仰角）
    /// </summary>
    private void CalculateCameraRotation()
    {
        // 看向的目标
        Vector3 lookTarget = target.position + Vector3.up * bodyHeight;
        // 从摄像机位置到目标的方向
        Vector3 dir = lookTarget - transform.position;

       
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
        float yaw = 0f;
        if (flatDir.sqrMagnitude > 0.0001f)
            yaw = Quaternion.LookRotation(flatDir).eulerAngles.y;

        float pitch = currentPitchAngle;

        // 构建最终朝向
        cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    /// <summary>
    /// 设置摄像机跟随目标
    /// </summary>
    /// <param name="targetTrans"></param>
    public void SetTarget(Transform targetTrans)
    {
        target = targetTrans;
        //重置俯仰角
        currentPitchAngle = 0f;
    }

}
