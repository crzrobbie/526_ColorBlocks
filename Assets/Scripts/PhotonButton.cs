using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonButton : MonoBehaviourPunCallbacks
{

    public menuLogic mLogic;

    public InputField createRoomInput, findRoomInput;

    public void onClickCreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
       
    }


    public void onClickFindRoom()
    {
        PhotonNetwork.JoinRoom(findRoomInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");

        mLogic.disableMenuUI();
    }



}
