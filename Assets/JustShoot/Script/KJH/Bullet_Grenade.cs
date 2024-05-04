using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Grenade : Bullet
{
    [SerializeField]GameObject fragPrf; //파편 프리팹
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;
        //if (collision.gameObject.CompareTag("Enemy"))//착탄한 대상의 태그가 Enemy일 경우, 이펙트를 피튀기는 이펙트로 변경
        //{
        //    type = 1;
        //}

        //여기에 적에 데미지를 입히는 기능을 추가할 것.
        ProjectileDestroy(collision.contacts[0].point, type);//충돌 위치를 넘겨서 각종 처리를 하는 역할.
    }
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.ExplosionEffectGenerate(hitPosition, 1);
        SplashDamage(hitPosition, 10);
        FragDemage(hitPosition, fragPrf, 100);

        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }

    void SplashDamage(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);//모든 콜라이더 검출

        foreach (Collider hit in colliders)//콜라이더 순회
        {
            if (hit.gameObject.CompareTag("Enemy"))
            {
                hit.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg * (radius - (center - hit.transform.position).magnitude)/radius);//피해 가함. 거리에 따라 데미지가 선형적으로 감소
            }
        }
    }
    void FragDemage(Vector3 center, GameObject fragPrf, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject frag = ObjectPoolManager.Instance.DequeueObject(fragPrf);
            frag.transform.position = center + new Vector3(0, 1, 0);
            frag.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            frag.GetComponent<Bullet>().Init(baseDmg * 0.01f, 100, 0.2f);
        }
    }
}
