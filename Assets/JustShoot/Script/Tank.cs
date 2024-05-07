using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tank : SceneSingleton<Tank>
{
    Vector3 moveVector;
    Vector3 rotateVector;
    Rigidbody rigidbody;

    float movePower = 40;
    float rotatePower = 10;

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

        Vector3 turretdirection = turret.InverseTransformDirection(direction);
        turretdirection = new Vector3(turretdirection.x, 0, turretdirection.z);        
        //rotationBody = Quaternion.Euler(0, rotationBody.eulerAngles.y, 0);
        turret.Rotate(Vector3.up, Mathf.Clamp((turretdirection.x) * 270, -90, 90) * Time.deltaTime);

        if (true)
        {
            direction = Quaternion.AngleAxis(-15, cannon.right) * direction;
            Vector3 cannondirection = cannon.InverseTransformDirection(direction);
            cannondirection = new Vector3(0, cannondirection.y, 0);
            //rotationBody = Quaternion.Euler(0, rotationBody.eulerAngles.y, 0);
            cannon.Rotate(Vector3.right, Mathf.Clamp((-cannondirection.y) * 270, -90, 90) * Time.deltaTime);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && (moveVector != Vector3.zero || rotateVector != Vector3.zero))
        {
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(10000);
        }
    }
}
