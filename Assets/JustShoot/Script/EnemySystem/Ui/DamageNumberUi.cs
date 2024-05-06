using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberUi : MonoBehaviour
{
    Vector3 velocity = Vector3.up * 300f;
    float age = 0f;
    public float lifeTime = 1f;
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
        age = 0f;
        damageui.fontSize = fontSize;
    }

    private void Update()
    {
        age += Time.deltaTime;
        Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
        Vector3 offset = new Vector3(100f, 100f);
        pos += offset;
        pos += velocity * age;
        damageui.fontSize = fontSize * (lifeTime - age);
        damageui.rectTransform.position = pos;
        StartCoroutine(EffectManager.Instance.EnqueueObject(gameObject, lifeTime));
    }
}
