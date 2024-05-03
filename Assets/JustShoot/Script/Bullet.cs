using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected float dmg;

    public void Init(float dmg, float velocity, float lifeTime)//�Ҹ��� ������, �ӵ�, �����ð� �ʱ�ȭ.
    {
        this.dmg = dmg;
        GetComponent<Rigidbody>().velocity = this.transform.forward * velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;
        if(collision.gameObject.CompareTag("Enemy"))//��ź�� ����� �±װ� Enemy�� ���, ����Ʈ�� ��Ƣ��� ����Ʈ�� ����
        {
            type = 1;
        }

        //���⿡ ���� �������� ������ ����� �߰��� ��.
        ProjectileDestroy(collision.contacts[0].point, type);//�浹 ��ġ�� �Ѱܼ� ���� ó���� �ϴ� ����.
    }    
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);        
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }
}
