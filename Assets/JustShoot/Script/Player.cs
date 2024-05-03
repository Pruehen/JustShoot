using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Player : SceneSingleton<Player>
{
    [SerializeField] Animator animator;

    Vector2 moveVector = Vector2.zero;
    Vector2 moveVectorTarget;
    float moveSpeed = 1;
    bool isFire = false;
    bool isZoom = false;
    Vector2 mouseDeltaPos = Vector2.zero;

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

    float hp;
    float maxHp = 100;

   // CinemachineImpulseSource impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        //tpsCmc = tpsVCam.GetComponent<CinemachineVirtualCamera>();        
        cc = GetComponent<CharacterController>();
        //impulseSource = GetComponent<CinemachineImpulseSource>();

        SetCamType(false);

        WeaponChange(0);

        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        MoveOrder();//�̵�  
        RotateOrder();//ĳ���� �� �ѱ� ȸ��

        WeaponSelect();
        //GunFire();//���� ���
        //Debug.Log(tpsVCam.transform.position);
    }
    private void LateUpdate()
    {
        CamRotate();//ī�޶� ȸ��
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
            mouseDeltaPos *= 0.2f;
        }
        else
        {
            mouseDeltaPos *= 0.4f;
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
        if(isFire)//��ư ��Ŭ�� �Է��� �����Ǿ��� ��� (��Ŭ�� �ϴ� ���� ���)
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

            //WeaponŬ�������� �߻� ó���ϴ� ������ ����
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
        hp -= dmg;
        if(hp <= 0)
        {
            Debug.Log("�÷��̾� ü�� 0");
            //��� ��� �߰�
        }
    }

    void OnMove(InputValue inputValue)//WASD ����
    {
        moveVectorTarget = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�
        //moveVectorTarget = inputMovement;
        //Debug.Log(inputMovement);
    }
    void OnSprint(InputValue inputValue)
    {        
        float value = inputValue.Get<float>();
        moveSpeed = (value * 4) + 1;
        //Debug.Log(isClick);
    }
    void OnLeftClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//������ ��
        {
            isFire = true;
        }
        else//�� ��
        {
            isFire = false;
        }
        controlweapon.SetTrigger(isFire);
    }
    void OnRightClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//������ ��
        {
            isZoom = true;            
        }
        else//�� ��
        {
            isZoom = false;            
        }
        SetCamType(isZoom);
    }
    void OnAim(InputValue inputValue)
    {
        mouseDeltaPos = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�        
    }
    void OnReload(InputValue inputValue)
    {
        float isClick = inputValue.Get<float>();

        if(!isReload)
        {
            //Debug.Log(isClick);
            isReload = true;
            Reload();
        }
    }
}
