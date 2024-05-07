using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank : SceneSingleton<Tank>
{
    Vector3 moveVector;
    Vector3 rotateVector;
    Rigidbody rigidbody;

    float movePower = 30;
    float rotatePower = 30;

    public Transform turret;
    public Transform cannon;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.onTank)
        {
            rigidbody.AddForce(moveVector * movePower, ForceMode.Acceleration);
            rigidbody.AddTorque(rotateVector * rotatePower, ForceMode.Acceleration);

            Vector3 camForward = Player.Instance.CamForward();
            RotateOrder(camForward);
        }        
    }

    void RotateOrder(Vector3 camForward)
    {
        Vector3 direction = camForward.normalized;

        Quaternion rotationWeapon = Quaternion.LookRotation(direction);
        rotationWeapon = Quaternion.Euler(rotationWeapon.eulerAngles.x, cannon.rotation.eulerAngles.y, rotationWeapon.eulerAngles.z);
        cannon.rotation = Quaternion.Slerp(cannon.rotation, rotationWeapon, Time.deltaTime * 0.4f);

        direction = new Vector3(direction.x, 0, direction.z);

        Quaternion rotationBody = Quaternion.LookRotation(direction);
        //rotationBody = Quaternion.Euler(0, rotationBody.eulerAngles.y, 0);
        turret.Rotate(Vector3.up, Mathf.Clamp((turret.forward.x - direction.x) * 10, 0, 90 * Time.deltaTime));
    }

    void OnMove(InputValue inputValue)//WASD ¡∂¿€
    {
        Vector2 moveVector2 = inputValue.Get<Vector2>();//¿Œ«≤ ∫§≈Õ πﬁæ∆ø»
        Debug.Log(moveVector2);
        moveVector = this.transform.forward * moveVector2.y;
        rotateVector = this.transform.up * moveVector2.x;
        //moveVectorTarget = inputMovement;
        //Debug.Log(inputMovement);
    }    
    bool isFire;
    void OnLeftClick(InputValue inputValue)//∏∂øÏΩ∫ ¡¬≈¨∏Ø
    {
        if (Player.Instance.onTank)
        {
            float isClick = inputValue.Get<float>();

            if (isClick == 1)//¥≠∑∂¿ª ∂ß
            {
                isFire = true;
            }
            else//∂ø ∂ß
            {
                isFire = false;
            }
            //controlweapon.SetTrigger(isFire);
        }
    }
}
