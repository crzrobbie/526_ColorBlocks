using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class photonConnect : MonoBehaviourPunCallbacks
{
    public GameObject sectionView1, sectionView2;

    public void connectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();

        

        Debug.Log("Connecting to photon...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");

        sectionView1.SetActive(false);
        sectionView2.SetActive(true);
        //PhotonNetwork.JoinRandomRoom();
    }

}
