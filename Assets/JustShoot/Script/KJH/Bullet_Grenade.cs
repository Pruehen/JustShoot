using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Grenade : Bullet
{
    [SerializeField]GameObject fragPrf; //���� ������
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;
        if (collision.gameObject.CompareTag("Enemy"))//��ź�� ����� �±װ� Enemy�� ���, ����Ʈ�� ��Ƣ��� ����Ʈ�� ����
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
