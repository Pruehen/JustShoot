using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        //type 에따라 hit effect가 바뀜
        //effectManaget에서 설정하기
        int type = 0;
        if(collision.gameObject.CompareTag(tag: "Player"))
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
