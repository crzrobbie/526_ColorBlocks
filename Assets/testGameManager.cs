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
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(this.player.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addBodyObject(Vector2 vector)
    {
        PhotonNetwork.Instantiate(body.name, vector, Quaternion.identity, 0);
        num++;
        Debug.Log(num);
    }
}
