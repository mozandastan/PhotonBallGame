using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class RoomController : MonoBehaviourPunCallbacks
{
    //Player instance prefab, must be located in the Resources folder
    public GameObject playerPrefab;
    //Player spawn point
    public Transform spawnPoint;
    [SerializeField] TextMeshProUGUI roomNameText, playerTextObj;
    [SerializeField] GameObject canvasPanelObj;

    public static RoomController Instance;

    void Start()
    {
        Instance = this;

        //In case we started this demo with the wrong scene being active, simply load the menu scene
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
            return;
        }
        else
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name + "   " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            
        }

        Invoke("instDelay", 1);
        //We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
       
    }

    public void instDelay()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);

        ResfreshPlayerList();

    }

    private void ResfreshPlayerList()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + "   " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        for (int i = 0; i < canvasPanelObj.transform.childCount; i++)
        {
            Destroy(canvasPanelObj.transform.GetChild(i).gameObject);
        }

        int posYPlayerText = -65;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Show if this player is a Master Client. There can only be one Master Client per Room so use this to define the authoritative logic etc.)
            GameObject playerTextObjTemp = Instantiate(playerTextObj.gameObject, canvasPanelObj.transform);
            playerTextObjTemp.transform.localScale = new Vector3(1, 1, 1);
            playerTextObjTemp.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(playerTextObjTemp.transform.localPosition.x, posYPlayerText, playerTextObjTemp.transform.localPosition.z);
            posYPlayerText -= 30;

            string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": Owner" : "");
            playerTextObjTemp.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName + isMasterClient + PhotonNetwork.PlayerList[i].CustomProperties["deaths"];
        }
    }

    public void LeaveRoomBtnFnc()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " EnteredRoom");
        ResfreshPlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " LeftRoom");
        ResfreshPlayerList();
    }
}
