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
    private int lastAssignedRank;
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<count>().lastrank++;
        lastAssignedRank = other.GetComponent<count>().lastrank;
        if (!photonView.IsMine)
            return;
        if (other.CompareTag("Lap"))
        {
            Debug.Log("Lap trigger crossed");
            EndRace(false);

        }
        else if(other.CompareTag("Finish"))
        {
            gameObject.GetComponentInChildren<Camera>().gameObject.transform.parent = null;
            GetComponent<CarMovementController>().enabled = false;
            if (lastAssignedRank==PhotonNetwork.CurrentRoom.PlayerCount)
            {
                GameManager.instance.ShowScore();
                EndRace(true);
            }
            Debug.Log("Car reached finish");
            
        }
        
    }

    void EndRace(bool realEnd)
    {
        if (!photonView.IsMine)
            return;
        string currentPlayerNN = photonView.Owner.NickName;
        object[] data = {currentPlayerNN, photonView.ViewID, lastAssignedRank, photonView.IsMine};
        PhotonNetwork.RaiseEvent((byte) GameManager.raiseEventCodes.raceFinishUpdateRank,data
            ,new RaiseEventOptions(){Receivers = ReceiverGroup.MasterClient}
            ,new SendOptions(){Reliability = false} );
        
        
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
