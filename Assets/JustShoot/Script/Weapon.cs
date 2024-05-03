using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform firePoint;//����ü ���� ��ġ
    [SerializeField] Transform fpsViewPoint;//1��Ī���� �� ����ī�޶� ��ġ    

    [Header("Spec")]
    [SerializeField] GameObject projectilePrf;//�߻��� ����ü ������
    [SerializeField] float rpm;//�д� �߻� �ӵ�
    [SerializeField] int fireCount;//1ȸ�� �߻� �ӵ�. 
    [SerializeField] float burstDelay;//1ȸ�� �������� �߻��ϰ� �� ���, �� �߰��� �ð� ����
    [SerializeField] int magazineBulletCount;//1źâ�� źȯ ��
    [SerializeField] float operability;//���� ���ۼ�. ��ġ�� Ŭ���� ĳ���� ȸ���� ����
    [SerializeField] float projectileVelocity;//�����ʼ�. m/s����
    [SerializeField] float dmg;//�ߴ� ������
    [SerializeField] float recoil;//�ݵ�. Ŭ���� ȭ���� ũ�� Ʀ
    [SerializeField] float reloadTime;//������ �ð�. (��)

    float delay;//���� ���� �ӵ� ������ ���� ����
    int bullet;//���� ������ ���� źȯ ��
    bool trigger;//������ Ʈ���� ���� (true�� ��� ���� �߻�)

    public void SetTrigger(bool value)
    {
        trigger = value;
    }
    public float GetReloadTime()
    {
        return reloadTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        delay = 0;
        bullet = magazineBulletCount;
        trigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        delay += Time.deltaTime;

        if(trigger && delay >= 60/rpm && bullet > 0)//Ʈ���� on && �߻� ���� �ð� && ��ź ������ ���
        {
            delay = 0;
            StartCoroutine(Fire(fireCount, burstDelay));
        }
    }

    IEnumerator Fire(int count, float delay)
    {
        GameObject bulletIst;
        for (int i = 0; i < count; i++)
        {            
            bulletIst = ObjectPoolManager.Instance.DequeueObject(projectilePrf);//������ ����
            bulletIst.transform.position = firePoint.position;//��ǥ ����
            bulletIst.transform.rotation = firePoint.rotation;//ȸ�� ����

            bulletIst.GetComponent<Bullet>().Init(dmg, projectileVelocity, 2);//������, ź�� ����

            EffectManager.Instance.FireEffectGenenate(firePoint.position, firePoint.rotation);//�߻� ����Ʈ ����
            //impulseSource.GenerateImpulse(this.transform.position);
            //mouseDeltaPos = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 3f));

            //animator.SetTrigger("Fire");
            bullet--;

            yield return new WaitForSeconds(delay);
        }
    }

    public void ReloedStart()
    {
        bullet = 0;
    }
    public void ReloedEnd()
    {
        bullet = magazineBulletCount;
    }
}
