using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[System.Serializable]
public class PlayerCombat : Combat
{
    private int killCount = 0;
    private float dealCount = 0f;

    public PlayerCombat(Transform owner, float maxHp, bool defaultEffectOnDamaged = true) : base(owner, maxHp, defaultEffectOnDamaged)
    {
    }
    public void AddDealCount(float damage)
    {
        dealCount += damage;
        Debug.Log("addDealCount");
    }
    public void AddKillCount()
    {
        killCount++;
        Debug.Log("addKillCount");
    }
    public int GetKillCount()
    {
        return killCount;
    }
    public float GetDealCount()
    {
        return dealCount;
    }
}
