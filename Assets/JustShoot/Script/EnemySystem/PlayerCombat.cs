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
    public Action OnKill;
    public GameObject killSfx;

    public override void Init(Transform owner, float maxHp)
    {
        base.Init(owner, maxHp);
    }

    public void AddDealCount(float damage)
    {
        dealCount += damage;
    }
    public void AddKillCount()
    {
        killCount++;
        SFXManager.Instance.SoundOnAttach(_owner.transform, killSfx);
        OnKill?.Invoke();
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
