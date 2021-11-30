using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Photon.Pun;
using UnityEngine;

public class LapController : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        return;
        if (other.CompareTag("Lap"))
        {
            Debug.Log("Lap trigger crossed");
        }
        else if(other.CompareTag("Finish"))
        {
            Debug.Log("Car reached finish");
            EndRace();
        }
        
    }

    void EndRace()
    {
        if (!photonView.IsMine)
            return;
        gameObject.GetComponentInChildren<Camera>().gameObject.transform.parent = null;
        GetComponent<CarMovementController>().enabled = false;

    }
    
}
