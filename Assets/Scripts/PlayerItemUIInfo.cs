using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUIInfo : MonoBehaviour
{
    public Text playerName;
    public Button readyBtn;
    public Image readyImg;
    public void Init(int pNum, string pName)
    {
        playerName.text =  pName;
        //We don't show the ready button if the player is not the 'local player'
        //only the local player can press the 'ready' button
        if (PhotonNetwork.LocalPlayer.ActorNumber != pNum)
        {
            readyBtn.gameObject.SetActive(false);
        }
        else
            readyBtn.gameObject.SetActive(true);
    }
    
    public void LeaveGameButtonPressed()
    {
        Destroy(gameObject);
    }
    
    #region READY_BUTTON
    public void OnReadyButtonClicked()
    {
        //Transmit to photon network that our local player just pressed the ready button
        ExitGames.Client.Photon.Hashtable properties =
            new ExitGames.Client.Photon.Hashtable()
            {
                {"pReady",true }
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        SetReadyState(true);
        readyBtn.gameObject.SetActive(false);
    }
    /// <summary>
    /// This is called on a localplayer's instance via OnReadyButtonClicked()
    /// Also for every remote player that joins the room (and when a remote player marks as ready)
    /// this function will be called as well (via NetworkMgr)
    /// </summary>
    /// <param name="isReady"></param>
    public void SetReadyState(bool isReady)
    {
        if(isReady)
        {
            readyImg.enabled = true;
        }
    }
    #endregion
}
