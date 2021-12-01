using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSync : MonoBehaviourPun
{
    public Text label;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CarMovementController>().enabled = photonView.IsMine;
        
        GetComponentInChildren<Camera>().gameObject.SetActive(photonView.IsMine);
        GetComponent<LapController>().enabled = photonView.IsMine;
        GO_ID_Duo duo = new GO_ID_Duo(gameObject, photonView.ViewID);
        if (!GameManager.instance.playerRanks.Exists(x=>x.viewID == photonView.ViewID))
        {
            GameManager.instance.playerRanks.Add(duo);
        }
        label.text = photonView.Owner.NickName;

        if (photonView.IsMine)
        {
            GetComponentInChildren<Text>().enabled = false;
        }
    }

    
}
