using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : SceneSingleton<Player>
{
    [SerializeField] Animator animator;

    Vector2 moveVector = Vector2.zero;
    Vector2 moveVectorTarget;
    float moveSpeed = 1;
    bool isFire = false;
    bool isZoom = false;
    Vector2 mouseDeltaPos = Vector2.zero;
    float senst;//카메라 감도
    int controlWeaponIndex;

    CharacterController cc;

    [SerializeField] GameObject tpsVCamRoot;
    [SerializeField] CinemachineVirtualCamera tpsVCam;   
    [SerializeField] Transform weaponPoint;

    [Header("UsingWeapons")]
    [SerializeField] Weapon[] weapons;
    
    Weapon controlweapon;

    //float fireDelay = 0;
    //float delayCount = 0.1f;
    //int shell = 100;

    bool isReload = false;
    bool isDead = false;

    public PlayerCombat combat = new PlayerCombat();
    PlayerCombatData data = new PlayerCombatData();

    // CinemachineImpulseSource impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        combat.Init(transform, 100f);
        //tpsCmc = tpsVCam.GetComponent<CinemachineVirtualCamera>();        
        cc = GetComponent<CharacterController>();
        //impulseSource = GetComponent<CinemachineImpulseSource>();

        SetCamType(false);

        controlWeaponIndex = 0;
        WeaponChange(controlWeaponIndex);

        combat.OnDamaged += HitAnimPlay;
        combat.OnDead += DieAnimPlay;

        senst = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            MoveOrder();//이동  
            RotateOrder();//캐릭터 및 총기 회전

            WeaponSelect();
            //GunFire();//무기 사용
            //Debug.Log(tpsVCam.transform.position);

            SetCombatData();
        }
        else
        {
            DeadCamMove();
        }
    }
    private void LateUpdate()
    {
        if (!isDead)
        {
            CamRotate();//카메라 회전
        }
    }
    private void MoveOrder()
    {        
        moveVector = Vector2.Lerp(moveVector, moveVectorTarget * moveSpeed, Time.deltaTime * 5);

        Vector3 moveVector3 = new Vector3(moveVector.x * 0.5f, Physics.gravity.y, moveVector.y);
        //moveVector3 = this.transform.rotation * moveVector3;
        
        cc.Move(this.transform.rotation * moveVector3 * Time.deltaTime);

        animator.SetFloat("X_Speed", moveVector.x);
        animator.SetFloat("Y_Speed", moveVector.y);
    }
    void CamRotate()
    {
        tpsVCamRoot.transform.position = this.transform.position + new Vector3(0, 1.5f, 0);

        Vector3 camAngle = tpsVCamRoot.transform.rotation.eulerAngles;

        
        if(isZoom)
        {
            mouseDeltaPos *= 0.2f * senst;
        }
        else
        {
            mouseDeltaPos *= 0.4f * senst;
        }

        float x = camAngle.x - mouseDeltaPos.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 25f);
        }
        else
        {
            x = Mathf.Clamp(x, 345f, 361f);
        }

        tpsVCamRoot.transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDeltaPos.x, camAngle.z);
        mouseDeltaPos *= 0.9f;
    }
    void DeadCamMove()
    {
        tpsVCam.transform.localPosition += new Vector3(1, 1, -1) * Time.deltaTime;
        tpsVCam.transform.LookAt(this.transform.position);
    }
    void RotateOrder()
    {
        Vector3 direction = (tpsVCam.transform.forward).normalized;
        
        Quaternion rotationWeapon = Quaternion.LookRotation(direction);
        rotationWeapon = Quaternion.Euler(rotationWeapon.eulerAngles.x, this.transform.rotation.eulerAngles.y, rotationWeapon.eulerAngles.z);
        weaponPoint.rotation = Quaternion.Slerp(weaponPoint.rotation, rotationWeapon, Time.deltaTime * 6);

        direction = new Vector3(direction.x, 0, direction.z);

        Quaternion rotationBody = Quaternion.LookRotation(direction);
        //rotationBody = Quaternion.Euler(0, rotationBody.eulerAngles.y, 0);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationBody, Time.deltaTime * 6);
    }
    void SetCamType(bool isFps)
    {
        if (isFps)
        {
            tpsVCam.Priority = 9;
        }
        else
        {
            tpsVCam.Priority = 11;
        }
    }
    void WeaponSelect()
    {
        if (isReload == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                WeaponChange(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                WeaponChange(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                WeaponChange(2);
            }
        }
    }
    void WeaponChange(int index)
    {
        if(weapons.Length > index)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if(index == i)
                {
                    weapons[i].gameObject.SetActive(true);
                    controlweapon = weapons[i];
                    controlWeaponIndex = index;
                }
                else
                {
                    weapons[i].gameObject.SetActive(false);
                }
            }
        }
    }
    void GunFire()
    {
        if(isFire)//버튼 좌클릭 입력이 감지되었을 경우 (좌클릭 하는 중인 경우)
        {
            //fireDelay = 0;

            //GameObject bulletIst = ObjectPoolManager.Instance.DequeueObject(bullet);
            //bulletIst.transform.position = firePoint.position;
            //bulletIst.transform.rotation = firePoint.rotation;

            //bulletIst.GetComponent<Rigidbody>().velocity = bulletIst.transform.forward * 500;

            //EffectManager.Instance.FireEffectGenenate(firePoint.position, firePoint.rotation);
            ////impulseSource.GenerateImpulse(this.transform.position);
            //mouseDeltaPos = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 3f));

            //animator.SetTrigger("Fire");
            //shell--;

            //Weapon클래스에서 발사 처리하는 것으로 변경
        }
    }
    void Reload()
    {
        animator.SetTrigger("Reload");
        animator.SetFloat("ReloadSpeed", 4/controlweapon.GetReloadTime());
        controlweapon.ReloedStart();
        StartCoroutine(ReloadEnd());
    }
    IEnumerator ReloadEnd()
    {
        yield return new WaitForSeconds(controlweapon.GetReloadTime());
        isReload = false;
        controlweapon.ReloedEnd();
        //shell += 100;
        //shell = Mathf.Clamp(shell, 0, 101);
    }

    public void TakeDamage(float dmg)
    {
        combat.TakeDamage(dmg);
        if(!isDead && combat.IsDead())
        {
            isDead = true;
            SetCamType(false);
        }
    }


    void OnMove(InputValue inputValue)//WASD 조작
    {
        moveVectorTarget = inputValue.Get<Vector2>();//인풋 벡터 받아옴
        //moveVectorTarget = inputMovement;
        //Debug.Log(inputMovement);
    }
    void OnSprint(InputValue inputValue)
    {        
        float value = inputValue.Get<float>();
        moveSpeed = (value * 4) + 1;
        //Debug.Log(isClick);
    }
    void OnLeftClick(InputValue inputValue)//마우스 좌클릭
    {
        if (!isDead)
        {
            float isClick = inputValue.Get<float>();

            if (isClick == 1)//눌렀을 때
            {
                isFire = true;
            }
            else//뗄 때
            {
                isFire = false;
            }
            controlweapon.SetTrigger(isFire);
        }
    }
    void OnRightClick(InputValue inputValue)//마우스 우클릭
    {
        if (!isDead)
        {
            float isClick = inputValue.Get<float>();

            if (isClick == 1)//눌렀을 때
            {
                isZoom = true;
            }
            else//뗄 때
            {
                isZoom = false;
            }
            SetCamType(isZoom);
        }
    }
    void OnAim(InputValue inputValue)
    {
        mouseDeltaPos = inputValue.Get<Vector2>();//인풋 벡터 받아옴        
    }
    void OnReload(InputValue inputValue)
    {
        if (!isDead)
        {
            float isClick = inputValue.Get<float>();

            if (!isReload)
            {
                //Debug.Log(isClick);
                isReload = true;
                Reload();
            }
        }
    }

    void SetCombatData()
    {
        data.playerMaxHp = combat.GetMaxHp();
        data.playerCurHp = combat.GetHp();
        data.controlWeaponName = controlweapon.gameObject.name;
        data.controlWeaponIndex = controlWeaponIndex;
        data.cwMaxMag = controlweapon.magazinBulletcount();
        data.cwCurMag = controlweapon.bullet;
        data.killCount = combat.GetKillCount();
    }

    public PlayerCombatData GetCombatData()
    {
        return data;
    }


    //combat에 이벤트 등록
    private void HitAnimPlay()
    {
        animator.SetTrigger("Hit");
    }
    private void DieAnimPlay()
    {
        animator.SetTrigger("Die");
    }
}

public class PlayerCombatData//참조용 데이터 클래스
{
    public float playerMaxHp;//최대 체력
    public float playerCurHp;//현재 체력
    public string controlWeaponName;//사용중인 무기 이름
    public int controlWeaponIndex;//사용중인 무기 인덱스
    public int cwMaxMag;//사용중인 무기 최대 장탄량
    public int cwCurMag;//사용중인 무기 현재 장탄량
    public int killCount;//킬수
}