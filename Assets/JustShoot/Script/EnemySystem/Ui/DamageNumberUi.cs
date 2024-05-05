using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberUi : MonoBehaviour
{
    TMPro.TMP_Text damageui;
    private void Awake()
    {
        damageui = GetComponent<TMPro.TMP_Text>();
    }

    public void Init(string text, Vector3 position)
    {
        damageui.rectTransform.position = Camera.main.WorldToScreenPoint(position);
        damageui.text = text;
    }
}
