using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : SceneSingleton<SFXManager>
{
    public GameObject explosion;
    public GameObject reload;

    public void SoundOn(Vector3 position, GameObject prf)//재생할 오디오소스가 담긴 프리팹을 매개변수로 받아서 position에서 재생시킴.
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(prf);

        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;

        AudioSource audioSource = item.GetComponent<AudioSource>();
        audioSource.Play();
        float onTime = audioSource.clip.length;

        StartCoroutine(EnqueueObject(item, onTime));
    }
    public void SoundOnAttach(Transform parent, GameObject prf)//재생할 오디오소스가 담긴 프리팹을 매개변수로 받아서 position에서 재생시킴.
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(prf);

        item.transform.SetParent(parent, false);

        AudioSource audioSource = item.GetComponent<AudioSource>();
        audioSource.Play();
        float onTime = audioSource.clip.length;

        StartCoroutine(EnqueueObject(item, onTime));
    }

    public void ExplosionSoundOn(Vector3 position)//매니저에 미리 캐싱해놓은 프리팹을 매개변수로 넘겨서 position에서 재생시킴.
    {
        SoundOn(position, explosion);
    }
    public void ReloadSoundOn(Vector3 position)
    {
        SoundOn(position, reload);
    }

    IEnumerator EnqueueObject(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
