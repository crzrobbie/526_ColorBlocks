using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class Block1 : MonoBehaviour
{
    /**
     * create blocks using thread
     */

    public float speed = 0.3f;
    public GameObject tailPrefab;

    Vector2 dir = Vector2.right;

    Hashtable domain = new Hashtable();
    Hashtable tail = new Hashtable();

    List<Vector2> toCreateBlockList = new List<Vector2>();

    bool inDomain = false;

    float minX;
    float minY;
    float maxX;
    float maxY;

    float startX;
    float startY;

    void Start()
    {
        //initialize value
        startX = transform.position.x;
        minX = transform.position.x;
        maxX = transform.position.x;

        startY = transform.position.y;
        minY = transform.position.y;
        maxY = transform.position.y;
        createDomain(); //create start block
        InvokeRepeating("Move", speed, speed); //control
    }

    void Move()
    {
        //Debug.Log("before transform:"+vector);
        Vector2 vector = transform.position;
        transform.Translate(dir);
        string key = vector.ToString();

        if (!domain.ContainsKey(key) && !tail.ContainsKey(key))
        {
            inDomain = false;
            GameObject trace = Instantiate(tailPrefab, vector, Quaternion.identity);
            tail.Add(key, trace);
            updateField(vector);
        }
        //Debug.Log("MinX:" + minX + " MaxX:" + maxX + " MinY:" + minY + " MaxY:" + maxY);
    }

    
    void Update()
    {
        Control();

        while(toCreateBlockList.Count > 0)
        {
            Vector2 v = toCreateBlockList[0];
            if (!domain.ContainsKey(v.ToString()))
            {
                GameObject body = Instantiate(tailPrefab, v, Quaternion.identity);
                domain.Add(v.ToString(), body);
            }
            toCreateBlockList.RemoveAt(0);
        }

        /*
        if (toCreateBlockList.Count > 0)
        {
            for (int i = 0; i < toCreateBlockList.Count; i++)
            {
                Vector2 v = toCreateBlockList[i];
                if (!domain.ContainsKey(v.ToString()))
                {
                    GameObject body = Instantiate(tailPrefab, v, Quaternion.identity);
                    domain.Add(v.ToString(), body);
                }
            }
            toCreateBlockList.Clear();
        }
        */
    }

    private void Control()
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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2 v = new Vector2(vector.x - i, vector.y + 1 - j);
                GameObject d = Instantiate(tailPrefab, v, Quaternion.identity);
                domain.Add(v.ToString(), d);
                updateField(v);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float x = collision.transform.position.x;
        float y = collision.transform.position.y;
        if(x==startX && y==startY)
        {
            Debug.Log("Start Point");
        }
        else
        {
            string position = ((Vector2)collision.transform.position).ToString();
            if (domain.ContainsKey(position))
            {
                //collide domain
                //Debug.Log("collide domain");
                if(!inDomain)
                {
                    //first collide domain
                    inDomain = true;
                    Debug.Log("first collide domain");
                    //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    //stopwatch.Start();
                    //fill();
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //Debug.Log(timeSpan.TotalMilliseconds);
                    float[] limit = { minX, maxX, minY, maxY };
                    new Thread(new ParameterizedThreadStart(fill)).Start(limit);
                }
                else
                {
                    //head is in domain
                }
            }
            else if (tail.ContainsKey(position))
            {
                //collide tail
                Debug.Log("collide tail");
                //TODO clear all objects and game over
                ICollection key = domain.Keys;
                foreach(string k in key)
                {
                    Destroy((GameObject)domain[k]);
                }
                domain.Clear();
                key = tail.Keys;
                foreach(string k in key)
                {
                    Destroy((GameObject)tail[k]);
                }
                tail.Clear();
                Destroy(this);
            }
            else
            {
                //collide border
            }
        }
        

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Collision stay");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Collision finish");
    }

    /*
     * limit[0] = minX
     * limit[1] = maxX
     * limit[2] = minY
     * limit[3] = maxY
     */
    private void fill(object obj)
    {
        float[] limit = obj as float[];
        ICollection key = tail.Keys;
        foreach(string k in key)
        {
            if (!domain.ContainsKey(k))
            {
                domain.Add(k, tail[k]);
            } else
            {
                Debug.Log("Contain key");
            }
        }
        tail.Clear();
        for(float i = limit[0]; i <= limit[1]; i++)
        {
            for(float j = limit[2]; j <= limit[3]; j++)
            {
                Vector2 v = new Vector2(i, j);
                if(!domain.ContainsKey(v.ToString()))
                {
                    if(isInDomain(i,j,limit))
                    {
                        //GameObject body = Instantiate(tailPrefab, v, Quaternion.identity);
                        //domain.Add(v.ToString(), body);
                        toCreateBlockList.Add(v);
                    }
                }
            }
        }
    }

    private bool isInDomain(float x, float y, float[]limit)
    {
        bool hasLeft = false;
        bool hasRight = false;
        bool hasUp = false;
        bool hasBottom = false;
        for(float i = limit[2]; i < y; i++)
        {
            Vector2 v = new Vector2(x, i);
            if(domain.ContainsKey(v.ToString()))
            {
                hasBottom = true; break;
            }
        }

        for(float i = y+1; i <= limit[3]; i++)
        {
            Vector2 v = new Vector2(x, i);
            if (domain.ContainsKey(v.ToString()))
            {
                hasUp = true; break;
            }
        }
        for(float i = limit[0]; i < x; i++)
        {
            Vector2 v = new Vector2(i, y);
            if (domain.ContainsKey(v.ToString()))
            {
                hasLeft = true; break;
            }
        }
        for(float i = x+1; i <= limit[1]; i++)
        {
            Vector2 v = new Vector2(i, y);
            if(domain.ContainsKey(v.ToString()))
            {
                hasRight = true; break;
            }
        }
        return hasLeft && hasRight && hasUp && hasBottom;
    }
}
