using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviourPun
{
    public float countDownStartValue = 3;
    private float timer;
    void Start()
    {
        timer = countDownStartValue;
    }

    private void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        if (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            photonView.RPC("UpdateTimerText", RpcTarget.All);
        }
        else
        {
            photonView.RPC("StartRace", RpcTarget.All);

        }
    }
    [PunRPC]
    void StartRace()
    {
        GetComponent<CarMovementController>().EnableMovement();
    }

    [PunRPC]
    void UpdateTimerText(float time)
    {
        if (time>0.0f)
        {
            GameManager.instance.countDownText.text = Mathf.RoundToInt(time).ToString();
            
        }
        else
        {
            GameManager.instance.countDownText.text = "";

            GameManager.instance.countDownText.transform.parent.gameObject.SetActive(false);
        }
        
    }
}
