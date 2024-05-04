using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    PlayerCombatData data;


    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        data = Player.Instance.GetCombatData();
    }
    
    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.text =  $"{data.cwCurMag} / {data.cwMaxMag}";
    }
}
