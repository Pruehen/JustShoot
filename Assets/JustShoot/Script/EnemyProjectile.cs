using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Bullet
{
    ParticleSystem ps;
    Rigidbody erb;
    private void Awake()
    {
        ps = transform.GetComponentInChildren<ParticleSystem>();
        erb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;

        if (collision.gameObject.CompareTag("Enemy"))//착탄한 대상의 태그가 Enemy일 경우, 이펙트를 피튀기는 이펙트로 변경
        {
            type = 1;
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg);
        }
        else if (collision.gameObject.CompareTag("Player"))//착탄한 대상의 태그가 Player일 경우, 이펙트를 피튀기는 이펙트로 변경
        {
            type = 1;
            Player.Instance.TakeDamage(baseDmg);
        }
        StartCoroutine(ProjectileDestroy(collision.contacts[0].point, type));//충돌 위치를 넘겨서 각종 처리를 하는 역할.
    }
    IEnumerator ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);//착탄 이펙트 발생
        erb.velocity = Vector3.zero;
        ps.Stop();

        yield return new WaitForSeconds(3f);
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);//이 오브젝트 파괴
    }
}
