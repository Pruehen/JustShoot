using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCount : MonoBehaviour
{
    TMPro.TMP_Text text;
    private void Awake()
    {
        Player.Instance.combat.OnKill += SetText;
        text = GetComponent<TMPro.TMP_Text>();
    }
    private void SetText()
    {
        text.text = $"Kill : {Player.Instance.combat.GetKillCount()}";
    }
}
