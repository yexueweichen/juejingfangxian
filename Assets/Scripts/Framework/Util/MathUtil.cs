using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 数学工具类
/// </summary>
public class MathUtil
{
    #region 距离计算
    /// <summary>
    /// 获取XZ平面上 两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXZ(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(srcPos, targetPos);
    }

    /// <summary>
    /// 判断两点之间距离 是否小于等于目标距离XZ平面
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXZ(Vector3 srcPos, Vector3 targetPos, float dis)
    {
        return GetObjDistanceXZ(srcPos, targetPos) <= dis;
    }
    #endregion

    #region 射线检测相关

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的GameObject信息传递出去）</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo.collider.gameObject);
    }

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的对象信息上挂在的指定脚本传递出去）</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
        {
            T component = hitInfo.collider.gameObject.GetComponent<T>();
            if (component == null)
                component = hitInfo.collider.GetComponentInParent<T>();
            if (component != null)
                callBack.Invoke(component);
        }
    }

    /// <summary>
    /// 射线检测 获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            T component = hitInfos[i].collider.gameObject.GetComponent<T>();
            if (component == null)
                component = hitInfos[i].collider.GetComponentInParent<T>();
            if (component != null)
                callBack.Invoke(component);
        }
    }
    #endregion

    #region 范围检测
    /// <summary>
    /// 进行球体范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
    /// <param name="center">球体的中心点</param>
    /// <param name="radius">球体的半径</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    public static void OverlapSphere<T>(Vector3 center, float radius, int layerMask, UnityAction<T> callBack) where T:class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
                callBack.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
    #endregion
}
