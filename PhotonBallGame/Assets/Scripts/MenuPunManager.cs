using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MenuPunManager : MonoBehaviourPunCallbacks
{
    //Our player name
    string playerName = "Player 1";
    //Users are separated from each other by gameversion (which allows you to make breaking changes).
    string gameVersion = "0.1";
    //The list of created rooms
    List<RoomInfo> createdRooms = new List<RoomInfo>();
    //Use this name when creating a Room
    string roomName = "Room 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;

    

    [SerializeField] private GameObject canvasObj, RoomPanel, ContentObj;
    [SerializeField] private TextMeshProUGUI StatusText;
    [SerializeField] private TMP_InputField CreateRoomNameInput, PlayerNameInput;

    public GameObject roomObj;

    // Use this for initialization
    void Start()
    {

        //This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            //Set the App version before connecting
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (joiningRoom || !PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState != ClientState.JoinedLobby && canvasObj.GetComponent<GraphicRaycaster>().enabled)
        {
            canvasObj.GetComponent<GraphicRaycaster>().enabled = false;

        }
        if (!joiningRoom && PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState == ClientState.JoinedLobby && !canvasObj.GetComponent<GraphicRaycaster>().enabled)
        {
            canvasObj.GetComponent<GraphicRaycaster>().enabled = true;
        }
    }

    public void CreateRoomBtnFnc()
    {
        if (roomName != "")
        {
            roomName = CreateRoomNameInput.text;

            joiningRoom = true;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = (byte)10; //Set any number

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    private void ListRoomsFnc()
    {
        for (int i = 0; i < ContentObj.transform.childCount; i++)
        {
            Destroy(ContentObj.transform.GetChild(i).gameObject);
        }
        int roomYSpace = -35;

        for (int i = 0; i < createdRooms.Count; i++)
        {
            GameObject roomObjTemp = Instantiate(roomObj, ContentObj.transform);
            roomObjTemp.transform.localScale = new Vector3(1, 1, 1);
            roomObjTemp.transform.localPosition = new Vector3(roomObjTemp.transform.localPosition.x, roomYSpace, roomObjTemp.transform.localPosition.z);
            roomYSpace -= 65;

            var index = i;
            roomObjTemp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = createdRooms[i].Name + "   " + createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers;
            roomObjTemp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => JoinRoomBtnFnc(index));

        }
    }

    public void JoinRoomBtnFnc(int roomNum)
    {
        joiningRoom = true;

        //Set our Player name
        playerName = PlayerNameInput.text;
        PhotonNetwork.NickName = playerName;

        //Join the Room
        PhotonNetwork.JoinRoom(createdRooms[roomNum].Name);
    }
    public void RefreshRoomsBtnFnc()
    {
        if (PhotonNetwork.IsConnected)
        {
            //Re-join Lobby to get the latest Room list
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
        else
        {
            //We are not connected, estabilish a new connection
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
    }

    public override void OnConnectedToMaster()
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("OnConnectedToMaster");
        //After we connected to Master server, join the Lobby
        PhotonNetwork.JoinLobby(TypedLobby.Default);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("We have received the Room list");
        //After this callback, update the room list
        createdRooms = roomList;

        ListRoomsFnc();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("OnCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        joiningRoom = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    public override void OnCreatedRoom()
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("OnCreatedRoom");
        //Set our player name
        playerName = PlayerNameInput.text;
        PhotonNetwork.NickName = playerName;
        //Load the Scene called GameLevel (Make sure it's added to build settings)
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinedRoom()
    {
        StatusText.text = "Status: " + PhotonNetwork.NetworkClientState;
        Debug.Log("OnJoinedRoom");
    }

}
