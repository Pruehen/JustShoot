using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollWeapon : MonoBehaviour
{

    public float rotatePower = 80f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, rotatePower, 0f) * Time.deltaTime, Space.World);
    }
}
