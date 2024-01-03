using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun, IPunObservable
{

    public MonoBehaviour[] localScripts;
    public GameObject[] localObjects;
    Vector3 latestPos;
    Quaternion latestRot;
    Vector3 velocity;
    Vector3 angularVelocity;
    Rigidbody r;
    bool valuesReceived = false;

    void Start()
    {
        r = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            //Player is local
            gameObject.tag = "Player";

            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            //Player is Remote, deactivate the scripts and object that should only be enabled for the local player
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }

            GetComponent<Renderer>().material.color = Color.grey;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(r.velocity);
            stream.SendNext(r.angularVelocity);
            
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            angularVelocity = (Vector3)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && valuesReceived)
        {
            //Update Object position and Rigidbody parameters
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            r.velocity = velocity;
            r.angularVelocity = angularVelocity;
        }
        else
        {
            if (this.transform.position.y <= -10)
            {
                PhotonNetwork.Destroy(this.gameObject);
                RoomController.Instance.instDelay();
            }
        }
    }

}
