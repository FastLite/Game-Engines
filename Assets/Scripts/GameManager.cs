using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<PlayerStandingUIItem> standingsUIList;
    public enum raiseEventCodes
    {
        raceFinishCode = 0,
        raceFinishUpdateRank = 1
    };
    private int lastAssignedRank = 0;
    public List<Transform> playerStartPoints;
    public List<GameObject> playerPrefabs;
    public Text countDownText;
    public GameObject CountDownGameObject;

    public GameObject myCarInstance;
    public static GameManager instance = null;

    public List<GO_ID_Duo> playerRanks;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance!=this)
        {
            Destroy(gameObject);
        }
        playerRanks = new List<GO_ID_Duo>();
    }
    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Not connected to photon network");
            return;
        }

        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        Vector3 startPos = playerStartPoints[actorNum - 1].position;
        
         myCarInstance = PhotonNetwork.Instantiate(playerPrefabs[PlayerPrefs.GetInt("carId")].name, startPos,Quaternion.identity);

    }

    void CheckAndUpdateUI(int photonViewID)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        lastAssignedRank++;
        int indx = playerRanks.FindIndex(x => x.viewID == photonViewID);
        playerRanks[indx].rank = lastAssignedRank;
        object data = new object[] {playerRanks[indx].viewID, playerRanks[indx].rank};
        PhotonNetwork.RaiseEvent((byte) raiseEventCodes.raceFinishCode, data,
            new RaiseEventOptions() {Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
    }

    public void OnEventCallBack(EventData photonEventData)
    {
        if (photonEventData.Code ==(byte)raiseEventCodes.raceFinishUpdateRank )
        {
            object[] incomingData = (object[]) photonEventData.CustomData;
            CheckAndUpdateUI((int)incomingData[1]);

        }
        else if (photonEventData.Code == (byte)raiseEventCodes.raceFinishCode)
        {
            object[] incomingData = (object[]) photonEventData.CustomData;
            int viewId = (int)incomingData[0];
            int rank = (int)incomingData[1];

            int indx = playerRanks.FindIndex(x => x.viewID == viewId);
            playerRanks[indx].rank = rank;
            Debug.Log(playerRanks[indx].pv.Owner.NickName + " FINIXSHED AT POSITION: " + rank);
            
            
        }
       
    }
    
}
