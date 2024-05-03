using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;
        if(collision.gameObject.CompareTag("Enemy"))
        {
            type = 1;
        }

        ProjectileDestroy(collision.contacts[0].point, type);
    }    
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);        
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }
}
