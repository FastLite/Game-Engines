using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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
            EndRace(false);

        }
        else if(other.CompareTag("Finish"))
        {
            Debug.Log("Car reached finish");
            GameManager.instance.ShowScore();
            EndRace(true);
        }
        
    }

    void EndRace(bool realEnd)
    {
        if (!photonView.IsMine)
            return;
        string currentPlayerNN = photonView.Owner.NickName;
        object[] data = new object[]{currentPlayerNN, photonView.ViewID};
        PhotonNetwork.RaiseEvent((byte) GameManager.raiseEventCodes.raceFinishUpdateRank,data
            ,new RaiseEventOptions(){Receivers = ReceiverGroup.MasterClient}
            ,new SendOptions(){Reliability = false} );
        if (!realEnd)
            return;
        gameObject.GetComponentInChildren<Camera>().gameObject.transform.parent = null;
        GetComponent<CarMovementController>().enabled = false;
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += GameManager.instance.OnEventCallBack;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= GameManager.instance.OnEventCallBack;
    }
}
