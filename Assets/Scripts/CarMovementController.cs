using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarMovementController : MonoBehaviour
{

    Rigidbody rb;
    public carParamSO carType;
    public Vector3 thrust; //50z
    public Vector3 rotationTorque;//10y

    private bool isMovementEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        thrust = carType.speed;
        rotationTorque = carType.handeling;
        rb = GetComponent<Rigidbody>();

    }

    /*** enables car movement after countdown is over ***/
    public void EnableMovement()
    {
        isMovementEnabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isMovementEnabled) return;

        //forward movement
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(thrust);
        }
        //backward movement
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(-thrust);
        }
        //steer left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(-rotationTorque);
        }
        //steer right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(rotationTorque);
        }

    }

}