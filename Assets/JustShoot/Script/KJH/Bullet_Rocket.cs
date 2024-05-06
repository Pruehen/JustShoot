using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Rocket : Bullet
{
    [SerializeField] ParticleSystem particleSystem;
    Rigidbody rigidbody;
    float velocity;
    public override void Init(float dmg, float velocity, float lifeTime)//불릿의 데미지, 속도, 유지시간 초기화.
    {
        this.baseDmg = dmg;
        this.lifeTime = lifeTime;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        this.velocity = velocity;
        particleSystem.transform.parent = this.transform;
        particleSystem.transform.position = this.transform.position;
        particleSystem.transform.rotation = this.transform.rotation;
        particleSystem.Play();
    }

    IEnumerator ParticleDestroy()
    {
        // 파티클이 재생되는 동안 오브젝트가 삭제되어도 파티클이 사라지지 않도록 설정합니다.
        particleSystem.transform.parent = null;
        particleSystem.Stop();

        // 파티클 재생이 끝날 때까지 기다립니다.
        yield return new WaitUntil(() => particleSystem.isStopped);

        particleSystem.transform.parent = this.transform;
        particleSystem.transform.position = this.transform.position;
        particleSystem.transform.rotation = this.transform.rotation;
    }


    private void FixedUpdate()
    {
        if(lifeTime > 4.5f)
        {
            rigidbody.AddForce(this.transform.forward * velocity * 2, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))//착탄한 대상의 태그가 Enemy일 경우 데미지를 가함
        {
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg);
        }
        ProjectileDestroy(collision.contacts[0].point);//충돌 위치를 넘겨서 각종 처리를 하는 역할.
    }
    void ProjectileDestroy(Vector3 hitPosition)
    {
        EffectManager.Instance.ExplosionEffectGenerate(hitPosition, 2);
        SplashDamage(hitPosition, 10);
        StartCoroutine(ParticleDestroy());

        SFXManager.Instance.ExplosionSoundOn(hitPosition);
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }

    void SplashDamage(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);//모든 콜라이더 검출

        foreach (Collider hit in colliders)//콜라이더 순회
        {
            Rigidbody targetRigidbody;
            if(hit.gameObject.TryGetComponent<Rigidbody>(out targetRigidbody))
            {
                targetRigidbody.AddForce((hit.transform.position - center).normalized * 3, ForceMode.VelocityChange);
            }

            if (hit.gameObject.CompareTag("Enemy"))
            {
                hit.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg * (radius - (center - hit.transform.position).magnitude) / radius);//피해 가함. 거리에 따라 데미지가 선형적으로 감소
            }
        }
    }
}
