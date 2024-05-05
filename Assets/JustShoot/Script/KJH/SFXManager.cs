using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public GameObject gunFire_s;
    public GameObject gunFire_m;
    public GameObject gunFire_l;
    public GameObject rocketLaunch;
    public GameObject explosion;
    public GameObject reload;

    public void GunFireSoundOn(Vector3 position, int type)
    {
        GameObject item;
        if (type == 0)
        {
            item = ObjectPoolManager.Instance.DequeueObject(gunFire_s);
        }
        else if(type == 1)
        {
            item = ObjectPoolManager.Instance.DequeueObject(gunFire_m);
        }
        else
        {
            item = ObjectPoolManager.Instance.DequeueObject(gunFire_l);
        }

        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;

        AudioSource audioSource = item.GetComponent<AudioSource>();
        audioSource.Play();
        float onTime = audioSource.clip.length;

        StartCoroutine(EnqueueObject(item, onTime));
    }

    IEnumerator EnqueueObject(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
