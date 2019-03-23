using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Block : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 0.3f;
    public GameObject tailPrefab;

    Vector2 dir = Vector2.right;

    List<Transform> tail = new List<Transform>();
    List<Transform> domain = new List<Transform>();

    bool inDomain = false;

    float minX;
    float minY;
    float maxX;
    float maxY;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    void Start()
    {
        //initialize value
        minX = transform.position.x;
        maxX = transform.position.x;
        minY = transform.position.y;
        maxY = transform.position.y;
        createDomain(); //create start block
        InvokeRepeating("Move", speed, speed); //control
    }

    void Move()
    {
        //Debug.Log("before transform:"+vector);
        
        transform.Translate(dir);
        Vector2 vector = transform.position;
        updateField(vector);
        bool isContain = isContains(domain, vector);
        if (!inDomain && isContain)
        {
            //fill the content
            //Debug.Log("To fill the content");
            //Thread thread = new Thread(fill);
            //thread.Start();
            stopwatch.Start();
            fill();
            inDomain = true;
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Debug.Log("total time:" + timeSpan.TotalMilliseconds);
        }
        if (!isContain)
        {
            GameObject trace = Instantiate(tailPrefab, vector, Quaternion.identity);
            tail.Insert(0, trace.transform);
            inDomain = false;
        }
        
        //Debug.Log("MinX:" + minX + " MaxX:" + maxX + " MinY:" + minY + " MaxY:" + maxY);
    }

    private void fill()
    {
        domain.InsertRange(0, tail);
        tail.Clear();
        Hashtable xTable = new Hashtable();
        Hashtable yTable = new Hashtable();
        foreach(Transform t in domain)
        {
            float x = t.position.x;
            float y = t.position.y;
            if(xTable.ContainsKey(x))
            {
                List<Transform> xList = (List<Transform>) xTable[x];
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
                List<Transform> yList = (List<Transform>) yTable[y];
                yList.Add(t);
            }
            else
            {
                List<Transform> list = new List<Transform>();
                list.Add(t);
                yTable.Add(y, list);
            }
        }
        for(float i = minX; i <= maxX; i++)
        {
            for(float j = minY; j <= maxY; j++)
            {
                List<Transform> xList = (List<Transform>) xTable[i];
                List<Transform> yList = (List<Transform>) yTable[j];
                if(isInDomain(xList,yList,i,j))
                {
                    Vector2 vector = new Vector2(i, j);
                    GameObject body = Instantiate(tailPrefab, vector, Quaternion.identity);
                    //domain.Insert(0, body.transform);
                }

            }
        }
    }

    private bool isInDomain(List<Transform> xList, List<Transform> yList, float x, float y)
    {
        if (xList.Count == 0 || yList.Count == 0) return false;
        float minY = xList[0].position.y, maxY = xList[0].position.y;
        foreach(Transform t in xList)
        {
            minY = Math.Min(minY, t.position.y);
            maxY = Math.Max(maxY, t.position.y);
        }
        float minX = yList[0].position.x, maxX = yList[0].position.x;
        foreach(Transform t in yList)
        {
            minX = Math.Min(minX, t.position.x);
            maxX = Math.Max(maxX, t.position.x);
        }
        if (x >= minX && x <= maxX && y >= minY && y <= maxY) return true;
        return false;
    }

    private bool isContains(List<Transform> domain, Vector2 v)
    {
        foreach(Transform t in domain)
        {
            if (t.position.x == v.x && t.position.y == v.y)
                return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left)
            dir = Vector2.right;
        else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
            dir = Vector2.left;
        else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down)
            dir = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
            dir = Vector2.down;
        
    }

    void createDomain()
    {
        Vector2 vector = transform.position;
        int width = 3;
        for(int i=0; i < width; i++)
        {
            for(int j = 0; j < width; j++)
            {
                Vector2 v = new Vector2(vector.x-i,vector.y+1-j);
                updateField(v);
                GameObject d = Instantiate(tailPrefab, v, Quaternion.identity);
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
