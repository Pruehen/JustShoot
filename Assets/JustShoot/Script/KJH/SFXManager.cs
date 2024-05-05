using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : SceneSingleton<SFXManager>
{
    public GameObject explosion;
    public GameObject reload;

    public void SoundOn(Vector3 position, GameObject prf)
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(prf);

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
