using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型 枚举
/// </summary>
public enum E_EventType 
{
    /// <summary>
    /// 怪物死亡事件 
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家获取奖励 
    /// </summary>
    E_Player_GetReward,
    /// <summary>
    /// 场景切换时进度变化获取
    /// </summary>
    E_SceneLoadChange,
    /// <summary>
    /// 更新能量值
    /// </summary>
    E_AddEnergy,
    E_ReduceEnergy,

    E_UpdateTowerHp,
    E_UpdateTEnergy,

    // CharacterChoose UI 事件 
    E_UI_Char_RequestInit,   // 请求初始化（视图 -> 控制器）
    E_UI_Char_Next,          // 右/下一个（视图 -> 控制器）
    E_UI_Char_Prev,          // 左/上一个（视图 -> 控制器）
    E_UI_Char_Buy,           // 购买（视图 -> 控制器）
    E_UI_Char_Start,         // 开始（视图 -> 控制器）
    E_UI_Char_Back,          // 返回（视图 -> 控制器）

    // 视图更新事件
    E_UI_UpdateMoney,        // int
    E_UI_UpdateRoleInfo,     // RoleInfos.RoleData
    E_UI_ShowTip,          // string

// 输入系统 事件 
      E_Input_Space,       
    /// <summary>
    /// 水平热键 
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// 竖直热键
    /// </summary>
    E_Input_Vertical,

    E_Mouse_0, //射击
    E_Input_Shift, // 下蹲
    E_Input_ShiftUp, // 松开下蹲
    E_Mouse_X, //转头
    E_MonsterKilled, // 怪物击杀事件

}
