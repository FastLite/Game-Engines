using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string _playerName;
    private GameObject playeritem;
    public GameObject errorPlate;
    public Text errorText;

    Dictionary<int, GameObject> leavingPlayer;    
    public string playerName
    {
        set { _playerName = value; }
        get { return _playerName; }
    }

    /******* Setup a dynamic property to pass the room name to a roomName dynamic property here ******/

    private string _roomName;

    public string roomName
    {
        set { _roomName = value; }
        get { return _roomName; }
    }

    private string _gameMode = "rm";

    public GameObject loginPanel;
    public GameObject waitingToConnectPanel;
    public GameObject gamelobbyOptionsPanel;

    public GameObject createRoomPanel;
    public GameObject creatingRoomPanel;
    public GameObject roomLobbyPanel;


    public Text roomInfoText;
    public Text gameTypeText;

    /// <summary>
    /// drag and drop 'PlayerList' under 'RoomUserPanel' into this field
    /// </summary>
    public Transform playerListHolder;


    /// <summary>
    /// drag and drop 'playerListItem' from prefabs folder into this field
    /// </summary>
    public GameObject playerListItemPrefab;

    /// <summary>
    /// drag-drop reference of 'JoiningRoomPanel' to this field
    /// </summary>
    public GameObject joiningRoomPanel;


    /// <summary>
    /// drag-drop reference of 'startGameButton' from 'RoomUserPanel' in inspector
    /// </summary>
    public UnityEngine.UI.Button startGameBtn;


    /// <summary>
    /// maintain a dictionary to keep track of all playerListItem GameObjects 
    /// instantiatted so that we can remove it from the display when a player leaves the room
    /// </summary>
    private Dictionary<int, GameObject> playerGODict;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    private void Awake()
    {
        playerGODict = new Dictionary<int, GameObject>(); //initialize this before we use it later.....
    }

    //MasterClient -> Generally, it is the one that creates a room. Think of this as the host as well
    //Client  -> all other clients who join an existing room created by the masterclietn
    
    void Connect()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// This function should be called when the login button is pressed in the UI
    /// </summary>
    public void OnLoginButtonPressed()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("<color=red> Player name not entered. Can't cannect to server without it.</color>");
        }
        else
        {
            Connect();
            waitingToConnectPanel.SetActive(true);
            loginPanel.SetActive(false);
        }
    }


    public override void OnConnected()
    {
        Debug.Log("<color=green>Connection established with Photon..</color>");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=green>" + PhotonNetwork.LocalPlayer.NickName + " got connected.</color>");

        waitingToConnectPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);
    }


    public void OnCreateRoomButtonPressed()
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("<color=red> ROom name null or empty. Can't create a room.</color>");
        }
        else
        {
            CreateRoom();
            creatingRoomPanel.SetActive(true);
            createRoomPanel.SetActive(false);
        }
    }


    void CreateRoom()
    {
        Photon.Realtime.RoomOptions ro = new Photon.Realtime.RoomOptions();

        ro.MaxPlayers = 4;

        //we use 'm' as a short-hand property to denote gameMode
        ro.CustomRoomPropertiesForLobby = new string[] {"m"};

        //grp is my field name - abbreviated for game room properties
        ExitGames.Client.Photon.Hashtable grp =
            new ExitGames.Client.Photon.Hashtable()
            {
                {"m", _gameMode} //,
                //   {"e",  },
                //  {"l",  },
            };

        ro.CustomRoomProperties = grp;

        PhotonNetwork.CreateRoom(roomName, ro);
    }

    /// <summary>
    /// called when the room is  created succcesfully
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("<color=green> Room created successfully...</color>");
        roomLobbyPanel.SetActive(true);
        creatingRoomPanel.SetActive(false);
    }

    /// <summary>
    /// called when the room creation is failed
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red> Create room failed: error code"
                  + returnCode + ". err msg: "
                  + message + "</color>");
        errorText.text = " Create room failed: error code" + returnCode.ToString() + ". err msg:" + message;
        errorPlate.SetActive(true);
        gamelobbyOptionsPanel.SetActive(true);
        creatingRoomPanel.SetActive(false);

    }


    /// <summary>
    /// Get a two letter code from the ui input, 
    /// either 'rm' for race mode. 'dm' for death match mode
    /// </summary>
    /// <param name="code"></param>
    public void SetGameMode(string code)
    {
        _gameMode = code;
        gameTypeText.text = code == "rc" ? "Racing" : "Death race";
    }


    /// <summary>
    /// Called when you join the room 
    /// (either as a client or by the masterClient right after creating the room)
    /// </summary>
    public override void OnJoinedRoom()
    {
        FindObjectOfType<CarSelector>().ShowNextPlate();
        joiningRoomPanel.SetActive(false);
        roomLobbyPanel.SetActive(true);

        Debug.Log("<color=cyan> User: " + PhotonNetwork.LocalPlayer.NickName + " joined "
                  + PhotonNetwork.CurrentRoom.Name + " ||| Players: "
                  + PhotonNetwork.CurrentRoom.PlayerCount + "/"
                  + PhotonNetwork.CurrentRoom.MaxPlayers + "</color>");

        //Output the above information in 'roomInfoText' textfield as well
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name
                                          + "  " + PhotonNetwork.CurrentRoom.PlayerCount + "/"
                                          + PhotonNetwork.CurrentRoom.MaxPlayers;


        //fetch the list of players in the room and create an instance of playerlistItem for each player
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            CreatePlayerListItem(p);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("<color=cyan> Remote User: " + newPlayer.NickName + " joined "
                  + PhotonNetwork.CurrentRoom.Name + " ||| Players: "
                  + PhotonNetwork.CurrentRoom.PlayerCount + "/"
                  + PhotonNetwork.CurrentRoom.MaxPlayers + "</color>");

        //Output the above information in 'roomInfoText' textfield as well
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name
                                          + "  " + PhotonNetwork.CurrentRoom.PlayerCount + "/"
                                          + PhotonNetwork.CurrentRoom.MaxPlayers;

        playeritem = CreatePlayerListItem(newPlayer);
        Debug.Log("Player item is" +playeritem);
    }


    /// <summary>
    /// Our function to populate the list of players who are in the room and/or entering the room
    /// </summary>
    /// <param name="newPlayer"></param>
    GameObject CreatePlayerListItem(Player newPlayer)
    {
        GameObject item = Instantiate(playerListItemPrefab, playerListHolder);
        playeritem = item;
        item.GetComponent<PlayerItemUIInfo>().Init(newPlayer.ActorNumber, newPlayer.NickName);
        playerGODict.Add(newPlayer.ActorNumber, item);


        object _isRemotePlayerReady;

        if (newPlayer.CustomProperties.TryGetValue("pReady", out _isRemotePlayerReady))
        {
            item.GetComponent<PlayerItemUIInfo>().SetReadyState((bool) _isRemotePlayerReady);
        }

        return item;

    }


    #region JOIN_RANDOM_ROOM

    /// <summary>
    /// Call this function on each of the two buttons
    /// 'RacingGameMode' and 'DeathRaceGameMode' 
    /// under 'JoinRandomRoomPanel->Background->GameModes'
    /// for race game mode, the code is 'rm'
    /// for deathgamemode, the code is 'dm'
    /// </summary>
    /// <param name="gameModeCode"></param>
    public void OnJoinRandomRoomModeButtonClicked(string gameModeCode)
    {
        Debug.Log("<color=orange> Trying to find a random room of gameMode type=" + gameModeCode + "</color>");


        ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable
            {
                {"m", gameModeCode}
            };

        PhotonNetwork.JoinRandomRoom(expectedProperties, 0);
        gameTypeText.text = gameModeCode == "rc" ? "Racing" : "Death race";

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red> Join random room failed with message = " + message + "</color>");
        errorText.text = " Join random room failed with message =" + message; 
        errorPlate.SetActive(true);
        gamelobbyOptionsPanel.SetActive(true);
        creatingRoomPanel.SetActive(false);

    }

    #endregion

    #region UPDATE_PLAYER_PROPERTIES

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        object _isRemotePlayerReady;

        if (changedProps.TryGetValue("pReady", out _isRemotePlayerReady))
        {
            playerGODict[target.ActorNumber].GetComponent<PlayerItemUIInfo>().SetReadyState((bool) _isRemotePlayerReady);
        }

        startGameBtn.interactable = IsGameReadyToStart();
    }

    #endregion


    #region GAME_READY_FUNCTION

    /// <summary>
    /// A private method that returns true if the game is ready to start
    /// </summary>
    /// <returns></returns>
    private bool IsGameReadyToStart()
    {
        //This check should only run on the MasterClient's side.
        if (!PhotonNetwork.IsMasterClient) return false;

        //check if each player in the game room is ready

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object _isRemotePlayerReady;
            if (p.CustomProperties.TryGetValue("pReady", out _isRemotePlayerReady))
            {
                if (!(bool) _isRemotePlayerReady)
                    return false;
            }
            else
            {
                Debug.LogError(
                    "Can't find pReady property. did you mis-spell in this function or in PlayerItemUIInfo??");
                return false;
            }
        }

        return true;
    }
    #endregion

    public override void OnPlayerLeftRoom(Player otherPlayer) //When Remote player leaves
    {
        Destroy(playerGODict[otherPlayer.ActorNumber]);

        playerGODict.Remove(otherPlayer.ActorNumber);
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name
                                          + "  " + PhotonNetwork.CurrentRoom.PlayerCount + "/"
                                          + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnLeftRoom() //For local player
    {
        ExitGames.Client.Photon.Hashtable properties =
            new ExitGames.Client.Photon.Hashtable()
            {
                {"pReady",false }
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        
        foreach (var VARIABLE in playerGODict )
        {
            Destroy(playerGODict[VARIABLE.Key]);
        }
        playerGODict.Clear();
    }

    /*public override void OnLeftLobby()
    {
        leavingPlayer = new Dictionary<int, GameObject>
        {
            {PhotonNetwork.LocalPlayer.ActorNumber, playeritem}
        };
        Debug.Log(leavingPlayer + "         " + PhotonNetwork.LocalPlayer.ActorNumber);
        
        playerGODict.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
        roomLobbyPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);                                        
           
        
    }*/

    public void OnLeaveButtonPressed()
    {
        PhotonNetwork.LeaveRoom();
        roomLobbyPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);
        
    }

    public void OnStartGameButtonClicked()
    {
        object gameModeCode;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m",out gameModeCode))
        {
            if ((string) gameModeCode == "rc")
            {
                PhotonNetwork.LoadLevel("RaceModeLevel");
            }
            else if ((string) gameModeCode == "dm")
            {
                PhotonNetwork.LoadLevel("DeathModeLevel");

            }
            else
            {
                Debug.Log("You are tryying to acceess wrong property, check it" + gameModeCode);
            }
        }
    }
    
    
    
    
}