using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    CharacterController cc;

    [SerializeField] GameObject tpsVCam;
    //CinemachineVirtualCamera tpsCmc;
    [SerializeField] CinemachineVirtualCamera fpsVCam;
    [SerializeField] Transform weaponPoint;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform shellPoint;

    [Header("Prefabs")]
    public GameObject bullet;
    public GameObject bullet_Shell;

    float fireDelay = 0;
    float delayCount = 0.1f;
    int shell = 100;

    bool isReload = false;

   // CinemachineImpulseSource impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        //tpsCmc = tpsVCam.GetComponent<CinemachineVirtualCamera>();        
        cc = GetComponent<CharacterController>();
        //impulseSource = GetComponent<CinemachineImpulseSource>();

        SetCamType(false);
    }

    // Update is called once per frame
    void Update()
    {
        MoveOrder();//ÀÌµ¿  
        RotateOrder();//Ä³¸¯ÅÍ ¹× ÃÑ±â È¸Àü

        fireDelay += Time.deltaTime;
        GunFire();//¹«±â »ç¿ë
        //Debug.Log(tpsVCam.transform.position);
    }
    private void LateUpdate()
    {
        CamRotate();//Ä«¸Þ¶ó È¸Àü
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
        tpsVCam.transform.position = this.transform.position + new Vector3(0, 1.5f, 0);

        Vector3 camAngle = tpsVCam.transform.rotation.eulerAngles;

        
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

        tpsVCam.transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDeltaPos.x, camAngle.z);
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
            fpsVCam.Priority = 11;
        }
        else
        {
            fpsVCam.Priority = 9;
        }
    }
    void GunFire()
    {
        if(fireDelay >= delayCount && isFire && shell > 0 && !isReload)
        {
            fireDelay = 0;

            GameObject bulletIst = ObjectPoolManager.Instance.DequeueObject(bullet);
            bulletIst.transform.position = firePoint.position;
            bulletIst.transform.rotation = firePoint.rotation;

            bulletIst.GetComponent<Rigidbody>().velocity = bulletIst.transform.forward * 500;

            EffectManager.Instance.FireEffectGenenate(firePoint.position, firePoint.rotation);
            //impulseSource.GenerateImpulse(this.transform.position);
            mouseDeltaPos = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 3f));

            animator.SetTrigger("Fire");
            shell--;
        }
    }
    void Reload()
    {
        animator.SetTrigger("Reload");
        StartCoroutine(ReloadEnd());
    }
    IEnumerator ReloadEnd()
    {
        yield return new WaitForSeconds(3.2f);
        isReload = false;

        shell += 100;
        shell = Mathf.Clamp(shell, 0, 101);
    }

    void OnMove(InputValue inputValue)//WASD Á¶ÀÛ
    {
        moveVectorTarget = inputValue.Get<Vector2>();//ÀÎÇ² º¤ÅÍ ¹Þ¾Æ¿È
        //moveVectorTarget = inputMovement;
        //Debug.Log(inputMovement);
    }
    void OnSprint(InputValue inputValue)
    {        
        float value = inputValue.Get<float>();
        moveSpeed = (value * 4) + 1;
        //Debug.Log(isClick);
    }
    void OnLeftClick(InputValue inputValue)//¸¶¿ì½º ÁÂÅ¬¸¯
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//´­·¶À» ¶§
        {
            isFire = true;
        }
        else//¶¿ ¶§
        {
            isFire = false;
        }        
    }
    void OnRightClick(InputValue inputValue)//¸¶¿ì½º ¿ìÅ¬¸¯
    {
        float isClick = inputValue.Get<float>();

        if (isClick == 1)//´­·¶À» ¶§
        {
            isZoom = true;            
        }
        else//¶¿ ¶§
        {
            isZoom = false;            
        }
        SetCamType(isZoom);
    }
    void OnAim(InputValue inputValue)
    {
        mouseDeltaPos = inputValue.Get<Vector2>();//ÀÎÇ² º¤ÅÍ ¹Þ¾Æ¿È        
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
