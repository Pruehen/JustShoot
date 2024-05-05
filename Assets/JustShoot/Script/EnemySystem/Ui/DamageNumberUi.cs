using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberUi : MonoBehaviour
{
    Vector3 velocity = Vector3.up * 300f;
    float lifeTime = 0f;
    TMPro.TMP_Text damageui;
    Transform target;
    float fontSize = 50f;
    private void Awake()
    {
        damageui = GetComponent<TMPro.TMP_Text>();
    }

    public void Init(string text, Transform target)
    {
        damageui.text = text;
        this.target = target;
        lifeTime = 0f;
        damageui.fontSize = fontSize;
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
        Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
        Vector3 offset = new Vector3(100f, 100f);
        pos += offset;
        pos += velocity * lifeTime;
        damageui.fontSize = fontSize * (1f - lifeTime);
        damageui.rectTransform.position = pos;
    }
}
