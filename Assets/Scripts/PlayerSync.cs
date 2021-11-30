using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CarMovementController>().enabled = photonView.IsMine;
        
        GetComponentInChildren<Camera>().gameObject.SetActive(photonView.IsMine);
    }

    
}
