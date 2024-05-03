using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform firePoint;//투사체 생성 위치
    [SerializeField] Transform fpsViewPoint;//1인칭으로 볼 가상카메라 위치    

    [Header("Spec")]
    [SerializeField] GameObject projectilePrf;//발사할 투사체 프리팹
    [SerializeField] float rpm;//분당 발사 속도
    [SerializeField] int fireCount;//1회당 발사 속도. 
    [SerializeField] float burstDelay;//1회당 여러발을 발사하게 될 경우, 그 발간의 시간 간격
    [SerializeField] int magazineBulletCount;//1탄창당 탄환 수
    [SerializeField] float operability;//무기 조작성. 수치가 클수록 캐릭터 회전이 빠름
    [SerializeField] float projectileVelocity;//포구초속. m/s단위
    [SerializeField] float dmg;//발당 데미지
    [SerializeField] float recoil;//반동. 클수록 화면이 크게 튐
    [SerializeField] float reloadTime;//재장전 시간. (초)

    float delay;//무기 연사 속도 조절을 위한 변수
    int bullet;//현재 무기의 남은 탄환 수
    bool trigger;//무기의 트리거 상태 (true일 경우 무기 발사)

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

        if(trigger && delay >= 60/rpm && bullet > 0)//트리거 on && 발사 가능 시간 && 잔탄 남았을 경우
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
            bulletIst = ObjectPoolManager.Instance.DequeueObject(projectilePrf);//프리팹 생성
            bulletIst.transform.position = firePoint.position;//좌표 지정
            bulletIst.transform.rotation = firePoint.rotation;//회전 지정

            bulletIst.GetComponent<Bullet>().Init(dmg, projectileVelocity);//데미지, 탄속 지정

            EffectManager.Instance.FireEffectGenenate(firePoint.position, firePoint.rotation);//발사 이펙트 생성
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
