using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Combat
{
    private int killCount = 0;
    private float dealCount = 0f;

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
}
