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

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
