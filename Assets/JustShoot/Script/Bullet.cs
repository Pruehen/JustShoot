using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected float dmg;

    public void Init(float dmg, float velocity, float lifeTime)//불릿의 데미지, 속도, 유지시간 초기화.
    {
        this.dmg = dmg;
        GetComponent<Rigidbody>().velocity = this.transform.forward * velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;

        if (collision.gameObject.CompareTag("Enemy"))//착탄한 대상의 태그가 Enemy일 경우, 이펙트를 피튀기는 이펙트로 변경
        {
            type = 1;
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(dmg);
        }
        else if (collision.gameObject.CompareTag("Player"))//착탄한 대상의 태그가 Player일 경우, 이펙트를 피튀기는 이펙트로 변경
        {
            type = 1;
            Player.Instance.TakeDamage(dmg);
        }
        ProjectileDestroy(collision.contacts[0].point, type);//충돌 위치를 넘겨서 각종 처리를 하는 역할.
    }    
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);        
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }
}
