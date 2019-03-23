using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestPlayer1 : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed = 1f;
    public PhotonView photonView;
    public GameObject body;

    Vector2 dir = Vector2.right;



    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            InvokeRepeating("Move", speed, speed);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left)
            dir = Vector2.right;
        else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
            dir = Vector2.left;
        else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down)
            dir = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
            dir = Vector2.down;
    }

    void Move()
    {
        Vector2 vector = transform.position;
        transform.Translate(dir);
        PhotonNetwork.Instantiate(body.name, vector, Quaternion.identity, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        Debug.Log("Collid:" + collision.name);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }

}
