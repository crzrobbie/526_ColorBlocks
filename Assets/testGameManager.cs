using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class testGameManager : MonoBehaviourPun
{


    int num = 0;
    public GameObject player;
    public GameObject body;

    Color[] playerColor = { Color.green, Color.blue, Color.red, Color.yellow };


    // Start is called before the first frame update
    void Start()
    {
        int playerNum = PhotonNetwork.CurrentRoom.PlayerCount;
        player.GetComponent<SpriteRenderer>().color = playerColor[playerNum + 1];
        body.GetComponent<SpriteRenderer>().color = playerColor[playerNum + 1];
        PhotonNetwork.Instantiate(this.player.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
