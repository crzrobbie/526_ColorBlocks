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

    Color[] playerColor = { Color.red, Color.blue, Color.yellow, Color.white };

    Vector3[] startPositions = { new Vector3(-16, 13, 0), new Vector3(20, 13, 0), new Vector3(-16, -13, 0), new Vector3(20, -13, 0) };

    Vector2[] directions = { Vector2.right, Vector2.left, Vector2.right, Vector2.left }; 

    // Start is called before the first frame update
    void Start()
    {
        int playerNum = PhotonNetwork.CurrentRoom.PlayerCount;

        player.GetComponent<SpriteRenderer>().color = playerColor[playerNum - 1];
        body.GetComponent<SpriteRenderer>().color = playerColor[playerNum - 1];
        GameObject head = PhotonNetwork.Instantiate(this.player.name, startPositions[playerNum - 1], Quaternion.identity, 0);

        head.name = playerNum.ToString()+"_head";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
