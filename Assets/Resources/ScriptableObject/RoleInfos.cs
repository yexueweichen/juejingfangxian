using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoleInfos", menuName = "ScriptableObjects/RoleInfos", order = 0)]
public class RoleInfos:ScriptableObject

{
    [System.Serializable]
    public class RoleData
    {
        public int id;
        public string res;
        public int atk;
        public int lockMoney;
        public int hp;
        public string tips;
        public int type;
        public string hitEff;


    }

    public List<RoleData> roleDatas;












}
