using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> playerStartPoints;
    public List<GameObject> playerPrefabs;
    public Text countDownText;
    public GameObject CountDownGameObject;

    public GameObject myCarInstance;
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance!=this)
        {
            Destroy(gameObject);
        }
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
}
