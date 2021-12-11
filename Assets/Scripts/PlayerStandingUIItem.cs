﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStandingUIItem : MonoBehaviour
{
    public Text playerNameText;
    public Text playerRankText;
    public Text playerTime;

   public void UpdateInfo(string plName, int plRank,bool isPlayerYou)
    {
        playerNameText.text = plName;
        playerRankText.text = plRank.ToString();
        playerTime.text = (Time.timeSinceLevelLoad - 3).ToString("#.00");

        if (isPlayerYou)
        {
            playerNameText.color = Color.red;
            playerRankText.color = Color.red;
        }
        //TO - DO:: remember to test the game and update/add code here to fix the problems you will identify

    }

}
