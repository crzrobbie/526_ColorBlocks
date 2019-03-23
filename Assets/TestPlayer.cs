using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class TestPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed = 1f;
    public PhotonView photonView;
    public GameObject body;

    Vector2 dir = Vector2.right;

    List<Transform> tail = new List<Transform>();
    List<Transform> domain = new List<Transform>();

    bool inDomain = false;

    float minX;
    float minY;
    float maxX;
    float maxY;


    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            minX = transform.position.x;
            maxX = transform.position.x;
            minY = transform.position.y;
            maxY = transform.position.y;
            createDomain(); //create start block
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
        transform.Translate(dir);
        Vector2 vector = transform.position;
        updateField(vector);
        bool isContain = isContains(domain, vector);
        if (!inDomain && isContain)
        {
            fill();
            inDomain = true;
        }
        if (!isContain)
        {
            GameObject trace = PhotonNetwork.Instantiate(body.name, vector, Quaternion.identity, 0);
            tail.Insert(0, trace.transform);
            inDomain = false;
        }

    }

    private void fill()
    {
        domain.InsertRange(0, tail);
        tail.Clear();
        Hashtable xTable = new Hashtable();
        Hashtable yTable = new Hashtable();
        foreach (Transform t in domain)
        {
            float x = t.position.x;
            float y = t.position.y;
            if (xTable.ContainsKey(x))
            {
                List<Transform> xList = (List<Transform>)xTable[x];
                xList.Add(t);
            }
            else
            {
                List<Transform> list = new List<Transform>();
                list.Add(t);
                xTable.Add(x, list);
            }
            if (yTable.ContainsKey(y))
            {
                List<Transform> yList = (List<Transform>)yTable[y];
                yList.Add(t);
            }
            else
            {
                List<Transform> list = new List<Transform>();
                list.Add(t);
                yTable.Add(y, list);
            }
        }
        for (float i = minX; i <= maxX; i++)
        {
            for (float j = minY; j <= maxY; j++)
            {
                List<Transform> xList = (List<Transform>)xTable[i];
                List<Transform> yList = (List<Transform>)yTable[j];
                if (isInDomain(xList, yList, i, j))
                {
                    Vector2 vector = new Vector2(i, j);
                    PhotonNetwork.Instantiate(body.name, vector, Quaternion.identity, 0);
                    //domain.Insert(0, body.transform);
                }

            }
        }
    }

    private bool isInDomain(List<Transform> xList, List<Transform> yList, float x, float y)
    {
        if (xList.Count == 0 || yList.Count == 0) return false;
        float minY = xList[0].position.y, maxY = xList[0].position.y;
        foreach (Transform t in xList)
        {
            minY = Math.Min(minY, t.position.y);
            maxY = Math.Max(maxY, t.position.y);
        }
        float minX = yList[0].position.x, maxX = yList[0].position.x;
        foreach (Transform t in yList)
        {
            minX = Math.Min(minX, t.position.x);
            maxX = Math.Max(maxX, t.position.x);
        }
        if (x >= minX && x <= maxX && y >= minY && y <= maxY) return true;
        return false;
    }

    private bool isContains(List<Transform> domain, Vector2 v)
    {
        foreach (Transform t in domain)
        {
            if (t.position.x == v.x && t.position.y == v.y)
                return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        Debug.Log("Collid:"+collision.name);
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

    void createDomain()
    {
        Vector2 vector = transform.position;
        int width = 3;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2 v = new Vector2(vector.x - i, vector.y + 1 - j);
                updateField(v);
                GameObject d = PhotonNetwork.Instantiate(body.name, v, Quaternion.identity, 0);
                d.name = "domain";
                domain.Insert(0, d.transform);
                //Debug.Log(v);
            }
        }
    }

    private void updateField(Vector2 v)
    {
        minX = Math.Min(minX, v.x);
        maxX = Math.Max(maxX, v.x);
        minY = Math.Min(minY, v.y);
        maxY = Math.Max(maxY, v.y);
    }

}
