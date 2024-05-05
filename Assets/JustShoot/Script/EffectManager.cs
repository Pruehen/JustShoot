using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SceneSingleton<EffectManager>
{
    public GameObject bulletHitEffect;
    public GameObject bulletFireEffect;
    public GameObject bloodEffect;
    public GameObject explosionEffect_s;
    public GameObject explosionEffect_l;
    public GameObject damageNumberUi;
    public Transform damageUiParent;

    public void HitEffectGenenate(Vector3 position, int type)
    {
        if (type == 0)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(bulletHitEffect);
            item.transform.position = position;
            item.transform.rotation = Quaternion.identity;

            StartCoroutine(EnqueueObject(item, 0.5f));
        }
        else
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(bloodEffect);
            item.transform.position = position;
            item.transform.rotation = Quaternion.identity;

            StartCoroutine(EnqueueObject(item, 0.5f));
        }
    }
    public void FireEffectGenenate(Vector3 position, Quaternion rotation)
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(bulletFireEffect);
        item.transform.position = position;
        item.transform.rotation = rotation;

        StartCoroutine(EnqueueObject(item, 0.5f));
    }
    public void ExplosionEffectGenerate(Vector3 position, float size)
    {
        if (size == 1)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(explosionEffect_s);
            item.transform.position = position;
            item.transform.rotation = Quaternion.identity;
            StartCoroutine(EnqueueObject(item, 3));
        }
        else
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(explosionEffect_l);
            item.transform.position = position;
            item.transform.rotation = Quaternion.identity;
            StartCoroutine(EnqueueObject(item, 5));
        }
    }

    IEnumerator EnqueueObject(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }

    public void DamageNumberUiGenerate(Vector3 position, float damage)
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(damageNumberUi);
        item.transform.SetParent(damageUiParent);
        item.transform.GetComponent<DamageNumberUi>().Init(damage.ToString(), position);
        StartCoroutine(EnqueueObject(item, 3));
    }
}
