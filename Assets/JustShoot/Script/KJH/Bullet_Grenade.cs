using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Grenade : Bullet
{
    [SerializeField]GameObject fragPrf; //파편 프리팹
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;
        if (collision.gameObject.CompareTag("Enemy"))//착탄한 대상의 태그가 Enemy일 경우, 이펙트를 피튀기는 이펙트로 변경
        {
            type = 1;
        }

        //여기에 적에 데미지를 입히는 기능을 추가할 것.
        ProjectileDestroy(collision.contacts[0].point, type);//충돌 위치를 넘겨서 각종 처리를 하는 역할.
    }
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }
}
