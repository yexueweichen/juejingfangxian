using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : MonoBehaviour
{
   public int hp;
    public int maxHP;
    public bool isDead;

    private static MainTower instance;

    public static MainTower Instance
    {
        get
        {
         
            return instance;
        }
    }

    private MainTower() { }


    private void Awake()
    {
        instance = this;
    }

    //更新血量
    public void UpdateHp(int hp,int maxHP)
    {
        this.hp = hp;
        this.maxHP = maxHP;
        UIMgr.Instance.GetPanel<GamePanel>().UpdateTowerHp(hp, maxHP);

    }

    //受到伤害
    public void Wound(int dmg)
    {
        if (isDead)
            return;
         hp-=dmg;
        if(hp < 0)
        {
            hp= 0;
            isDead = true;
            //失败
           UIMgr.Instance.ShowPanel<GameOverPanel>().
                InitInfo((int)(GameLevelMgr.Instance.player.money*0.5f), false);
        }
       UpdateHp(hp, maxHP);
    }


    private void OnDestroy()
    {
        instance = null;
    }


}
