using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BallMove : MonoBehaviour
{

    private float ForceFactor = 3f;
    private Rigidbody rbBall;

    void Start()
    {
        rbBall = GetComponent<Rigidbody>();

    }


    private void Update()
    {

            if (Input.GetKey(KeyCode.W))
            {
                rbBall.AddForce(Vector3.forward * ForceFactor);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rbBall.AddForce(Vector3.back * ForceFactor);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rbBall.AddForce(Vector3.left * ForceFactor);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rbBall.AddForce(Vector3.right * ForceFactor);
            }

            Camera.main.transform.position = this.gameObject.transform.position + new Vector3(0, 3, -7);

    }


}
